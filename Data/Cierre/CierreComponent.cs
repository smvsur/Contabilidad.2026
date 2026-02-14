using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Cierre
{
    public class CierreComponent : ComponentRowCsv<Cierre>
    {
        //=====================================================================
        public CierreComponent()
        {
            Initialize();

            AddConverter(ToRow);
            AddConverter(To129Row);
        }

        //=====================================================================
        static Row ToRow(Cierre cierre)
        {
            return new Row()
            {
                DateTime = GetDateTime(),
                Code = cierre.Code,
                Concepto = $"CIERRE",
                Debit = cierre.Debe,
                Credit = cierre.Haber,
            };
        }

        //=====================================================================
        static Row? To129Row(Cierre cierre, CodeContainer container)
        {
            var code = 1290000000;

            if (!cierre.Code.ToString().StartsWith("6") && !cierre.Code.ToString().StartsWith("7"))
                return null;

            var description = $"{cierre.Code} {container.GetDescription(cierre.Code)}";

            return new Row()
            {
                DateTime = GetDateTime(),
                Code = code,
                Concepto = description,
                Debit = cierre.Haber,
                Credit = cierre.Debe,
            };
        }

        //=====================================================================
        static DateTime GetDateTime() => new DateTime(Constant.YEAR, 1, 1).AddYears(1) - TimeSpan.FromMilliseconds(2);
    }

    public class Cierre
    {
        public long Code { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }
    }
}
