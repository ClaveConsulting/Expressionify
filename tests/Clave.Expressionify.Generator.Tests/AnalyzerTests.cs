using System.Threading.Tasks;
using NUnit.Framework;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.AnalyzerVerifier<Clave.Expressionify.Generator.ExpressionifyAnalyzer>;

namespace Clave.Expressionify.Generator.Tests
{
    public class AnalyzerTests
    {
        [Test]
        public async Task TestNullPropagationMethod()
        {
            var test = @"
                namespace ConsoleApplication1
                {
                    public partial class Extensions
                    {
                        [Expressionify]
                        public static int? GetYear(System.DateTime? x) => x?.Year;
                    }
                    
                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }";

            var expected = Verify.Diagnostic(ExpressionifyAnalyzer.NullPropagationRule)
                .WithSpan("/0/Test0.cs", 7, 75, 7, 82)
                .WithArguments("GetYear");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}
