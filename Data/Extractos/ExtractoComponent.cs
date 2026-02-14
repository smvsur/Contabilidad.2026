using CsvHelper.Configuration;
using DEB.CSV;
using System.Globalization;

namespace Contabilidad.Data.Extractos
{
    public class ExtractoComponent : ComponentRowCsv<Extracto>
    {
        //=====================================================================
        public ExtractoComponent()
        {
            Initialize();
        }

        //=====================================================================
        protected override string[] FileExtesions => new string[] { ".csv" };

        //=====================================================================
        protected override Extracto[] ReadModel(string path) => HelperCsv.Read<Extracto>(path, new CsvConfiguration(new CultureInfo("en-GB"))
        {
            Delimiter = ",",
        });
    }

    public class Extracto
    {
        public double Importe { get; set; }
        public string Remesa { get; set; }
        public DateTime FechaProceso { get; set; }
        public DateTime FechaValor { get; set; }
        public string Concepto { get; set; }
        public string Observaciones { get; set; }
        public double Saldo { get; set; }
    }
}
