using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    /// <summary>
    ///     Represents a reference to a value or set of values
    /// </summary>
    /// <remarks>
    ///     This is the base interface for all references.  A reference is simply a pointer to a value or formula.
    ///     All formulas that are managed by the formula engine must be associated with a reference.
    ///     You can change the context of a formula by binding it to different types of references.
    /// </remarks>
    public interface IReference
    {
        /// <summary>Gets the values that the reference points to</summary>
        /// <param name="processor">A class responsible from processing the reference's values</param>
        /// <remarks>
        ///     You should call this method when you need reference's values.  The reference will pass each of its values to the
        ///     processor and it is up to that processor to store or use them to compute a result.
        /// </remarks>
        void GetReferenceValues(IReferenceValueProcessor processor);

        /// <summary>Determines whether this reference equals another</summary>
        /// <param name="ref">The reference to test against</param>
        /// <returns>True is the current reference is equal to ref.  False otherwise</returns>
        /// <remarks>This method exists mostly for testing purposes.</remarks>
        bool Equals(IReference @ref);

        /// <summary>Returns a formatted representation of the reference</summary>
        /// <remarks>This method allows you to get a string representation of the reference</remarks>
        string ToString();
    }
}