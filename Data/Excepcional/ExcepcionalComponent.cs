using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Ingresos
{
    public class ExcepcionalComponent : ComponentRowCsv<Ingreso>
    {
        //=====================================================================
        public ExcepcionalComponent()
        {
            Initialize();

            AddConverter(ToRow400);
            AddConverter(To700Row);
        }

        //=====================================================================
        static Row ToRow400(Ingreso ingreso, CodeContainer codeContainer)
        {
            var description = codeContainer.GetDescription(ingreso.Code400);
            var importe = ingreso.Importe;
            var type = importe < 0 ? "GASTO" : "INGRESO";

            return new Row()
            {
                DateTime = ingreso.DateTime,
                Code = ingreso.Code400,
                Concepto = $"{type} EXCEPCIONAL {description} {ingreso.Notes}",
                Debit = importe > 0 ? Math.Abs(importe) : 0,
                Credit = importe > 0 ? 0 : Math.Abs(importe),
            };
        }

        //=====================================================================
        static Row To700Row(Ingreso ingreso, CodeContainer codeContainer)
        {
            var description = codeContainer.GetDescription(ingreso.Code400);
            var importe = ingreso.Importe;
            var type = importe < 0 ? "GASTO" : "INGRESO";

            return new Row
            {
                DateTime = ingreso.DateTime,
                Code = importe > 0 ? 7680000000 : 6780000000,
                Concepto = $"{type} {description} {ingreso.Notes}",
                Debit = importe < 0 ? Math.Abs(importe) : 0,
                Credit = importe < 0 ? 0 : Math.Abs(importe),
            };
        }
    }

    public class Ingreso
    {
        public DateTime DateTime { get; set; }
        public long Code400 { get; set; }
        public double Importe { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
