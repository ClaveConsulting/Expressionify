using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Clave.Expressionify.Tests.First;
using Clave.Expressionify.Tests.Samples;
using NUnit.Framework;
using Shouldly;

namespace Clave.Expressionify.Tests
{
    public class Tests
    {
        [Test]
        public void TestBasic()
        {
            var prop = typeof(Class1).GetProperty("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop.ShouldNotBeNull();
            var expr = prop.GetValue(null) as Expression<Func<int, int>>;
            expr.ShouldNotBeNull();
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
            typeof(Class3).GetProperties(BindingFlags.NonPublic|BindingFlags.Static).ShouldNotBeEmpty();

            var prop0 = typeof(Class3).GetProperty("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop0.ShouldNotBeNull();
            var expr0 = prop0.GetValue(null) as Expression<Func<int, int>>;
            expr0.ShouldNotBeNull();
            expr0.Compile().Invoke(1).ShouldBe(8);

            var prop1 = typeof(Class3).GetProperty("Foo_Expressionify_1", BindingFlags.NonPublic | BindingFlags.Static);
            prop1.ShouldNotBeNull();
            var expr1 = prop1.GetValue(null) as Expression<Func<string, int>>;
            expr1.ShouldNotBeNull();
            expr1.Compile().Invoke("test").ShouldBe(0);
        }

        [Test]
        public void TestMethodGroup()
        {
            typeof(Class4).GetProperties(BindingFlags.NonPublic | BindingFlags.Static).ShouldNotBeEmpty();

            var prop0 = typeof(Class4).GetProperty("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop0.ShouldNotBeNull();
            var expr0 = prop0.GetValue(null) as Expression<Func<string, int>>;
            expr0.ShouldNotBeNull();
            expr0.Compile().Invoke("1").ShouldBe(8);

            var prop1 = typeof(Class4).GetProperty("Something_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop1.ShouldNotBeNull();
            var expr1 = prop1.GetValue(null) as Expression<Func<IEnumerable<string>, int>>;
            expr1.ShouldNotBeNull();
            expr1.Compile().Invoke(new[] { "test" }).ShouldBe(8);
        }
        [Test]
        public void TestExpressionify()
        {
            var data = new[]{
                "1",
                "2",
                "3"
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.ToInt())
                .ToList();

            result.ShouldBe(new[] { 1, 2, 3 });
        }

        [Test]
        public void TestMethodParameterUsedTwice()
        {
            var data = new[]{
                1,
                2,
                3
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.Squared())
                .ToList();

            result.ShouldBe(new[] { 1, 4, 9 });
        }

        [Test]
        public void TestMethodParameterUsedTwiceWithOverload()
        {
            var data = new[]{
                1.0,
                2.0,
                3.0
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.Squared())
                .ToList();

            result.ShouldBe(new[] { 1.0, 4.0, 9.0 });
        }

        [Test]
        public void TestMethodWithMultipleArguments()
        {
            var data = new[]{
                new {a = "1", b = "5"},
                new {a = "3", b = "5"},
                new {a = "2", b = "5"},
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.a.Pluss(x.b))
                .ToList();

            result.ShouldBe(new[] { 6, 8, 7 });
        }

        [Test]
        public void TestMethodCalledMultipleTimes()
        {
            var data = new[]{
                new {a = "1", b = "5"},
                new {a = "2", b = "5"},
                new {a = "3", b = "5"}
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.a.ToInt() + x.b.ToInt())
                .ToList();

            result.ShouldBe(new[] { 6, 7, 8 });
        }

        [Test]
        public void TestExpressionifiedTwice()
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

            secondTime.ShouldBeLessThan(firstTime);
        }
    }
}
