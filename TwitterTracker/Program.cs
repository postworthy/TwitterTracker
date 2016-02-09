using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterTracker.Core;

namespace TwitterTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("What would you like to track (ex: Twitter,Google,Apple)");
                args = new[] { Console.ReadLine() };
            }
            
            //Console.WriteLine("Starting Tracker");

            var tracker = Tracker.New((x) => {
                Console.WriteLine(x);
            },
            track: args[0],
            log: Console.Out);

            while (!tracker.IsActive) ;
            //Console.WriteLine("Tracker Active");
            
            tracker.Wait();
        }
    }
}
