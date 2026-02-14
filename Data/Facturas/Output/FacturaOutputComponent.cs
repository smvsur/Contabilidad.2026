using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Facturas.Output
{
    public class FacturaOutputComponent : ComponentRowCsv<Factura>
    {
        //=====================================================================
        public FacturaOutputComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To700Row);
            AddConverter(To477Row);
        }

        //=====================================================================
        static Row To4XXRow(Factura factura, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(factura.ClientName, out var code))
                throw new Exception($"No se ha encontrado el código para el cliente {factura.ClientName} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = factura.DateTime,
                Code = code.Id,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = factura.BaseImponible + factura.CuotaIva,
                Credit = 0.0,
            };
        }

        //=====================================================================
        static Row To700Row(Factura factura)
        {
            return new Row
            {
                DateTime = factura.DateTime,
                Code = factura.Code,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = 0.0,
                Credit = factura.BaseImponible,
            };
        }

        //=====================================================================
        static Row To477Row(Factura factura)
        {
            return new Row
            {
                DateTime = factura.DateTime,
                Code = 4770000001,
                Concepto = $"FACTURA {factura.ClientName}",
                Reference = factura.Id,
                Debit = 0.0,
                Credit = factura.CuotaIva,
            };
        }
    }

    public class Factura
    {
        public DateTime DateTime { get; set; }
        public long Code { get; set; }
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string ClientNie { get; set; }
        public string ClientAddress { get; set; }
        public double BaseImponible { get; set; }
        public int Iva { get; set; }
        public double CuotaIva { get; set; }
        public string? PdfPath { get; set; }
    }
}
