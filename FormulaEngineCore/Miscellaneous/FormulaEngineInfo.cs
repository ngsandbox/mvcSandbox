using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Contains useful information about the formula engine
    /// </summary>
    /// <remarks>
    ///     This class allows you to get various information about the formula engine.  Mostly used for development/testing and
    ///     when
    ///     you would like to show some statistics to the user.
    /// </remarks>
    public class FormulaEngineInfo
    {
        private readonly FormulaEngine _owner;

        internal FormulaEngineInfo(FormulaEngine owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///     Gets the total number of references tracked by the engine
        /// </summary>
        /// <value>A count indicating the total number of references</value>
        /// <remarks>This property lets you get a count of the total number of references that the engine is tracking</remarks>
        public int ReferenceCount
        {
            get { return _owner.ReferencePool.ReferenceCount; }
        }

        /// <summary>
        ///     Gets the number of dependents in the engine's dependency graph
        /// </summary>
        /// <value>A count of the number of dependents</value>
        /// <remarks>The count indicates the number of references that have other references which depend on them</remarks>
        public int DependentsCount
        {
            get { return _owner.DependencyManager.DependentsCount; }
        }

        /// <summary>
        ///     Gets the number of precedents in the engine's dependency graph
        /// </summary>
        /// <value>A count of the number of precedents</value>
        /// <remarks>The count indicates the number of references that are dependents of other references</remarks>
        public int PrecedentsCount
        {
            get { return _owner.DependencyManager.PrecedentsCount; }
        }

        /// <summary>
        ///     Gets a string representation of the engine's dependency graph
        /// </summary>
        /// <value>A string representing all dependencies</value>
        /// <remarks>
        ///     This property will return a string representation of the engine's dependents graph.  There will be one
        ///     line for each dependency and each line will be of the form "Ref1 -> Ref2, Ref3" and reads that a change in Ref1
        ///     will
        ///     change Ref2 and Ref3.
        /// </remarks>
        public string DependencyDump
        {
            get { return _owner.DependencyManager.DependencyDump; }
        }

        /// <summary>
        ///     Gets the calculation list for a reference
        /// </summary>
        /// <param name="root">The reference from where to calculate the list</param>
        /// <returns>An array of formulas that would need to be recalculated</returns>
        /// <remarks>
        ///     Given a reference, this method returns a list of formulas that would need to be recalculated when that reference
        ///     changes.
        ///     The formulas in the list will be in natural order.
        /// </remarks>
        public Formula[] GetCalculationList(IReference root)
        {
            FormulaEngine.ValidateNonNull(root, "root");
            Reference[] refs = _owner.DependencyManager.GetReferenceCalculationList((Reference)root);
            return _owner.GetFormulasFromReferences(refs);
        }

        /// <summary>
        ///     Determines whether a reference is valid
        /// </summary>
        /// <param name="ref">The reference to check</param>
        /// <returns>True if the reference is valid; False if it is not</returns>
        /// <remarks>
        ///     This function determines whether a given reference is valid.  For example: A reference to column C will become
        ///     invalid when column C is removed.
        /// </remarks>
        public bool IsReferenceValid(IReference @ref)
        {
            Reference realRef = _owner.ReferencePool.GetPooledReference((Reference)@ref);
            if (realRef == null)
            {
                return false;
            }
            return realRef.Valid;
        }

        /// <summary>
        ///     Gets the number of direct precedents of a reference
        /// </summary>
        /// <param name="ref">The reference whose direct precedents to get</param>
        /// <returns>A count of the number of direct precedents of the reference</returns>
        /// <remarks>The count indicates the number of references that have ref as their dependant</remarks>
        public int GetDirectPrecedentsCount(IReference @ref)
        {
            FormulaEngine.ValidateNonNull(@ref, "ref");
            Reference realRef = _owner.ReferencePool.GetPooledReference((Reference)@ref);
            if (realRef == null)
            {
                return 0;
            }
            return _owner.DependencyManager.GetDirectPrecedentsCount(realRef);
        }
    }
}