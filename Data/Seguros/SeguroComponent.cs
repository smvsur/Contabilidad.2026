using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Seguros
{
    public class SeguroComponent : ComponentRowCsv<Seguro>
    {
        //=====================================================================
        public SeguroComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To600Row);
        }

        //=====================================================================
        static Row To4XXRow(Seguro seguro, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(seguro.Concepto, out var code))
                throw new Exception($"No se ha encontrado el código para el concepto {seguro.Concepto} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = seguro.DateTime,
                Code = code.Id,
                Concepto = $"SEGURO {seguro.Concepto}",
                Debit = 0.0,
                Credit = seguro.Importe,
            };
        }

        //=====================================================================
        static Row To600Row(Seguro gasto)
        {
            return new Row
            {
                DateTime = gasto.DateTime,
                Code = 6250000001,
                Concepto = $"SEGURO {gasto.Concepto}",
                Debit = gasto.Importe,
                Credit = 0.0,
            };
        }
    }

    public class Seguro
    {
        public DateTime DateTime { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }
    }
}
