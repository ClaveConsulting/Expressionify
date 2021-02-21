using Clave.Expressionify.Generator.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Clave.Expressionify.Generator.Tests
{
    public class Tests : CodeFixVerifier
    {
        [Test]
        public void TestNothing()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void TestOkMethod()
        {

            var test = @"
                namespace ConsoleApplication1
                {
                    public partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                }";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void TestMissingStatic()
        {
            var test = @"
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public int Foo(int x) => 8;
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = ExpressionifyAnalyzer.StaticId,
                Message = "Method Foo marked with [Expressionify] must be static",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 6, 25)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                }";

            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void TestNotExpressionBody()
        {
            var test = @"
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) { return 8; }
                    }
                }";
            var expected = new DiagnosticResult
            {
                Id = ExpressionifyAnalyzer.ExpressionBodyId,
                Message = "Method Foo marked with [Expressionify] must have expression body",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 6, 25)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyNoCSharpFix(test);
        }

        [Test]
        public void TestNotInPartialClass()
        {
            var test = @"
                namespace ConsoleApplication1
                {
                    public static class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                }";
            var expected = new DiagnosticResult
            {
                Id = ExpressionifyAnalyzer.PartialClassId,
                Message = "Class containing a method marked with [Expressionify] must be partial",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 4, 21)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
                namespace ConsoleApplication1
                {
                    public static partial class Extensions
                    {
                        [Expressionify]
                        public static int Foo(int x) => 8;
                    }
                }";

            VerifyCSharpFix(test, fixtest);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new ExpressionifyAnalyzer();
        protected override CodeFixProvider GetCSharpCodeFixProvider() => new ExpressionifyCodeFixProvider();
    }
}