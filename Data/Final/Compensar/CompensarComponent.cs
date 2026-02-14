using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Final.Perdida
{
    public class CompensarComponent : ComponentRowCsv<Perdida>
    {
        //=====================================================================
        public CompensarComponent()
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
                Concepto = $"COMPENSAR",
                Debit = 0.0,
                Credit = cierre.Importe,

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
                Debit = cierre.Importe,
                Credit = 0.0,
            };
        }

        //=====================================================================
        static DateTime GetDateTime() => new DateTime(Constant.YEAR, 1, 1).AddYears(1) - TimeSpan.FromMilliseconds(1);
    }

    public class Compensar
    {
        public double Importe { get; set; }
    }
}