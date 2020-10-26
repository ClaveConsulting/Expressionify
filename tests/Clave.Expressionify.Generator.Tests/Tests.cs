using Clave.Expressionify.Generator.Tests.Samples;
using NUnit.Framework;
using Shouldly;

namespace Clave.Expressionify.Generator.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Class1.ToInt_0.Compile().Invoke("1").ShouldBe(1);
        }
    }
}