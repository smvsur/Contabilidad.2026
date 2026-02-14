using Contabilidad.Data.Extractos;
using DEB;
using System.Globalization;
using System.Text;

namespace Contabilidad.Analyze
{
    class CheckExtractos : IComponentAnalyze
    {
        //=====================================================================
        public void Process(ProcessContext context)
        {
            //-----------------------------------------------------------------
            string toString(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);

            //-----------------------------------------------------------------
            string toDisplay(string id) => context.Languaje != null && context.Languaje.TryGetValue(id, out var value) ? value : id;

            var extractoComponent = context.Components
                .FirstOrDefault(x => x is ExtractoComponent) as ExtractoComponent;

            var bankTable = context.Tables.FirstOrDefault(x => x.Code == 5720000000);

            var extractos = extractoComponent?.GetModels().Reverse().OrderBy(x => x.FechaProceso).ToArray();
            var rows = bankTable?.Rows;

            if (extractos == null || rows == null)
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"Remesa;DateTime;ImporteExtracto;ImporteRow;Diferencia;");

            foreach (var extracto in extractos)
            {
                var rs = rows.Where(x => x.Reference == extracto.Remesa).ToArray();
                var rowsSum = rs.Sum(x => x.Debit - x.Credit);
                var diference = extracto.Importe - rowsSum;

                if (Math.Abs(diference) > 0.001)
                    sb.AppendLine($"{extracto.Remesa};{extracto.FechaProceso:yyyy-MM-dd};{toString(extracto.Importe)};{toString(rowsSum)};{toString(diference)};");
            }

            var analyzePath = Path.Combine(context.OutputFolderPath, toDisplay("Analyze"));
            if (!Directory.Exists(analyzePath))
                Directory.CreateDirectory(analyzePath);
            var filePath = Path.Combine(analyzePath, "BankErros.csv");

            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
    }
}
