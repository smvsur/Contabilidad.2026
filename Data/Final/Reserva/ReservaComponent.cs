using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Final.Reserva
{
    public class ReservaComponent : ComponentRowCsv<Reserva>
    {
        //=====================================================================
        public ReservaComponent()
        {
            Initialize();

            AddConverter(To113);
            AddConverter(To129);
        }

        //=====================================================================
        static Row To113(Reserva cierre)
        {
            return new Row()
            {
                DateTime = GetDateTime(),
                Code = 1130000000,
                Concepto = $"RESERVA",
                Debit = 0.0,
                Credit = cierre.Importe,
            };
        }

        //=====================================================================
        static Row? To129(Reserva cierre, CodeContainer container)
        {
            var code = 1130000000;
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

    public class Reserva
    {
        public double Importe { get; set; }
    }
}
