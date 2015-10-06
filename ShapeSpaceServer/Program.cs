using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpaceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                Console.WriteLine("Version: " + args[0]);
            }
            Console.ReadKey(true);
        }
    }
}
