using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Apertura
{
    public class AperturaComponent : ComponentRowCsv<Apertura>
    {
        //=====================================================================
        public AperturaComponent()
        {
            Initialize();

            AddConverter(ToRow);
        }

        //=====================================================================
        static Row ToRow(Apertura apertura)
        {
            return new Row()
            {
                DateTime = DateTime.MinValue,
                Code = apertura.Code,
                Concepto = $"APERTURA",
                Debit = apertura.Debe,
                Credit = apertura.Haber,
            };
        }
    }

    public class Apertura
    {
        public long Code { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }
    }
}
