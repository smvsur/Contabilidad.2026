using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Amortizaciones
{
    public class AmortizacionesComponent : ComponentRowCsv<Amortizacion>
    {
        //=====================================================================
        public AmortizacionesComponent()
        {
            Initialize();

            AddConverter(To281Row);
            AddConverter(To681Row);
        }

        //=====================================================================
        static Row To281Row(Amortizacion amortizacion, CodeContainer codeContainer)
        {
            var code = 2810000000 + amortizacion.Index;
            return new Row()
            {
                DateTime = GetDateTime(),
                Code = code,
                Concepto = $"AMORTIZACION {amortizacion.Notes}",
                Debit = 0.0,
                Credit = amortizacion.Importe,
            };
        }

        //=====================================================================
        static Row To681Row(Amortizacion amortizacion, CodeContainer codeContainer)
        {
            var code = 6810000000;
            
            return new Row()
            {
                DateTime = GetDateTime(),
                Code = code,
                Concepto = $"AMORTIZACION {amortizacion.Notes}",
                Debit = amortizacion.Importe,
                Credit = 0.0,
            };
        }

        //=====================================================================
        static DateTime GetDateTime() => new DateTime(Constant.YEAR, 1, 1).AddYears(1) - TimeSpan.FromMilliseconds(10);
    }

    public class Amortizacion
    {
        public long Index { get; set; }
        public double Importe { get; set; }
        public string Notes { get; set; }
    }
}
