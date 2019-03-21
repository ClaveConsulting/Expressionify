using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Expressionate.CodeGen
{
  public class ExpressionateTask : Task
    {
        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string[] SourceFiles { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"project: {ProjectPath}");
            foreach (var path in SourceFiles)
            {
                Log.LogMessage(MessageImportance.High, $"file: {path}");
                using (var stream = File.OpenRead(path))
                {
                    var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                    var root = tree.GetRoot();
                    var methods = root
                        .DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.AttributeLists
                            .SelectMany(l => l.Attributes)
                            .Any(a => a.Name.ToString() == "Expressify"));

                    var properties = new List<PropertyDeclarationSyntax>();

                    foreach (var method in methods)
                    {
                        Log.LogMessage(MessageImportance.High, $"  method: {method.Identifier.Text}");

                        if (method.ExpressionBody == null)
                        {
                            var line = 1 + method.Identifier.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
                            Log.LogError($"{path}({line}): error 0: A method with [Expressify] attribute must have expression body");
                            return false;
                        }

                        var property = method.ToExpressionProperty();

                        properties.Add(property);
                    }
                    if(properties.Any()){
                        var newClass = root.WithOnlyTheseProperties(properties);
                        var newPath = $"{ProjectPath}/obj/CodeGen/{path}";
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                        File.WriteAllText(newPath, newClass);
                    }
                }
            }

            return true;
        }
    }
}