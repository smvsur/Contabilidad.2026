using Contabilidad.Data.Cierre;
using DEB;
using System.Globalization;
using System.Text;

namespace Contabilidad.Analyze
{
    public class NextApertura : IComponentAnalyze
    {
        //=====================================================================
        public void Process(ProcessContext context)
        {
            //-----------------------------------------------------------------
            string toDisplay(string id) => context.Languaje != null && context.Languaje.TryGetValue(id, out var value) ? value : id;

            //-----------------------------------------------------------------
            static string ToString(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);

            var cierre = context.Components
                .FirstOrDefault(x => x is CierreComponent) as CierreComponent;

            if (cierre == null)
                return;

            var models = cierre.GetModels();

            var sb = new StringBuilder();
            sb.AppendLine($"Code;Debe;Haber;");

            foreach (var m in models)
            {
                if (m.Code >= 6000000000)
                    continue;
                sb.AppendLine($"{m.Code};{ToString(m.Haber)};{ToString(m.Debe)};");
            }

            var analyzePath = Path.Combine(context.OutputFolderPath, toDisplay("Analyze"));
            if (!Directory.Exists(analyzePath))
                Directory.CreateDirectory(analyzePath);
            var filePath = Path.Combine(analyzePath, "NextApertura.csv");

            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
    }
}
