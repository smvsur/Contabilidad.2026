using Contabilidad.Data.Facturas.Input;
using Contabilidad.Data.Facturas.Output;
using Contabilidad.Data.IVA;
using DEB;
using System.Text;

namespace Contabilidad.Analyze
{
    public class TrimestralIVA : IComponentAnalyze
    {
        //=====================================================================
        public void Process(ProcessContext context)
        {
            var facturasOutput = (context.Components
                .FirstOrDefault(x => x is FacturaOutputComponent)
                as FacturaOutputComponent)?
                .GetModels();

            var facturasInput = (context.Components
                .FirstOrDefault(x => x is FacturaInputComponent)
                as FacturaInputComponent)?
                .GetModels();

            var iva = (context.Components
                .FirstOrDefault(x => x is IVAComponent)
                as IVAComponent)?
                .GetModels();

            var ivaRepercutido = iva.Where(x => !x.IsSoportado).ToArray();
            var ivaSoportado = iva.Where(x => x.IsSoportado).ToArray();

            //-----------------------------------------------------------------
            string toDisplay(string id) => context.Languaje != null && context.Languaje.TryGetValue(id, out var value) ? value : id;

            var path = Path.Combine(context.OutputFolderPath, toDisplay("Analyze"));

            Write("Iva Repercutido", path, facturasOutput, ivaRepercutido);
            Write("Iva Soportado", path, facturasInput, ivaSoportado);
        }

        //=====================================================================
        static void Write(string name, string outputPath, Factura[] factura, IVA[] ivas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# " + name);
            sb.AppendLine();

            var byTrimestre = SegregateByTrimestre(factura);

            foreach (var (trimestre, items) in byTrimestre)
            {
                sb.AppendLine("### Trimestre " + trimestre);
                sb.AppendLine();

                var byIva = SegregateByIva(items);

                sb.AppendLine("| % | BASE | CUOTA IVA | TOTAL |");
                sb.AppendLine("|---|---|---|---|");

                double sumBase = 0.0;
                double sumCuota = 0.0;
                double sumTotal = 0.0;

                foreach (var (cuota, items2) in byIva.OrderBy(x => x.Key))
                {
                    var totalBase = items2.Sum(f => f.BaseImponible);
                    var totalIva = items2.Sum(f => f.CuotaIva);
                    var total = totalBase + totalIva;

                    sumBase += totalBase;
                    sumCuota += totalIva;
                    sumTotal += total;
                    sb.AppendLine($"| {cuota} | {totalBase:N2} | {totalIva:N2} | {total:N2} |");
                }

                sb.AppendLine($"| | {sumBase:N2} | {sumCuota:N2} | {sumTotal:N2} |");
                sb.AppendLine();

                var iva = ivas.FirstOrDefault(x => x.Trimestre == trimestre);

                if (Math.Abs(iva.Importe - sumCuota) > 0.001)
                {
                    sb.AppendLine($"Mismatch IVA Presentado = {iva.Importe}");
                    sb.AppendLine();
                }
            }

            var path = System.IO.Path.Combine(outputPath, name + ".md");
            System.IO.File.WriteAllText(path, sb.ToString());
        }

        //=====================================================================
        static Dictionary<int, Factura[]> SegregateByIva(Factura[] all)
        {
            return all.GroupBy(f => f.Iva).ToArray().ToDictionary(g => g.Key, g => g.ToArray());
        }

        //=====================================================================
        static Dictionary<int, Factura[]> SegregateByTrimestre(Factura[] all)
        {
            var result = new Dictionary<int, Factura[]>();

            for (int trimestre = 1; trimestre <= 4; trimestre++)
            {
                var start = new DateTime(Constant.YEAR, (trimestre - 1) * 3 + 1, 1);
                var end = start.AddMonths(3).AddDays(-1);
                result.Add(trimestre, GetByDateTime(all, start, end));
            }

            return result;
        }
        
        //=====================================================================
        static Factura[] GetByDateTime(Factura[] all, DateTime start, DateTime end)
        {
            return all.Where(f =>
            {
                var date = f.DateTime.Date;
                return date >= start && date <= end;
            }).ToArray();
        }
    }
}
