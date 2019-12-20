using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clave.Expressionify.Example
{
    class Program
    {
        static async Task Main()
        {
            using var context = new SchoolContext();
            await Initialize(context);

            var student = await context.Students
                .Expressionify()
                .Where(s => s.IsOver18())
                .FirstOrDefaultAsync();

            Console.WriteLine(student.Name);
        }

        private static async Task Initialize(SchoolContext context)
        {
            var std = new Student()
            {
                Name = "Bill",
                DateOfBirth = DateTime.Now.AddYears(-16)
            };

            context.Students.Add(std);
            await context.SaveChangesAsync();
        }
    }
}
