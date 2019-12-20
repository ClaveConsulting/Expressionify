using System;

namespace Clave.Expressionify.Example
{
    public static class Extensions
    {
        [Expressionify]
        public static bool IsOver18(this Student student) => DateTime.Now.AddYears(18) > student.DateOfBirth;
    }
}
