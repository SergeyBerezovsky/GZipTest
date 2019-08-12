using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GZipTest
{
    class Program
    {       
        static int Main(string[] args)
        {     
            Console.WriteLine(args[0] + " " + args[1] + " " + args[2]);
            DateTime begin = DateTime.Now;
            Operator opr = new Operator(args[0],args[1],args[2]);
            opr.Start();
            DateTime end = DateTime.Now;
            TimeSpan ts = end - begin;
            Console.WriteLine();
            Console.WriteLine("Время выполнения {0}",ts);
            
            Console.ReadKey();
            return opr.errorMsg();
        }
    }     

}
