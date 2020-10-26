using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Clave.Expressionify.Generator.Tests.Samples;
using NUnit.Framework;
using Shouldly;

namespace Clave.Expressionify.Generator.Tests
{
    public class Tests
    {
        [Test]
        public void TestBasic()
        {
            var prop = typeof(Class1).GetProperty("Foo_0");
            prop.ShouldNotBeNull();
            var expr = prop.GetValue(null) as Expression<Func<int, int>>;
            expr.Compile().Invoke(1).ShouldBe(8);
        }

        [Test]
        public void TestNonExpressionify()
        {
            typeof(Class2).GetProperties().ShouldBeEmpty();
        }

        [Test]
        public void TestOverload()
        {
            typeof(Class3).GetProperties().ShouldNotBeEmpty();

            var prop0 = typeof(Class3).GetProperty("Foo_0");
            prop0.ShouldNotBeNull();
            var expr0 = prop0.GetValue(null) as Expression<Func<int, int>>;
            expr0.Compile().Invoke(1).ShouldBe(8);

            var prop1 = typeof(Class3).GetProperty("Foo_1");
            prop1.ShouldNotBeNull();
            var expr1 = prop1.GetValue(null) as Expression<Func<string, int>>;
            expr1.Compile().Invoke("test").ShouldBe(0);
        }

        [Test]
        public void TestMethodGroup()
        {
            typeof(Class4).GetProperties().ShouldNotBeEmpty();

            var prop0 = typeof(Class4).GetProperty("Foo_0");
            prop0.ShouldNotBeNull();
            var expr0 = prop0.GetValue(null) as Expression<Func<string, int>>;
            expr0.Compile().Invoke("1").ShouldBe(8);

            var prop1 = typeof(Class4).GetProperty("Something_0");
            prop1.ShouldNotBeNull();
            var expr1 = prop1.GetValue(null) as Expression<Func<IEnumerable<string>, int>>;
            expr1.Compile().Invoke(new[] { "test" }).ShouldBe(8);
        }
    }
}