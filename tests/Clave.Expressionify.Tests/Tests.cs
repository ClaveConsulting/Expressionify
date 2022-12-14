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
        public void TestClass()
        {
            var prop = typeof(Class1).GetMethod("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop.ShouldNotBeNull();
            var expr = prop.Invoke(null, Array.Empty<object>()) as Expression<Func<int, int>>;
            expr.ShouldNotBeNull();
            expr.Compile().Invoke(1).ShouldBe(8);
        }

        [Test]
        public void TestRecord()
        {
            var prop = typeof(Record1).GetMethod("Create_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop.ShouldNotBeNull();
            var expr = prop.Invoke(null, Array.Empty<object>()) as Expression<Func<string, Record1>>;
            expr.ShouldNotBeNull();
            expr.Compile().Invoke("test").ShouldBe(new Record1("test"));
        }

        [Test]
        public void TestNonExpressionify()
        {
            typeof(Class2).GetProperties().ShouldBeEmpty();
        }

        [Test]
        public void TestOverload()
        {
            typeof(Class3).GetMethods(BindingFlags.NonPublic|BindingFlags.Static).ShouldNotBeEmpty();

            var prop0 = typeof(Class3).GetMethod("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop0.ShouldNotBeNull();
            var expr0 = prop0.Invoke(null, Array.Empty<object>()) as Expression<Func<int, int>>;
            expr0.ShouldNotBeNull();
            expr0.Compile().Invoke(1).ShouldBe(8);

            var prop1 = typeof(Class3).GetMethod("Foo_Expressionify_1", BindingFlags.NonPublic | BindingFlags.Static);
            prop1.ShouldNotBeNull();
            var expr1 = prop1.Invoke(null, Array.Empty<object>()) as Expression<Func<string, int>>;
            expr1.ShouldNotBeNull();
            expr1.Compile().Invoke("test").ShouldBe(0);
        }

        [Test]
        public void TestMethodGroup()
        {
            typeof(Class4).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).ShouldNotBeEmpty();

            var prop0 = typeof(Class4).GetMethod("Foo_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop0.ShouldNotBeNull();
            var expr0 = prop0.Invoke(null, Array.Empty<object>()) as Expression<Func<string, int>>;
            expr0.ShouldNotBeNull();
            expr0.Compile().Invoke("1").ShouldBe(8);

            var prop1 = typeof(Class4).GetMethod("Something_Expressionify_0", BindingFlags.NonPublic | BindingFlags.Static);
            prop1.ShouldNotBeNull();
            var expr1 = prop1.Invoke(null, Array.Empty<object>()) as Expression<Func<IEnumerable<string>, int>>;
            expr1.ShouldNotBeNull();
            expr1.Compile().Invoke(new[] { "test" }).ShouldBe(8);
        }

        [Test]
        public void TestExpressionifyClass()
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
        public void TestExpressionifyNestedClass()
        {
            var data = new[]{
                1,
                2,
                3
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => Class4.NestedClass1.Bar(x))
                .ToList();

            result.ShouldBe(new[] { "=9", "=10", "=11" });
        }

        [Test]
        public void TestExpressionifyRecord()
        {
            var data = new[]{
                "1",
                "2",
                "3"
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => Record1.Create(x))
                .ToList();

            result.ShouldBe(new Record1[] { new("1"), new("2"), new("3") });
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

        [Test]
        public void TestGenericExpression()
        {
            var data = new IThing[]
            {
                new Thing1(),
                new Thing2(),
                new Thing1()
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => x.GetName())
                .ToList();

            result.ShouldBe(new[] { "Thing1", "Thing2", "Thing1" });
        }

        [Test]
        public void TestGenericType()
        {
            var data = new string[]
            {
                "1",
                "2",
                "3"
            };

            var result = data.AsQueryable()
                .Expressionify()
                .Select(x => GenericClass<string>.Foo(x))
                .ToList();

            result.ShouldBe(new[] { 8, 8, 8 });
        }
    }
}
