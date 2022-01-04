using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using VerifyCS = Clave.Expressionify.Generator.Tests.Verifiers.CSharpSourceGeneratorVerifier<Clave.Expressionify.Generator.ExpressionifySourceGenerator>;

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

        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;
    }
}",
@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0 { get; } = (int x) => 8;
    }
}")]
        public async Task TestBasicClass(string source, string generated)
        {
            await VerifyGenerated(source, generated);
        }

        public async Task VerifyGenerated(string source, string generated)
        {
            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { source, AttributeCode },
                    GeneratedSources =
                    {
                        (typeof(ExpressionifySourceGenerator), "Generated_0.cs", SourceText.From(generated, Encoding.UTF8, SourceHashAlgorithm.Sha1)),
                    },
                },
            }.RunAsync();
        }
    }
}
