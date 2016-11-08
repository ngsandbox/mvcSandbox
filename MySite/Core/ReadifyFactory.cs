using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySite.ReadifyKnockService;

namespace MySite.Core
{
    public static class ReadifyFactory
    {
        /// <summary>
        /// Calculating fibonacci number by received negative or positive index
        /// </summary>
        /// <param name="n">Negative or positive index</param>
        /// <exception cref="ArgumentOutOfRangeException">The incoming parameter should be between -92..92</exception>
        /// <returns>The fibonacci number</returns>
        public static long GetFibonacciNumber(long n)
        {
            Trace.TraceInformation("Start method GetFibonacciNumber: {0} ", n);
            long absN = Math.Abs(n);
            const long maxN = 92;
            if (absN > maxN)
            {
                throw new ArgumentOutOfRangeException(nameof(n), String.Format("The incoming parameter should be between -{0}..{0}", maxN));
            }

            long result = 0;
            if (absN != 0)
            {
                long second = 1;
                long sign = (n > 0) ? 1 : -1;
                for (long i = 0; i < absN; i++)
                {
                    var temp = result;
                    result = second;
                    second = temp + sign * second;
                }
            }

            Trace.TraceInformation("Finish method GetFibonacciNumber: {0} Result: {1}", n, result);
            return result;
        }

        /// <summary>
        /// Checking list of digits that they are really the dimensions of sides of triangle
        /// </summary>
        /// <param name="a">The first size of side</param>
        /// <param name="b">The second size of side</param>
        /// <param name="c">The third size of side</param>
        /// <returns>The triangle type</returns>
        public static TriangleType WhatShapeIsThis(int a, int b, int c)
        {
            Trace.TraceInformation("Start method WhatShape is this: {0} {1} {2}", a, b, c);
            TriangleType result = TriangleType.Error;
            try
            {
                if (a < 1 || b < 1 || c < 1 ||
                    a + (long)b <= c || b + (long)c <= a || c + (long)a <= b)
                {
                    return result = TriangleType.Error;
                }

                if (a == b && b == c)
                {
                    return result = TriangleType.Equilateral;
                }

                if (a == b || a == c || b == c)
                {
                    return result = TriangleType.Isosceles;
                }

                return result = TriangleType.Scalene;
            }
            finally
            {
                Trace.TraceInformation("Finish method WhatShape is this: {0} {1} {2}. Result: {3}", a, b, c, result);
            }
        }

        /// <summary>
        /// Reverse word's letters ignoring any symbols, whitespace, delimiters and digits
        /// </summary>
        /// <param name="s">The list of workds</param>
        /// <returns>String with reversed words</returns>
        public static string ReverseWords(string s)
        {
            Trace.TraceInformation("Start method ReverseWord: {0}", s);
            string result = "";
            if (!String.IsNullOrEmpty(s))
            {
                int index = 0;
                List<char> chars = new List<char>();
                foreach (char c in s)
                {
                    if (Char.IsLetter(c))
                    {
                        chars.Insert(index, c);
                    }
                    else
                    {
                        chars.Add(c);
                        index = chars.Count;
                    }
                }

                result = new string(chars.ToArray());
            }

            Trace.TraceInformation("Finish method ReverseWord: {0} Result: {1}", s, result);
            return result;
        }
    }
}