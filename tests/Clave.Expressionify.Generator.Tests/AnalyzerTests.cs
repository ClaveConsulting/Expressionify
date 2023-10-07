namespace Clave.Expressionify.Generator.Tests;
using System.Threading.Tasks;
using NUnit.Framework;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.AnalyzerVerifier<ExpressionifyAnalyzer>;

public class AnalyzerTests
{
    [Test]
    public async Task TestNullPropagationMethod()
    {
        var test = """
            namespace ConsoleApplication1
            {
                public partial class Extensions
                {
                    [Expressionify]
                    public static int? GetYear(System.DateTime? x) => x?.Year;
                }

                [System.AttributeUsage(System.AttributeTargets.Method)]
                public class ExpressionifyAttribute : System.Attribute {}
            }
            """;

        var expected = Verify.Diagnostic(ExpressionifyAnalyzer.NullPropagationRule)
            .WithSpan("/0/Test0.cs", 6, 59, 6, 66)
            .WithArguments("GetYear");

        await Verify.VerifyAnalyzerAsync(test, expected);
    }
}
