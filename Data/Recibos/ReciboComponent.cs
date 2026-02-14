using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Recibos
{
    public class ReciboComponent : ComponentRowCsv<Recibo>
    {
        //=====================================================================
        public ReciboComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To600Row);
        }

        //=====================================================================
        static Row To4XXRow(Recibo recibo, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(recibo.Concepto, out var code))
                throw new Exception($"No se ha encontrado el código para el concepto {recibo.Concepto} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = recibo.DateTime,
                Code = code.Id,
                Concepto = $"RECIBO {recibo.Concepto}",
                Debit = 0.0,
                Credit = recibo.Importe,
            };
        }

        //=====================================================================
        static Row To600Row(Recibo recibo)
        {
            return new Row
            {
                DateTime = recibo.DateTime,
                Code = recibo.Code,
                Concepto = $"RECIBO {recibo.Concepto}",
                Debit = recibo.Importe,
                Credit = 0.0,
            };
        }
    }

    public class Recibo
    {
        public DateTime DateTime { get; set; }
        public long Code { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }
    }
}
