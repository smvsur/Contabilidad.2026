using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.IVA
{
    public class IVAComponent : ComponentRowCsv<IVA>
    {
        //=====================================================================
        public IVAComponent()
        {
            Initialize();

            AddConverter(ToIVARow);
            AddConverter(ToHaciendaRow);
        }

        //=====================================================================
        static Row ToIVARow(IVA iva)
        {
            return new Row
            {
                DateTime = GetDateTime(iva.Trimestre),
                Code = iva.IsSoportado ? 4720000001 : 4770000001,
                Concepto = "IVA TRIMESTRE " + iva.Trimestre + " " + iva.Notes,
                Debit = iva.IsSoportado ? 0.0 : iva.Importe,
                Credit = iva.IsSoportado ? iva.Importe : 0.0,
            };
        }

        //=====================================================================
        static Row ToHaciendaRow(IVA iva)
        {
            return new Row
            {
                DateTime = GetDateTime(iva.Trimestre),
                Code = 4750000000,
                Concepto = "IVA TRIMESTRE " + iva.Trimestre + " " + iva.Notes,
                Debit = iva.IsSoportado ? iva.Importe : 0.0,
                Credit = iva.IsSoportado ? 0.0 : iva.Importe,
            };
        }

        //=====================================================================
        static DateTime GetDateTime(int trimestre) => new DateTime(Constant.YEAR, 1, 1).AddMonths(trimestre * 3) - TimeSpan.FromSeconds(1);
    }

    public class IVA
    {
        public bool IsSoportado { get; set; }
        public int Trimestre { get; set; }
        public double Importe { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
