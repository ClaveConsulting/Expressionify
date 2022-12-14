using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clave.Expressionify.Tests.Samples
{
    public interface IThing
    {
        string Name { get; }
    }

    public class Thing1 : IThing
    {
        public string Name => "Thing1";
    }

    public class Thing2 : IThing
    {
        public string Name => "Thing2";
    }
}
