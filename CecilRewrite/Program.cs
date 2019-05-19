using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CecilRewriteAndAddExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetAssemblyName = "UniNativeLinq.dll";
            using var resolver = new DefaultAssemblyResolver();
        }
    }
}
