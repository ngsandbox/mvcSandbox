using System;
using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Represents a reference outside of a sheet
    /// </summary>
    /// <remarks>
    ///     External references are similar to named references in that you can use both without any sheets.  A limitation of
    ///     named
    ///     references is that each name must be unique.  This can make it difficult to define many non-sheet formulas because
    ///     you have to
    ///     generate a unique name for each one.  External references do not have this limitation and you can create as many as
    ///     you need.
    ///     Their only drawback is that they cannot be referenced in formulas like named references.  This basically makes them
    ///     only useful as
    ///     bind targets for formulas.
    /// </remarks>
    /// <note>
    ///     Since each instance of an external reference is unique, you must keep track of the instance you create because you
    ///     will
    ///     need to supply it when you wish to get the formula bound to it from the engine.
    /// </note>
    public interface IExternalReference : IReference
    {
        /// <summary>Gets result of evaluating the reference's formula</summary>
        /// <value>The result of the reference's formula</value>
        /// <remarks>
        ///     You typically will listen to the <see cref="E:ciloci.FormulaEngine.IExternalReference.Recalculated" /> event and
        ///     when it fires you use this property in your handler to get the latest
        ///     value of the reference's formula.
        /// </remarks>
        object Result { get; }

        /// <summary>Signals that the reference's formula has been recalculated</summary>
        /// <remarks>
        ///     You can listen to this event to be notified when the formula that is bound to this reference has been
        ///     recalculated.
        /// </remarks>
        event EventHandler Recalculated;
    }
}