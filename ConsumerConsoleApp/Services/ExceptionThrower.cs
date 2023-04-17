using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerConsoleApp.Services
{
    internal class ExceptionThrower
    {
        public static void DoIt() {
            Log.Warning("I'm about to create an exception!");
            throw new Exception("Big Exception happened");
        }
    }
}
