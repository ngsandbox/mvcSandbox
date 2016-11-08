using System.Globalization;

namespace FormulaEngineCore
{
    public class FormulaParserSettings
    {
        public FormulaParserSettings()
        {
            ListSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

        public string ListSeparator { get; set; }
        public string DecimalSeparator { get; set; }
    }
}
