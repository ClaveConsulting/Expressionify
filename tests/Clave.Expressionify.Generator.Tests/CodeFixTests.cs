namespace Clave.Expressionify.Generator.Tests;
using System.Threading.Tasks;
using NUnit.Framework;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.CodeFixVerifier<ExpressionifyAnalyzer, ExpressionifyCodeFixProvider>;

public class CodeFixTests
{
    [Test]
    public async Task TestNothing()
    {
        var test = @"";

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Test]
    public async Task TestOkMethod()
    {
        var test = """
                namespace ConsoleApplication1
                {
                    public partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Test]
    public async Task TestWithoutNamespace()
    {

        var test = """
                public partial class Extensions
                {
                    [Expressionify]
                    public static int Foo(int x) => 8;
                }

                [System.AttributeUsage(System.AttributeTargets.Method)]
                public class ExpressionifyAttribute : System.Attribute {}
                """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Test]
    public async Task TestWithFileScopedNamespace()
    {
        var test = """
                namespace ConsoleApplication1;

                public partial class Extensions
                {
                    [Expressionify]
                    public static int Foo(int x) => 8;
                }

                [System.AttributeUsage(System.AttributeTargets.Method)]
                public class ExpressionifyAttribute : System.Attribute {}
                """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Test]
    public async Task TestMissingStatic()
    {
        var test = """
                namespace ConsoleApplication1
                {
                    public partial class Extensions
                    {
                        [Expressionify]
                        public int Foo(int x) => 8;
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        var expected = Verify.Diagnostic(ExpressionifyAnalyzer.StaticRule)
            .WithSpan("/0/Test0.cs", 5, 9, 6, 36)
            .WithArguments("Foo");

        var fixtest = """
                namespace ConsoleApplication1
                {
                    public partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        await Verify.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [Test]
    public async Task TestNotExpressionBody()
    {
        var test = """
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) { return 8; }
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        var expected = Verify.Diagnostic(ExpressionifyAnalyzer.ExpressionBodyRule)
            .WithSpan("/0/Test0.cs", 5, 9, 6, 51)
            .WithArguments("Foo");

        await Verify.VerifyCodeFixAsync(test, expected, test);
    }

    [Test]
    public async Task TestNotInPartialClass()
    {
        var test = """
                namespace ConsoleApplication1
                {
                    public static class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        var expected = Verify.Diagnostic(ExpressionifyAnalyzer.PartialClassRule)
            .WithSpan("/0/Test0.cs", 3, 5, 7, 6);

        await Verify.VerifyAnalyzerAsync(test, new [] { expected });

        var fixtest = """
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }

                    [System.AttributeUsage(System.AttributeTargets.Method)]
                    public class ExpressionifyAttribute : System.Attribute {}
                }
                """;

        await Verify.VerifyCodeFixAsync(test, expected, fixtest);
    }
}