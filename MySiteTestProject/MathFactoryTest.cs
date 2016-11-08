using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySite.Core;
using MySite.Services;
using MySite.ReadifyKnockService;

namespace MySiteTestProject
{
    [TestClass]
    public class MathFactoryTest
    {
        [TestMethod]
        public void ReverseWordsTest()
        {
            var firstIn = "abc cba/ccb";
            var res = ReadifyFactory.ReverseWords(firstIn);
            Assert.AreEqual("cba abc/bcc", res);
        }

        [TestMethod]
        public void ReverseWords2Test()
        {
            var firstIn = "abc cba/123";
            var res = ReadifyFactory.ReverseWords(firstIn);
            Assert.AreEqual("cba abc/123", res);
        }

        [TestMethod]
        public void ShapeIsEquilateral()
        {
            var res = ReadifyFactory.WhatShapeIsThis(1, 1, 1);
            Assert.AreEqual(TriangleType.Equilateral, res);
        }

        [TestMethod]
        public void ShapeIsIsosceles()
        {
            var res = ReadifyFactory.WhatShapeIsThis(2, 1, 2);
            Assert.AreEqual(TriangleType.Isosceles, res);
        }

        [TestMethod]
        public void ShapeIsScalene()
        {
            var res = ReadifyFactory.WhatShapeIsThis(5, 3, 7);
            Assert.AreEqual(TriangleType.Scalene, res);
        }

        [TestMethod]
        public void ShapeSideIsNegative()
        {
            var res = ReadifyFactory.WhatShapeIsThis(-1, 3, 7);
            Assert.AreEqual(TriangleType.Error, res);
        }

        [TestMethod]
        public void ShapeBordersNotConnect()
        {
            var res = ReadifyFactory.WhatShapeIsThis(3, 4, 10);
            Assert.AreEqual(TriangleType.Error, res);
        }

        [TestMethod]
        public void PositiveFibonacciNumberTest()
        {
            var firstIn = 9;
            var res = ReadifyFactory.GetFibonacciNumber(firstIn);
            Assert.AreEqual(34, res);
        }

        [TestMethod]
        public void NegativeFibonacciNumberTest()
        {
            var firstIn = -8;
            var res = ReadifyFactory.GetFibonacciNumber(firstIn);
            Assert.AreEqual(-21, res);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetFibonacciNumberArgExceptionTest()
        {
            var firstIn = 100;
            var res = ReadifyFactory.GetFibonacciNumber(firstIn);
            Assert.AreEqual(93, res);
        }
    }
}
