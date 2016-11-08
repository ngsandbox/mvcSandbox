namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Implemented by classes that process a reference's values
    /// </summary>
    /// <remarks>
    ///     An implementation of this interface can be passed to a reference's
    ///     <see cref="M:ciloci.FormulaEngine.IReference.GetReferenceValues(ciloci.FormulaEngine.IReferenceValueProcessor)" />
    ///     method.  The
    ///     reference will call the ProcessValue method on each of the values it represents.
    /// </remarks>
    public interface IReferenceValueProcessor
    {
        /// <summary>Processes a reference value</summary>
        /// <param name="value">The value from the reference that is to be processed</param>
        /// <returns>True to keep processing values; False to stop</returns>
        /// <remarks>
        ///     This method will be called once for each value that a reference represents.  Classes that implement this interface
        ///     must decide what to do with the value.
        /// </remarks>
        bool ProcessValue(object value);
    }
}