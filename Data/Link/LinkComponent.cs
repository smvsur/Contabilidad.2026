using Contabilidad.Data.Extractos;
using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Link
{
    public class LinkComponent : ComponentRowCsv<Link>
    {
        Extracto[]? _extractos = null;

        //=====================================================================
        public LinkComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To572Row);
        }

        //=====================================================================
        Row To572Row(Link link,ProcessContext context)
        {
            var remesa = link.Remesa;
            var extracto = SearchExtracto(context, remesa);

            var importe = link.Importe;

            return new Row()
            {
                DateTime = link.DateTime,
                Code = 5720000001,
                Concepto = extracto?.Concepto + " -- " + extracto?.Observaciones,
                Reference = link.Remesa,
                Debit = importe > 0 ? link.Importe : 0.0,
                Credit = importe < 0 ? -link.Importe : 0.0,
            };
        }

        //=====================================================================
        static Row? To4XXRow(Link link, CodeContainer codeContainer)
        {
            var importe = link.Importe;

            return new Row
            {
                DateTime = link.DateTime,
                Code = link.Code,
                Concepto = $"PAGO {codeContainer.GetDescription(link.Code)}",
                Reference = link.Remesa,
                Debit = importe < 0 ? -link.Importe : 0.0,
                Credit = importe > 0 ? link.Importe : 0.0,
            };
        }

        //=====================================================================
        Extracto? SearchExtracto(ProcessContext context, string remesa)
        {
            if (_extractos == null)
            {
                var extractoComponent = context.Components
                    .OfType<ExtractoComponent>()
                    .FirstOrDefault();

                if (extractoComponent != null)
                {
                    extractoComponent.Process(context);
                    _extractos = extractoComponent.GetModels();
                }             
                else
                {
                    _extractos = Array.Empty<Extracto>();
                }                
            }

            return _extractos.FirstOrDefault(x => x.Remesa == remesa);
        }
    }

    public class Link
    {
        public DateTime DateTime { get; set; }
        public long Code { get; set; }
        public double Importe { get; set; }
        public string Remesa { get; set; }
    }
}
