using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Verify = Clave.Expressionify.Generator.Tests.Verifiers.CSharpSourceGeneratorVerifier<Clave.Expressionify.Generator.ExpressionifySourceGenerator>;

namespace Clave.Expressionify.Generator.Tests
{
    [TestFixture]
    public class CodeGeneratorTests
    {
        private const string AttributeCode = @"
        namespace ConsoleApplication1
        {
            using System;

            [AttributeUsage(AttributeTargets.Method)]
            public class ExpressionifyAttribute : Attribute
            {
            }
        }";

        // Normal scenario
        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;
    }
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0() => (int x) => 8;
    }
}", TestName = "Normal scenario")]

        // File scoped namespace
        [TestCase(@"namespace ConsoleApplication1;
public partial class Extensions
{
    [Expressionify]
    public static int Foo(int x) => 8;
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0() => (int x) => 8;
    }
}", TestName = "File scoped namespace")]

        // Nested class
        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;

        public partial class Nested
        {
            [Expressionify]
            public static int Foo(int x) => 8;
        }
    }
}",
    @"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0() => (int x) => 8;
        public partial class Nested
        {
            private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0() => (int x) => 8;
        }
    }
}", TestName = "Nested class")]

        // Nullable
        [TestCase(@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static string? Foo(int x) => x < 10 ? null : ""bar"";
    }
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, string?>> Foo_Expressionify_0() => (int x) => x < 10 ? null : ""bar"";
    }
}", TestName = "Nullable")]

        // Nullable enabled but not used
        [TestCase(@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static string Foo(int x) => ""bar"";
    }
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, string>> Foo_Expressionify_0() => (int x) => ""bar"";
    }
}", TestName = "Nullable enabled but not used")]

        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static string Foo<T>(T x) => ""bar"";
    }
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<T, string>> Foo_Expressionify_0<T>() => (T x) => ""bar"";
    }
}", TestName = "Generic extension method")]

        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static string Foo<T>(T x) where T : System.Collections.IEnumerable => ""bar"";
    }
}",
@"#nullable enable

namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<T, string>> Foo_Expressionify_0<T>()
            where T : System.Collections.IEnumerable => (T x) => ""bar"";
    }
}", TestName = "Generic extension method with constraints")]
        public async Task TestGenerator(string source, string generated)
        {
            await VerifyGenerated(source, generated);
        }

        public async Task VerifyGenerated(string source, string generated)
        {
            await new Verify.Test
            {
                TestState =
                {
                    Sources = { source, AttributeCode },
                    GeneratedSources =
                    {
                        (typeof(ExpressionifySourceGenerator), "Test0_expressionify_0.g.cs", SourceText.From(generated.Replace(Environment.NewLine, "\r\n"), Encoding.UTF8, SourceHashAlgorithm.Sha1)),
                    }
                }
            }.RunAsync();
        }
    }
}