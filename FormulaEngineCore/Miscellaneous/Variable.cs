using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Represents a named value in a formula
    /// </summary>
    /// <remarks>
    ///     This class encapsulates a named value that can be used in formulas.
    ///     You define a variable using the <see cref="M:ciloci.FormulaEngine.FormulaEngine.DefineVariable(System.String)" />
    ///     method, set its value, and
    ///     then reference it from other formulas.  Once you are finished with the variable, you call its Dispose method to
    ///     remove it from
    ///     the formula engine.
    /// </remarks>
    /// <example>
    ///     This example shows how to define a variable, use it in a formula, and then undefine it:
    ///     <code>
    /// Dim engine As New FormulaEngine
    /// ' Create a new variable called 'x'
    /// Dim v As Variable = engine.DefineVariable("x")
    /// ' Give it a value of 100
    /// v.Value = 100
    /// ' Create a formula that uses the variable
    /// Dim f As Formula = engine.CreateFormula("=x*cos(x)")
    /// ' Evaluate the formula to get a result
    /// Dim result As Object = f.Evaluate()
    /// ' Undefine the variable
    /// v.Dispose()
    /// </code>
    /// </example>
    public class Variable
    {
        private FormulaEngine MyEngine;

        private NamedReference MyReference;

        internal Variable(FormulaEngine engine, string name)
        {
            MyEngine = engine;
            MyReference = (NamedReference) engine.ReferenceFactory.Named(name);
            engine.AddFormula("=0", MyReference);
            MyReference = (NamedReference) engine.ReferencePool.GetPooledReference(MyReference);
            MyReference.OperandValue = null;
        }

        /// <summary>
        ///     Gets the name of the variable
        /// </summary>
        /// <value>The variable's name</value>
        /// <remarks>Use this property to get the name of the variable</remarks>
        public string Name
        {
            get { return MyReference.Name; }
        }

        /// <summary>
        ///     Gets or sets the value of the variable
        /// </summary>
        /// <value>The value of the variable</value>
        /// <remarks>
        ///     Use this property to get the value of the variable or to assign a new value to it.  Once a new value is assigned,
        ///     all formulas that reference it will use the new value once they are recalculated.
        /// </remarks>
        public object Value
        {
            get { return MyReference.OperandValue; }
            set { MyReference.OperandValue = value; }
        }

        /// <summary>
        ///     Undefines the variable
        /// </summary>
        /// <remarks>Call this method when you are finished using the variable and wish to remove it from the formula engine</remarks>
        public void Dispose()
        {
            MyEngine.RemoveFormulaAt(MyReference);
            MyReference = null;
            MyEngine = null;
        }
    }
}