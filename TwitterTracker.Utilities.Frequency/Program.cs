using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Utilities.Frequency
{
    class Program
    {
        private class Item
        {
            public string Value { get; set; }
            public int Count { get; set; }
            public DateTime LastSeen { get; set; }
            public DateTime Created { get; set; }

            public Item()
            {
                Created = DateTime.Now;
            }

            public void Seen()
            {
                Count++;
                LastSeen = DateTime.Now;
            }

            public double Rank()
            {
                var score = Count;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var twitterStartDate = Convert.ToInt64((new DateTime(2006, 7, 15, 0, 0, 0, DateTimeKind.Utc) - epoch).TotalSeconds);
                var createDate = Convert.ToInt64((Created.ToUniversalTime() - epoch).TotalSeconds);
                var seconds = createDate - twitterStartDate;
                var order = Math.Log10(Math.Max(Math.Abs(score), 1));
                var sign = (score > 0) ? 1 : 0;
                return Math.Round(order + ((sign * seconds) / 45000.0), 7);
            }
        }
        static void Main(string[] args)
        { 
            
            int max = 0;
            if (args.Length == 1)
                int.TryParse(args[0], out max);

            if (max == 0)
                max = 1000;

            var items = new Dictionary<string, Item>(max);
            var errors = new List<string>();

            var input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                if (input.StartsWith("http"))
                {
                    if (items.ContainsKey(input))
                        items[input].Seen();
                    else
                    {
                        if (items.Count >= max)
                            items.Remove(items.OrderByDescending(x => x.Value.Rank()).Last().Key);

                        items.Add(input, new Item() { Value = input, Count = 1, LastSeen = DateTime.Now });

                    }
                }
                else
                    errors.Add(input);

                UpdateConsole(items, errors);

                input = Console.ReadLine();
            }
        }

        private static void UpdateConsole(Dictionary<string, Item> items, List<string> errors)
        {
            if (!Console.IsOutputRedirected)
                Console.Clear();

            Console.WriteLine("{0}\t\t{1}\t{2}\t\t{3}", "Rank", "Count", "Last Seen", "URL");
            Console.WriteLine("");

            var ordered = items
                .OrderByDescending(x => x.Value.Rank())
                .ThenByDescending(x=>x.Value.LastSeen)
                .Take(75);
            foreach (var i in ordered)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", i.Value.Rank(), i.Value.Count, i.Value.LastSeen, i.Value.Value);
            }

            Console.WriteLine("");
            Console.WriteLine("Last Updated: {0}",DateTime.Now);
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
