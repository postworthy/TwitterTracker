using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterTracker.Core;

namespace TwitterTracker.Utilities.Frequency
{
    public class Program
    {
        static void Main(string[] args)
        {
            foreach (var line in Run(args, ConsoleIn()))
            {
                Console.WriteLine(line);
            }
        }

        private static IEnumerable<string> ConsoleIn()
        {
            while (true)
            {
                yield return Console.ReadLine();
            }
        }

        public static IEnumerable<string> Run(string[] args, IEnumerable<string> inputs)
        {

            bool console = false;
            int max = 0;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (int.TryParse(arg, out max))
                        break;
                }
                foreach (var arg in args)
                {
                    if (arg == "-console")
                    {
                        console = true;
                        break;
                    }
                }
            }

            if (max == 0)
                max = 1000;

            var items = new Dictionary<string, FrequencyItem>(max);
            var errors = new List<string>();

            foreach (var input in inputs)
            {
                var lastOutput = DateTime.MinValue;

                var split = input.Contains("#retweets=") ?
                    input.Split(new[] { "#retweets=" }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray() :
                    input.Split(',');
                var count = 0;
                var item = split[1];
                var isItem = int.TryParse(split[0], out count) && item.StartsWith("http");
                if (isItem)
                {
                    if (items.ContainsKey(item))
                        items[item].Seen(count);
                    else
                    {
                        if (items.Count >= max)
                            items.Remove(items.OrderByDescending(x => x.Value.Rank()).Last().Key);

                        items.Add(item, new FrequencyItem() { Value = item, Count = Math.Max(count, 1), LastSeen = DateTime.Now });

                    }
                }
                else
                    errors.Add(input);

                if (console)
                    UpdateConsole(items, errors);
                else if (items.Count >= 25 && (DateTime.Now - lastOutput).TotalSeconds > 60 * 5)
                {
                    lastOutput = DateTime.Now;
                    yield return 
                        Convert.ToBase64String(
                            ASCIIEncoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(
                                    new FrequencyOutput
                                    {
                                        OutputTime = lastOutput,
                                        Items = items
                                            .OrderByDescending(x => x.Value.Rank())
                                            .ThenByDescending(x => x.Value.LastSeen)
                                            .Select(x => x.Value)
                                            .ToArray()
                                    })));
                }
            }
        }

        private static void UpdateConsole(Dictionary<string, FrequencyItem> items, List<string> errors)
        {
            if (!Console.IsOutputRedirected)
                Console.Clear();

            Console.WriteLine("{0}\t\t{1}\t{2}\t\t{3}", "Rank", "Count", "Last Seen", "URL");
            Console.WriteLine("");

            var ordered = items
                .OrderByDescending(x => x.Value.Rank())
                .ThenByDescending(x => x.Value.LastSeen)
                .Take(50);
            foreach (var i in ordered)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", i.Value.Rank(), i.Value.Count, i.Value.LastSeen, i.Value.Value);
            }

            Console.WriteLine("");
            Console.WriteLine("Last Updated: {0}", DateTime.Now);
            Console.WriteLine("");
            foreach (var e in errors)
            {
                Console.WriteLine("Error Handling: {0}", e);
            }
            if (Console.IsOutputRedirected)
                Console.WriteLine("----------");
        }
    }
}
