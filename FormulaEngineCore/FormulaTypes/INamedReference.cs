using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Represents a named reference
    /// </summary>
    /// <remarks>
    ///     Named references allow you to associate a formula with a name.  By binding a formula to a named reference,
    ///     you make it possible to use that formula's result in other formulas by simply typing the name.  This can make
    ///     formulas cleaner and less complex since you can
    ///     reuse a particular result in many formulas rather than duplicating the same expression in each one.  The formula
    ///     engine will recalculate
    ///     all formulas that depend on a name when the value of the formula bound to the name changes.
    /// </remarks>
    /// <example>
    ///     This example shows how you can define a constant and use it in another formula:
    ///     <code>
    /// Dim engine As New FormulaEngine
    /// ' Add a constant named InterestRate with a value of 0.15
    /// engine.AddFormula("=0.15", engine.ReferenceFactory.Named("InterestRate"))
    /// ' Use the constant in a formula
    /// Dim result As Object = engine.Evaluate("=1000 * InterestRate")
    /// </code>
    /// </example>
    public interface INamedReference : IReference
    {
        /// <summary>
        ///     Gets the name of the reference
        /// </summary>
        /// <value>The name of the reference</value>
        /// <remarks>This property lets you obtain the name of the reference</remarks>
        string Name { get; }
    }
}