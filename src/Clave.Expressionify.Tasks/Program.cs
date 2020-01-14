using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Clave.Expressionify.Tasks
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

                try
                {
                    var classes = root.TransformClasses();
                    
                    if (classes.Any())
                    {
                        var newClass = root.WithOnlyTheseClasses(classes.ToArray());
                        var newPath = Path.Combine(projectPath, "obj", "CodeGen", path);
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                        File.WriteAllText(newPath, newClass);
                        Console.WriteLine(newPath);
                    }
                }
                catch (CodeGenException e)
                {
                    Console.Error.WriteLine(path + e.Message);
                    return 1;
                }
            }

            return 0;
        }
    }

    public class CodeGenException : Exception
    {
        public CodeGenException(string message) : base(message)
        {
            
        }
    }
}