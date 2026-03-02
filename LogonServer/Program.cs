using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogonServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            server LoginServer = new server("0.0.0.0",6767);


        }
    } 
}
