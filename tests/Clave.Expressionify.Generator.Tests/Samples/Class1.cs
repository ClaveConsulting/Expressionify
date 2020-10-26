using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clave.Expressionify.Generator.Tests.Samples
{
    public static partial class Class1
    {
        [Expressionify]
        private static int ToInt(this string value) => Convert.ToInt32(value);
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class ExpressionifyAttribute : Attribute
    {
    }
}
