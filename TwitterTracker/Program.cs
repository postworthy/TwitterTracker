﻿using System;
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

            Run(args, Console.Out);
        }

        public static void Run(string[] args, TextWriter stream)
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
                tracker = Tracker.New(args[1], args[0].Contains("v") ? stream : null);
            else
                tracker = Tracker.New(args[0], args[0].Contains("v") ? stream : null);

            var locker = new object();

            var t = new Action(() =>
            {
                foreach (var tweet in tracker.ResultStream(Tracker.StreamTypes.Tracker))
                {
                    lock (locker)
                    {
                        stream.WriteLine(tweet);
                    }
                }
            });

            var u = new Action(() =>
            {
                foreach (var tweet in tracker.ResultStream(Tracker.StreamTypes.User))
                {
                    lock (locker)
                    {
                        stream.WriteLine(tweet);
                    }
                }
            });

            //Multithreaded only if we need it.
            if (args[0].Contains("t") && args[0].Contains("u"))
            {
                var actions = new List<Action>() { t, u };
                var tasks = actions.Select(x => Task.Run(x)).ToArray();
                Task.WaitAll(tasks);
            }
            else if (args[0].Contains("t"))
                t();
            else if (args[0].Contains("u"))
                u();
        }
    }
}
