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
                args = new[] { "-tv", Console.ReadLine() };
            }
            
            var tracker = Tracker.New(
                args[1], 
                args[0].Contains("v") ? Console.Out : null);

            foreach(var tweet in tracker.ResultStream())
            {
                Console.WriteLine(tweet);
            }
        }
    }
}
