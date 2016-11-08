using MySite.ReadifyKnockService;
using System;
using System.Diagnostics;
using System.ServiceModel;
using MySite.Core;

namespace MySite.Services
{
    public class Readify : IRedPill
    {
        /// <summary>
        /// Sending my Readify token
        /// </summary>
        /// <returns>Readify token</returns>
        public Guid WhatIsYourToken()
        {
            return new Guid("321ba856-320d-422d-a2db-353d7b0ccd04");
        }

        /// <summary>
        /// Calculating fibonacci number by received negative or positive index
        /// </summary>
        /// <param name="n">Negative or positive index</param>
        /// <exception cref="ArgumentOutOfRangeException">The incoming parameter should be between -92..92</exception>
        /// <returns>The fibonacci number</returns>
        public long FibonacciNumber(long n)
        {
            try
            {
                return ReadifyFactory.GetFibonacciNumber(n);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Trace.TraceWarning("Argument was out or range while invoking FibonacciNumber. n: {0}, Error: {1}", n, ex);
                throw new FaultException<ArgumentOutOfRangeException>(ex, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error invoking FibonacciNumber. n: {0}, Error: {1}", n, ex);
                throw new FaultException("Sorry, there happened something unexpected!!!");
            }
        }

        /// <summary>
        /// Checking list of digits that they are really the dimensions of sides of triangle
        /// </summary>
        /// <param name="a">The first size of side</param>
        /// <param name="b">The second size of side</param>
        /// <param name="c">The third size of side</param>
        /// <returns>The triangle type</returns>
        public TriangleType WhatShapeIsThis(int a, int b, int c)
        {
            try
            {
                return ReadifyFactory.WhatShapeIsThis(a, b, c);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error invoking WhatShapeIsThis. a: {0}, b: {1}, c: {2}, Error: {3}", a, b, c, ex);
                throw new FaultException("Sorry, there happened something unexpected!!!");
            }
        }

        /// <summary>
        /// Reverse word's letters ignoring any symbols, whitespace, delimiters and digits
        /// </summary>
        /// <param name="s">The list of workds</param>
        /// <returns>String with reversed words</returns>
        public string ReverseWords(string s)
        {
            try
            {
                return ReadifyFactory.ReverseWords(s);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error invoking ReverseWords. a: {0}, Error: {1}", s, ex);
                throw new FaultException("Sorry, there happened something unexpected!!!");
            }
        }
    }
}
