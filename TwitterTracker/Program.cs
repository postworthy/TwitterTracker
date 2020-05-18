using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterTracker.Core;

namespace TwitterTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                if (!(args.Length == 1 && args[0].Contains("u")))
                {
                    Console.WriteLine("What would you like to track (ex: Twitter,Google,Apple)");
                    args = new[] {
                        args.Length == 1 && args[0].Contains("t") ? args[0] : "-tv",
                        Console.ReadLine()
                    };
                }
            }

            foreach(var line in Run(args))
            {
                Console.WriteLine(line);
            }
        }

        public static IEnumerable<string> Run(string[] args)
        {
            //Bypass Cert Validation
            if (args[0].Contains("x"))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
            }

            Tracker tracker = null;

            if (args.Length == 2)
                tracker = Tracker.New(args[1], args[0].Contains("v") ? Console.Out : null);
            else
                tracker = Tracker.New(args[0], args[0].Contains("v") ? Console.Out : null);

            var tweets = new ConcurrentBag<string>();

            var t = new Action(() =>
            {
                foreach (var tweet in tracker.ResultStream(Tracker.StreamTypes.Tracker))
                {
                    tweets.Add(tweet.ToString());
                }
            });

            var u = new Action(() =>
            {
                foreach (var tweet in tracker.ResultStream(Tracker.StreamTypes.User))
                {
                    tweets.Add(tweet.ToString());
                }
            });

            var tasks = new Task[] { };

            //Multithreaded only if we need it.
            if (args[0].Contains("t") && args[0].Contains("u"))
            {
                var actions = new List<Action>() { t, u };
                tasks = actions.Select(x => Task.Run(x)).ToArray();
                //Task.WaitAll(tasks);
            }
            else if (args[0].Contains("t"))
                tasks = new[] { Task.Run(t) };
            else if (args[0].Contains("u"))
                tasks = new[] { Task.Run(u) };

            while (true)
            {
                while (tweets.TryTake(out var tweet))
                {
                    yield return tweet;
                }
            }
        }
    }
}
