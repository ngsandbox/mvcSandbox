using System.Drawing;
using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    internal class ReferenceParseInfo
    {
        public CharacterRange Location;
        public ReferenceParseProperties ParseProperties;
        public ReferenceProperties Properties;
        public Reference Target;
    }
}