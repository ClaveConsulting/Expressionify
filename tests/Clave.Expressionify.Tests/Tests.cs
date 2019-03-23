using System;
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
        public void TestNaming()
        {
            var name = ExpressionifyVisitor.GetExpressionifyClassName(typeof(ExtensionMethods).AssemblyQualifiedName);

            Assert.AreEqual("Tests.ExtensionMethods_Expressionify, Expressionify.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", name);
        }
    }
}