using DEB;
using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.GastosPersonal
{
    public class GastosPersonalComponent : ComponentRowCsv<GastoPersonales>
    {
        //=====================================================================
        public GastosPersonalComponent()
        {
            Initialize();

            AddConverter(To4XXRow);
            AddConverter(To600Row);
        }

        //=====================================================================
        static Row To4XXRow(GastoPersonales gasto, CodeContainer codesContainer)
        {
            if (!codesContainer.TryGetCode(gasto.Concepto, out var code))
                throw new Exception($"No se ha encontrado el código para el concepto {gasto.Concepto} en la lista de códigos 4XX.");

            return new Row
            {
                DateTime = gasto.DateTime,
                Code = code.Id,
                Concepto = $"GASTO {gasto.Concepto}",
                Debit = 0.0,
                Credit = Math.Round(gasto.Importe * gasto.Conversion, 2),
            };
        }

        //=====================================================================
        static Row To600Row(GastoPersonales gasto)
        {
            return new Row
            {
                DateTime = gasto.DateTime,
                Code = gasto.Code,
                Concepto = $"GASTO {gasto.Concepto}",
                Debit = Math.Round(gasto.Importe * gasto.Conversion, 2),
                Credit = 0.0,
            };
        }
    }

    //=====================================================================
    public class GastoPersonales : Gastos.Gasto
    {
        public double Conversion { get; set; }
    }
}
