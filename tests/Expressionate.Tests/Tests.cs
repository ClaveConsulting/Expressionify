using System.Linq;
using Expressionate;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestExpression()
        {
            var data = new[]{
                "1",
                "2",
                "3"
            };

            data.AsQueryable()
                .Expressionate()
                .Select(x => x.ToInt())
                .ToList();

            Assert.Pass();
        }

        [Test]
        public void TestNaming()
        {
            var name = ExpressionateVisitor.GetExpressionateClassName(typeof(ExtensionMethods).AssemblyQualifiedName);

            Assert.AreEqual("Tests.ExtensionMethods_Expressionate, Expressionate.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", name);
        }
    }
}