using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Clave.Expressionify.CodeGen
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var projectPath = args[0];
            var sourceFiles = args.Skip(1).FirstOrDefault()?.Split(';');

            foreach (var path in sourceFiles)
            {
                using var stream = File.OpenRead(Path.IsPathRooted(path) ? path : Path.Combine(projectPath, path));
                var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                var root = tree.GetRoot();
                var methods = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.AttributeLists
                        .SelectMany(l => l.Attributes)
                        .Any(a => a.Name.ToString() == "Expressionify"));

                var properties = new List<PropertyDeclarationSyntax>();

                foreach (var method in methods)
                {
                    if (method.ExpressionBody == null)
                    {
                        var line = 1 + method.Identifier.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
                        Console.Error.WriteLine($"{path}({line}): error 0: A method with [Expressionify] attribute must have expression body");
                        return 1;
                    }

                    if (method.Modifiers.FirstOrDefault(m => m.Kind() == SyntaxKind.StaticKeyword) == null)
                    {
                        var line = 1 + method.Identifier.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
                        Console.Error.WriteLine($"{path}({line}): error 0: A method with [Expressionify] attribute must be static. Make it an extension method if you need to use properties of the object");
                        return 1;
                    }

                    try
                    {
                        properties.Add(method.ToExpressionProperty());
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"Error parsing {path} {method.Identifier}: {e.Message}");
                        throw;
                    }
                }

                if (properties.Any())
                {
                    var newClass = root.WithOnlyTheseProperties(properties);
                    var newPath = Path.Combine(projectPath, "obj", "CodeGen", path);
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.WriteAllText(newPath, newClass);
                    Console.WriteLine(newPath);
                }
            }

            return 0;
        }
    }
}