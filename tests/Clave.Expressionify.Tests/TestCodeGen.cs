using System.Linq;
using Clave.Expressionify.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Shouldly;

namespace Clave.Expressionify.Tests
{
    [TestFixture]
    public class TestCodeGen
    {
        [Test]
        public void TestBasic()
        {
            var root = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                }").GetRoot();

            var classes = root.TransformClasses();

            classes.Count.ShouldBe(1);

            classes[0].Identifier.Text.ShouldBe("Extensions_Expressionify");

            var properties = classes[0]
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            properties.Count.ShouldBe(1);

            properties[0].Identifier.Text.ShouldBe("Foo_0");
        }

        [Test]
        public void TestNonExpressionify()
        {
            var root = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        [Expressionify]
                        public static int Foo(int x) => 8;

                        public static int Bar(int x) => 0;
                    }
                }").GetRoot();

            var classes = root.TransformClasses();

            classes.Count.ShouldBe(1);

            classes[0].Identifier.Text.ShouldBe("Extensions_Expressionify");

            var properties = classes[0]
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            properties.Count.ShouldBe(1);

            properties[0].Identifier.Text.ShouldBe("Foo_0");
        }

        [Test]
        public void TestNothing()
        {
            var root = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        public static int Foo(int x) => 8;

                        public static int Bar(int x) => 0;
                    }
                }").GetRoot();

            var classes = root.TransformClasses();

            classes.ShouldBeEmpty();
        }

        [Test]
        public void TestTwoClasses()
        {
            var root = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                    public static class Extensions2{
                        [Expressionify]
                        public static int Bar(int x) => 0;
                    }
                }").GetRoot();

            var classes = root.TransformClasses();

            classes.Count.ShouldBe(2);

            classes[0].Identifier.Text.ShouldBe("Extensions_Expressionify");
            classes[1].Identifier.Text.ShouldBe("Extensions2_Expressionify");

            var properties1 = classes[0]
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            properties1.Count.ShouldBe(1);

            properties1[0].Identifier.Text.ShouldBe("Foo_0");

            var properties2 = classes[1]
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            properties2.Count.ShouldBe(1);

            properties2[0].Identifier.Text.ShouldBe("Bar_0");
        }


        [Test]
        public void TestOverload()
        {
            var root = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        [Expressionify]
                        public static int Foo(int x) => 8;

                        [Expressionify]
                        public static int Foo(string x) => 0;
                    }
                }").GetRoot();

            var classes = root.TransformClasses();

            classes.Count.ShouldBe(1);

            classes[0].Identifier.Text.ShouldBe("Extensions_Expressionify");

            var properties = classes[0]
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .ToList();

            properties.Count.ShouldBe(2);

            properties[0].Identifier.Text.ShouldBe("Foo_0");

            properties[1].Identifier.Text.ShouldBe("Foo_1");
        }

        [Test]
        public void TestMethodGroup()
        {
            var source = CSharpSyntaxTree.ParseText(@"using System;
                namespace Test {
                    public static class Extensions{
                        [Expressionify]
                        public static int Foo(int x) => 8;

                        [Expressionify]
                        public static int Something(IEnumerable<string> x) => x.Select(Foo);
                    }
                }");

            var properties = source.GetRoot().TransformProperties();

            Assert.IsNotEmpty(properties);
        }
    }
}