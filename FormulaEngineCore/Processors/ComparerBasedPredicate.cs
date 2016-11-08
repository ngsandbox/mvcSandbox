using System.Collections;
using System.Globalization;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Processors
{
    internal class ComparerBasedPredicate : NonNullPredicate
    {
        private readonly IComparer _comparer;

        public ComparerBasedPredicate(object target, IComparer comparer)
            : base(target)
        {
            _comparer = comparer;
        }

        public static ComparerBasedPredicate CreateDefault(object target)
        {
            return new ComparerBasedPredicate(target, new Comparer(CultureInfo.CurrentCulture));
        }

        protected override int Compare(object value, object target)
        {
            return _comparer.Compare(value, target);
        }
    }
}