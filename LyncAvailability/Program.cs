using LyncAvailability.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyncAvailability
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine(Resources.Usage);
                return;
            }

            LyncManagerStatus lyncManager = new LyncManagerStatus(args[0]);

            //wait for callback to set lm to true
            while (!lyncManager.Done)
            {
                //Console.WriteLine("running");
            }
        }
    }
}
