using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Facturas.Input
{
    public class FacturaInputComponent : ComponentRowCsv<FacturaInput>
    {
        //=====================================================================
        public FacturaInputComponent()
        {
            Initialize();

            AddConverter(To600Row);
            AddConverter(To472Row);
            AddConverter(To4XXRow);
            AddConverter(To475Row);
        }

        //=====================================================================
        public static Row To4XXRow(FacturaInput factura, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(factura.ClientName, out var code))
                throw new Exception($"No se ha encontrado el código para el cliente {factura.ClientName} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = factura.DateTime,
                Code = code.Id,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = 0.0,
                Credit = factura.BaseImponible + factura.CuotaIva + factura.Suplidos - factura.Retencion,
            };
        }

        //=====================================================================
        public static Row To600Row(FacturaInput factura)
        {
            return new Row
            {
                DateTime = factura.DateTime,
                Code = factura.Code,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = Math.Round(factura.BaseImponible + factura.Suplidos, 2),
                Credit = 0.0,
            };
        }

        //=====================================================================
        public static Row To472Row(FacturaInput factura)
        {
            return new Row
            {
                DateTime = factura.DateTime,
                Code = 4720000001,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = Math.Round(factura.CuotaIva, 2),
                Credit = 0,
            };
        }

        //=====================================================================
        //Retenciones Profesionales
        public static Row? To475Row(FacturaInput factura)
        {
            var code = 4751000002;

            if (factura.Retencion == 0)
                return null;

            return new Row
            {
                DateTime = factura.DateTime,
                Code = 4751000002,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = 0.0,
                Credit = factura.Retencion,
            };
        }
    }

    public class FacturaInput : Output.Factura
    {
        public double Suplidos { get; set; }
        public double Retencion { get; set; }
    }
}
