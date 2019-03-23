using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Clave.Expressionify.CodeGen
{
    public class ExpressionifyTask : Task
    {
        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string[] SourceFiles { get; set; }

        [Output]
        public string[] GeneratedClasses { get; set; }

        public override bool Execute()
        {
            var generatedClasses = new List<string>();
            foreach (var path in SourceFiles)
            {
                using (var stream = File.OpenRead(path))
                {
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
                            Log.LogError($"{path}({line}): error 0: A method with [Expressionify] attribute must have expression body");
                            return false;
                        }

                        properties.Add(method.ToExpressionProperty());
                    }

                    if (properties.Any())
                    {
                        var newClass = root.WithOnlyTheseProperties(properties);
                        var newPath = $"{ProjectPath}/obj/CodeGen/{path}";
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                        File.WriteAllText(newPath, newClass);
                        generatedClasses.Add(newPath);
                    }
                }
            }

            GeneratedClasses = generatedClasses.ToArray();

            return true;
        }
    }
}