using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Gastos
{
    public class GastosComponent : ComponentRowCsv<Gasto>
    {
        //=====================================================================
        public GastosComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To600Row);
        }

        //=====================================================================
        static Row To4XXRow(Gasto gasto, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(gasto.Concepto, out var code))
                throw new Exception($"No se ha encontrado el código para el concepto {gasto.Concepto} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = gasto.DateTime,
                Code = code.Id,
                Concepto = $"GASTO {gasto.Concepto}",
                Debit = 0.0,
                Credit = gasto.Importe,
            };
        }

        //=====================================================================
        static Row To600Row(Gasto gasto)
        {
            return new Row
            {
                DateTime = gasto.DateTime,
                Code = gasto.Code,
                Concepto = $"GASTO {gasto.Concepto}",
                Debit = gasto.Importe,
                Credit = 0.0,
            };
        }
    }

    public class Gasto
    {
        public DateTime DateTime { get; set; }
        public long Code { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }
    }
}
