using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Nominas
{
    public class NominaComponent : ComponentRowCsv<Nomina>
    {
        //=====================================================================
        public NominaComponent()
        {
            Initialize();

            AddConverter(To465Row);
            AddConverter(To4751_1Row);
            AddConverter(To4751_4Row);
            AddConverter(To476Row);
            AddConverter(To640Row);
        }

        //=====================================================================
        //Remuneraciones pendientes de pago
        static Row To465Row(Nomina nomina)
        {
            return new Row()
            {
                DateTime = nomina.DateTime,
                Code = 4650000001,
                Concepto = $"NOMINA {nomina.Description}",
                Debit = 0.0,
                Credit = nomina.GetLiquidoAPercibir(),
            };
        }

        //=====================================================================
        //Retenciones trabajadores
        static Row To4751_1Row(Nomina nomina)
        {
            return new Row()
            {
                DateTime = nomina.DateTime,
                Code = 4751000001,
                Concepto = $"NOMINA {nomina.Description}",
                Debit = 0.0,
                Credit = nomina.GetRetencionSalario(),
            };
        }

        //=====================================================================
        //RETENCIONES REMUNERACIONES ESPECIES
        static Row To4751_4Row(Nomina nomina)
        {
            return new Row()
            {
                DateTime = nomina.DateTime,
                Code = 4751000004,
                Concepto = $"NOMINA {nomina.Description}",
                Debit = 0.0,
                Credit = nomina.GetRetencionSeguridadSocial(),
            };
        }

        //=====================================================================
        //Seguridad Social Acreadora
        static Row To476Row(Nomina nomina)
        {
            return new Row()
            {
                DateTime = nomina.DateTime,
                Code = 4760000001,
                Concepto = $"NOMINA {nomina.Description}",
                Debit = 0.0,
                Credit = nomina.SeguridadSocial,
            };
        }

        //=====================================================================
        static Row To640Row(Nomina nomina)
        {
            return new Row()
            {
                DateTime = nomina.DateTime,
                Code = 6400000001,
                Concepto = $"NOMINA {nomina.Description}",
                Debit = nomina.GetTotalDevengado(),
                Credit = 0.0,
            };
        }
    }

    public class Nomina
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public double Salario { get; set; }
        public double SeguridadSocial { get; set; }
        public double Retencion { get; set; }

        public double GetBaseIrpf() => Salario + SeguridadSocial;
        public double GetRetencionSalario() => Math.Round(Salario * Retencion, 2);
        public double GetRetencionSeguridadSocial() => Math.Round(SeguridadSocial * Retencion, 2);
        public double GetTotalDevengado() => GetBaseIrpf() + GetRetencionSeguridadSocial();
        public double GetTotalADeducir() => GetRetencionSalario() + SeguridadSocial + GetRetencionSeguridadSocial();
        public double GetLiquidoAPercibir() => GetTotalDevengado() - GetTotalADeducir();
    }
}
