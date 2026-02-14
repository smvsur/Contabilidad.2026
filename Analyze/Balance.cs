using Contabilidad.Data.Cierre;
using DEB;
using System.Globalization;
using System.Text;

namespace Contabilidad.Analyze
{
    public class Balance : IComponentAnalyze
    {
        //=====================================================================
        public void Process(ProcessContext context)
        {
            //-----------------------------------------------------------------
            string toDisplay(string id) => context.Languaje != null && context.Languaje.TryGetValue(id, out var value) ? value : id;

            var cierre = context.Components
                .FirstOrDefault(x => x is CierreComponent) as CierreComponent;

            if (cierre == null)
                return;

            var cierres = cierre.GetModels();

            var activo = new Activo(cierres);
            var patrimonioNetoYPasivo = new PatrimonioNetoYPasivo(cierres);

            // Generar el balance
            var sb = new StringBuilder();
            sb.AppendLine($"# BALANCE DE SITUACIÓN - AÑO {Constant.YEAR}");
            sb.AppendLine();

            var activoNoCorriente = activo.ActivoNoCorriente.SelectMany(x => x.Value).CalculateSaldo();
            var activoCorriente = activo.ActivoCorriente.SelectMany(x => x.Value).CalculateSaldo();
            var patrimonioNeto = patrimonioNetoYPasivo.PatrimonioNeto.SelectMany(x => x.Value).CalculateSaldo() * -1;
            var pasivoNoCorriente = patrimonioNetoYPasivo.PasivoNoCorriente.SelectMany(x => x.Value).CalculateSaldo() * -1;
            var pasivoCorriente = patrimonioNetoYPasivo.PasivoCorriente.SelectMany(x => x.Value).CalculateSaldo() * -1;

            // ACTIVO
            sb.AppendLine($"## ACTIVO : {(activoNoCorriente + activoCorriente).ToString("0.00")}");
            sb.AppendLine();

            sb.AppendLine($"### ACTIVO NO CORRIENTE : {activoNoCorriente.ToString("0.00")}");
            sb.AppendLine();
            foreach (var group in activo.ActivoNoCorriente)
                AppendGroup(sb, context, group.Value, " " + group.Key, false);

            sb.AppendLine($"### ACTIVO CORRIENTE : {activoCorriente.ToString("0.00")}");
            sb.AppendLine();
            foreach (var group in activo.ActivoCorriente)
                AppendGroup(sb, context, group.Value, " " + group.Key, false);

            // ACTIVO
            sb.AppendLine($"## PATRIMONIO NETO Y PASIVO : {(patrimonioNeto + pasivoNoCorriente + pasivoCorriente).ToString("0.00")}");
            sb.AppendLine();

            sb.AppendLine($"### PATRIMONIO NETO: {patrimonioNeto.ToString("0.00")}");
            sb.AppendLine();
            foreach (var group in patrimonioNetoYPasivo.PatrimonioNeto)
                AppendGroup(sb, context, group.Value, " " + group.Key, true);

            sb.AppendLine($"### PASIVO NO CORRIENTE: {pasivoNoCorriente.ToString("0.00")}");
            sb.AppendLine();
            foreach (var group in patrimonioNetoYPasivo.PasivoNoCorriente)
                AppendGroup(sb, context, group.Value, " " + group.Key, true);

            sb.AppendLine($"### PASIVO CORRIENTE: {pasivoCorriente.ToString("0.00")}");
            sb.AppendLine();
            foreach (var group in patrimonioNetoYPasivo.PasivoCorriente)
                AppendGroup(sb, context, group.Value, " " + group.Key, true);

            var analyzePath = Path.Combine(context.OutputFolderPath, toDisplay("Analyze"));
            if (!Directory.Exists(analyzePath))
                Directory.CreateDirectory(analyzePath);
            var filePath = Path.Combine(analyzePath, "Balance.md");

            System.IO.File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        //=====================================================================
        static void AppendGroup(StringBuilder sb, ProcessContext context, Cierre[] items, string displayName, bool invertSign)
        {
            sb.AppendLine($"#### {displayName} : {ToString(items.CalculateSaldo() * (invertSign ? -1 : 1)), 20}");

            sb.AppendLine();

            if (items.Any())
            {
                sb.AppendLine("| Cuenta | Saldo |");
                sb.AppendLine("| :--- | :---: |");

                foreach (var cuenta in items)
                    sb.AppendLine($"| {cuenta.Code} {context.CodeContainer.GetDescription(cuenta.Code)} | {ToString((cuenta.Haber - cuenta.Debe) * (invertSign ? -1 : 1))} |");
            }

            sb.AppendLine();
        }

        //=====================================================================
        static string ToString(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);

        //=====================================================================
        class Activo(Cierre[] models)
        {
            public Dictionary<string, Cierre[]> ActivoNoCorriente { get; } = new Dictionary<string, Cierre[]>
            {
                { "I. Inmovilizado intangible", models.FilterByCodes(20) },
                { "II. Inmovilizado material", models.FilterByCodes(21, 28) },
                { "III. Inversiones inmobiliarias", models.FilterByCodes(220, 229) },
                { "IV. Inversiones en empresas del grupo y asociadas a largo plazo", new Cierre[0] },
                { "V. Inversiones financieras a largo plazo", new Cierre[0] },
                { "VI. Activos por impuestos diferidos", models.FilterByCodes(474, 4745) }
            };

            public Dictionary<string, Cierre[]> ActivoCorriente { get; } = new Dictionary<string, Cierre[]>
            {
                { "I. Existencias", models.FilterByRange(300, 399) },
                { "III. Deudores comerciales y otras cuentas a cobrar", models.FilterByCodes(43, 44, 5531, 5533, 460, 544, 4709, 4700, 4708, 471, 472, 5580) },
                { "VI. Inversiones financieras a corto plazo", new Cierre[0] },
                { "VII. Efectivo y otros activos líquidos equivalentes", models.FilterByCodes(57)  },
            };
        }

        //=====================================================================
        class PatrimonioNetoYPasivo(Cierre[] models)
        {
            public Dictionary<string, Cierre[]> PatrimonioNeto { get; } = new Dictionary<string, Cierre[]>()
            {
                { "I. Capital", models.FilterByCodes(10) },
                { "II. Prima de emisión", models.FilterByCodes(110) },
                { "III. Reservas", models.FilterByCodes(112, 113, 114, 119) },
                { "V. Resultados ejercicios anteriores", models.FilterByCodes(120, 121) },
                { "VI. Otras aportaciones de socios", models.FilterByCodes(118) },
                { "VII. Resultado del ejercicio", models.FilterByCodes(129) },
            };

            public Dictionary<string, Cierre[]> PasivoNoCorriente { get; } = new Dictionary<string, Cierre[]>()
            {
                { "I. Provisiones a largo plazo", models.FilterByCodes(14) },
                { "II. Deudas a largo plazo", models.FilterByCodes(17) },
                { "IV. Pasivos por impuestos diferidos", models.FilterByCodes(479) }
            };

            public Dictionary<string, Cierre[]> PasivoCorriente { get; } = new Dictionary<string, Cierre[]>()
            {
                { "II. Provisiones a corto plazo", models.FilterByRange(499, 529) },
                { "III. Deudas a corto plazo", models.FilterByCodes(500, 501, 505, 506, 5105, 520, 526) },
                { "V. Acreedores comerciales y otras cuentas a pagar", models.FilterByCodes(40, 41, 465, 466, 4752, 4750, 4751, 4753, 476, 477, 478, 438, 473) }
            };
        }
    }

    //=====================================================================
    static class Extensions
    {
        //=====================================================================
        public static Cierre[] FilterByRange(this Cierre[] cierres, long minCode, long maxCode)
        {
            minCode = RegularizeCode(minCode);
            maxCode = RegularizeCode(maxCode);
            return cierres.Where(c => c.Code >= minCode && c.Code <= maxCode).ToArray();
        }

        //=====================================================================
        public static Cierre[] FilterByCodes(this Cierre[] cierres, params long[] codes)
            => cierres.Where(m => IsContained(codes, m.Code)).ToArray();

        //=====================================================================
        public static Cierre[] FilterBySaldo(this Cierre[] cierres, Func<double, bool> predicate)
            => cierres.Where(c => predicate(c.Haber - c.Debe)).ToArray();

        //=====================================================================
        public static double CalculateSaldo(this IEnumerable<Cierre> cierres)
            => cierres.Sum(c => c.Haber - c.Debe);

        //=====================================================================
        static bool IsContained(long[] containers, long code)
        {
            foreach (var container in containers)
            {
                if (IsContained(container, code))
                    return true;
            }
            return false;
        }

        //=====================================================================
        static bool IsContained(long containerCode, long code)
        {
            var lowLimit = RegularizeCode(containerCode);
            var highLimit = RegularizeCode(containerCode + 1);
            code = RegularizeCode(code);
            return code >= lowLimit && code < highLimit;
        }

        //=====================================================================
        static long RegularizeCode(long code)
        {
            while (code < 1000000000)
            {
                code *= 10;
            }
            return code;
        }
    }
}
