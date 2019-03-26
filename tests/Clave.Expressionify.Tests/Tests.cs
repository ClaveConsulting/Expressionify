using System;
using System.Diagnostics;
using System.Linq;
using Clave.Expressionify;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void TestExpression()
        {
            var data = new[]{
                "1",
                "2",
                "3"
            };

            data.AsQueryable()
                .Expressionify()
                .Select(x => x.ToInt())
                .ToList();

            Assert.Pass();
        }

        [Test]
        public void TestExpressionMultipleTimes()
        {
            var data = new[]{
                new {a = "1", b = "5"},
                new {a = "2", b = "5"},
                new {a = "3", b = "5"}
            };

            data.AsQueryable()
                .Expressionify()
                .Select(x => x.a.ToInt() + x.b.ToInt())
                .ToList();

            Assert.Pass();
        }

        [Test]
        public void TestExpressionTwice()
        {
            var data = new[]{
                "1",
                "2",
                "3"
            };

            var sw = Stopwatch.StartNew();
            data.AsQueryable()
                .Expressionify()
                .Select(x => x.ToDouble())
                .ToList();
            var firstTime = sw.Elapsed;

            sw.Restart();
            data.AsQueryable()
                .Expressionify()
                .Select(x => x.ToDouble())
                .ToList();
            var secondTime = sw.Elapsed;

            Assert.Less(secondTime, firstTime);
        }

        [Test]
        public void TestNaming()
        {
            var name = ExpressionifyVisitor.GetExpressionifyClassName(typeof(ExtensionMethods).AssemblyQualifiedName);

            Assert.AreEqual("Tests.ExtensionMethods_Expressionify, Clave.Expressionify.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", name);
        }
    }
}
