namespace Contabilidad
{
    public class Constant
    {
        public const int YEAR = 2025;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string getOuptputPath([System.Runtime.CompilerServices.CallerFilePath] string filePath = "")
            {
                return Path.Combine(filePath, "..", "Resultado");
            }

            DEB.Processor.Process(typeof(Program).Assembly, getOuptputPath(), new Dictionary<string, string>()
            {
                { "Date", "Fecha"},
                { "Reference", "Referencia"},
                { "Debit", "Debe"},
                { "Credit", "Haber" },
                { "Concept", "Concepto" },
                { "Balance", "Saldo" },
                { "Sums", "Sumas" },
                { "Diference", "Diferencia" },
            });

            Console.WriteLine("Proceso finalizado.");
        }

        public class MismatchRowGenerator : DEB.CSV.MismatchAnalyzeCSV { }
        public class Writer : DEB.CSV.WriteTableCsv { }
    }
}