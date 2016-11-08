using System.Collections;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class RangeMovedOperator : ReferenceOperator
    {
        private readonly SheetReference _dest;
        private readonly FormulaEngine _owner;
        private readonly SheetReference _source;

        public RangeMovedOperator(FormulaEngine owner, SheetReference source, SheetReference dest)
        {
            _owner = owner;
            _source = source;
            _dest = dest;
        }

        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return @ref.GridOps.OnRangeMoved(_source, _dest);
        }

        public override void PreOperate(IList references)
        {
            _owner.DependencyManager.RemoveRangeLinks();
        }

        public override void PostOperate(IList references)
        {
            _owner.DependencyManager.AddRangeLinks();
            IList circularRefs = new ArrayList();

            foreach (Reference @ref in references)
            {
                if (_owner.DependencyManager.IsCircularReference(@ref))
                {
                    circularRefs.Add(@ref);
                }
            }

            if (circularRefs.Count > 0)
            {
                _owner.OnCircularReferenceDetected(circularRefs);
            }
        }
    }
}