using System;
using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Contains information about the cause of circular references
    /// </summary>
    public class CircularReferenceDetectedEventArgs : EventArgs
    {
        private readonly IReference[] _roots;

        internal CircularReferenceDetectedEventArgs(IReference[] roots)
        {
            _roots = roots;
        }

        /// <summary>
        ///     The root of each circular reference
        /// </summary>
        /// <value>An array of the circular reference roots</value>
        /// <remarks>
        ///     Each reference in this array is the root of a circular reference.  That is, by starting at the reference and
        ///     following all its dependents, you will eventually wind up at the same reference.
        /// </remarks>
        public IReference[] Roots
        {
            get { return _roots; }
        }
    }
}