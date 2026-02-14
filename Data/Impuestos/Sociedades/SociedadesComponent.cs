using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Impuestos.Sociedades
{
    internal class SociedadesComponent : ComponentRowCsv<Sociedades>
    {
        //=====================================================================
        public SociedadesComponent()
        {
            Initialize();

            AddConverter(To400);
            AddConverter(To600);
        }

        //=====================================================================
        static Row To400(Sociedades model)
        {
            return new Row
            {
                DateTime = GetDateTime(),
                Code = 4752000000,
                Concepto = "Impuesto Sociedades",
                Debit = 0.0,
                Credit = model.Cantidad,
            };
        }

        //=====================================================================
        static Row To600(Sociedades model)
        {
            return new Row
            {
                DateTime = GetDateTime(),
                Code = 6300000000,
                Concepto = "Impuesto Sociedades",
                Debit = model.Cantidad,
                Credit = 0.0,
            };
        }

        //=====================================================================
        static DateTime GetDateTime() => new DateTime(Constant.YEAR + 1, 1, 1) - TimeSpan.FromSeconds(1);
    }

    public class Sociedades
    {
        public double Cantidad { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
