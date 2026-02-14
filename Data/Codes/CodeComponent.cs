using DEB.CSV;
using DEB.Entities;

namespace Contabilidad.Data.Codes
{
    public class CodeComponent : ComponentCodeCsv<CodeInput>
    {
        //=====================================================================
        public CodeComponent()
        {
            Initialize();
            AddConverter(ToCode);
        }

        //=====================================================================
        public static Code ToCode(CodeInput input)
            => new() { Id = input.Code, Description = input.Description, };
    }

    public class CodeInput
    {
        public long Code { get; set; }
        public string Description { get; set; }
    }
}
