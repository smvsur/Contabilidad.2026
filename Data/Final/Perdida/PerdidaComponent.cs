using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Final.Perdida
{
    public class PerdidaComponent : ComponentRowCsv<Perdida>
    {
        //=====================================================================
        public PerdidaComponent()
        {
            Initialize();

            AddConverter(To121);
            AddConverter(To129);
        }

        //=====================================================================
        static Row To121(Perdida cierre)
        {
            return new Row()
            {
                DateTime = GetDateTime(),
                Code = 1210000000,
                Concepto = $"PERDIDA",
                Debit = cierre.Importe,
                Credit = 0.0,
            };
        }

        //=====================================================================
        static Row? To129(Perdida cierre, CodeContainer container)
        {
            var code = 1210000000;
            var description = $"{code} {container.GetDescription(code)}";

            return new Row()
            {
                DateTime = GetDateTime(),
                Code = 1290000000,
                Concepto = description,
                Debit = 0.0,
                Credit = cierre.Importe,
            };
        }

        //=====================================================================
        static DateTime GetDateTime() => new DateTime(Constant.YEAR, 1, 1).AddYears(1) - TimeSpan.FromMilliseconds(1);
    }

    public class Perdida
    {
        public double Importe { get; set; }
    }
}
