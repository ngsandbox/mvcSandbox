using System;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Represents an argument to a formula function
    /// </summary>
    /// <remarks>
    ///     This class represents an argument passed to a formula function.  Every such function will
    ///     receive an array of instances of this class; one for each argument the function was called with.  The class has
    ///     properties
    ///     for determining the type of the argument passed and getting its value.
    /// </remarks>
    public sealed class Argument
    {
        private readonly IOperand MyOperand;

        internal Argument(IOperand op)
        {
            MyOperand = op;
        }

        /// <summary>
        ///     Gets the value of an argument as a <see cref="System.Double" />
        /// </summary>
        /// <value>The value of the argument as a <see cref="System.Double" /></value>
        /// <remarks>This property will try to convert the value of the argument to a <see cref="System.Double" />.</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to a <see cref="System.Double" />
        /// </exception>
        public double ValueAsDouble
        {
            get
            {
                var op = (DoubleOperand)MyOperand.Convert(OperandType.Double);
                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return op.ValueAsDouble;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as an <see cref="System.Int32" />
        /// </summary>
        /// <value>The value of the argument as an <see cref="System.Int32" /></value>
        /// <remarks>This property will try to convert the value of the argument to an <see cref="System.Int32" />.</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to an <see cref="System.Int32" />
        /// </exception>
        public int ValueAsInteger
        {
            get
            {
                var op = (IntegerOperand)MyOperand.Convert(OperandType.Integer);
                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return op.ValueAsInteger;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as a <see cref="System.String" />
        /// </summary>
        /// <value>The value of the argument as a <see cref="System.String" /></value>
        /// <remarks>This property will try to convert the value of the argument to a <see cref="System.String" />.</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to a <see cref="System.String" />
        /// </exception>
        public string ValueAsString
        {
            get
            {
                var op = (StringOperand)MyOperand.Convert(OperandType.String);
                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }

                return op.ValueAsString;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as a <see cref="System.Boolean" />
        /// </summary>
        /// <value>The value of the argument as a <see cref="System.Boolean" /></value>
        /// <remarks>This property will try to convert the value of the argument to a <see cref="System.Boolean" />.</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to a
        ///     <see cref="System.Boolean" />
        /// </exception>
        public bool ValueAsBoolean
        {
            get
            {
                var op = (BooleanOperand)MyOperand.Convert(OperandType.Boolean);
                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return op.ValueAsBoolean;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as a <see cref="System.DateTime" />
        /// </summary>
        /// <value>The value of the argument as a <see cref="System.DateTime" /></value>
        /// <remarks>This property will try to convert the value of the argument to a <see cref="System.DateTime" />.</remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to a
        ///     <see cref="System.DateTime" />
        /// </exception>
        public DateTime ValueAsDateTime
        {
            get
            {
                var op = (DateTimeOperand)MyOperand.Convert(OperandType.DateTime);
                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return op.ValueAsDateTime;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as an <see cref="T:System.ciloci.FormulaEngine.ErrorValueType" />
        /// </summary>
        /// <value>The value of the argument as an <see cref="T:System.ciloci.FormulaEngine.ErrorValueType" /></value>
        /// <remarks>
        ///     This property will try to convert the value of the argument to an
        ///     <see cref="T:System.ciloci.FormulaEngine.ErrorValueType" />.
        /// </remarks>
        /// <exception cref="T:System.InvalidOperationException">
        ///     The value could not be converted to an
        ///     <see cref="T:System.ciloci.FormulaEngine.ErrorValueType" />
        /// </exception>
        public ErrorValueType ValueAsError
        {
            get
            {
                var op = (ErrorValueOperand)MyOperand.Convert(OperandType.Error);

                if (op == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return op.ValueAsErrorType;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as a reference
        /// </summary>
        /// <value>The value of the argument as a reference</value>
        /// <remarks>This property will try to convert the value of the argument to a reference.</remarks>
        /// <exception cref="T:System.InvalidOperationException">The value could not be converted to a reference</exception>
        public IReference ValueAsReference
        {
            get
            {
                var @ref = (IReference)MyOperand.Convert(OperandType.Reference);

                if (@ref == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return @ref;
            }
        }

        /// <summary>
        ///     Gets the value of an argument as a primitive
        /// </summary>
        /// <value>The value of the argument as a primitive</value>
        /// <remarks>
        ///     This property will try to convert the value of the argument to a primitive.  A primitive is any datatype
        ///     except a reference.
        /// </remarks>
        /// <exception cref="T:System.InvalidOperationException">The value could not be converted to a primitive</exception>
        public object ValueAsPrimitive
        {
            get
            {
                IOperand prim = MyOperand.Convert(OperandType.Primitive);

                if (prim == null)
                {
                    throw new InvalidOperationException("Conversion failed");
                }
                return prim.Value;
            }
        }

        internal IOperand ValueAsOperand
        {
            get { return MyOperand; }
        }

        /// <summary>
        ///     Indicates whether this argument is a double
        /// </summary>
        /// <value>True if the argument can be converted to a double; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsDouble
        {
            get { return IsType(OperandType.Double); }
        }

        /// <summary>
        ///     Indicates whether this argument is an integer
        /// </summary>
        /// <value>True if the argument can be converted to an integer; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsInteger
        {
            get { return IsType(OperandType.Integer); }
        }

        /// <summary>
        ///     Indicates whether this argument is a string
        /// </summary>
        /// <value>True if the argument can be converted to a string; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsString
        {
            get { return IsType(OperandType.String); }
        }

        /// <summary>
        ///     Indicates whether this argument is a boolean
        /// </summary>
        /// <value>True if the argument can be converted to a boolean; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsBoolean
        {
            get { return IsType(OperandType.Boolean); }
        }

        /// <summary>
        ///     Indicates whether this argument is a reference
        /// </summary>
        /// <value>True if the argument can be converted to a reference; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsReference
        {
            get { return IsType(OperandType.Reference); }
        }

        /// <summary>
        ///     Indicates whether this argument is an error value
        /// </summary>
        /// <value>True if the argument can be converted to an error value; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsError
        {
            get { return IsType(OperandType.Error); }
        }

        /// <summary>
        ///     Indicates whether this argument is a DateTime
        /// </summary>
        /// <value>True if the argument can be converted to a DateTime; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        /// </remarks>
        public bool IsDateTime
        {
            get { return IsType(OperandType.DateTime); }
        }

        /// <summary>
        ///     Indicates whether this argument is a primitive
        /// </summary>
        /// <value>True if the argument can be converted to a primitive; False otherwise</value>
        /// <remarks>
        ///     Use this property to test if the argument can be converted to a particular data type before trying to get its
        ///     value.
        ///     A primitive is any data type except a reference.
        /// </remarks>
        public bool IsPrimitive
        {
            get { return IsType(OperandType.Primitive); }
        }

        /// <summary>
        ///     Determines if this argument can be converted to a particular type
        /// </summary>
        /// <param name="opType">The operand type you wish to test</param>
        /// <returns>True if the argument can be converted to opType; False otherwise</returns>
        /// <remarks>This method is a more generic version of the IsXXX properties</remarks>
        public bool IsType(OperandType opType)
        {
            return MyOperand.Convert(opType) != null;
        }
    }
}