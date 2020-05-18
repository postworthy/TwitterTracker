using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterTracker.Core;

namespace TwitterTracker.Extract.Urls
{
    public class Program
    {
        static void Main(string[] args)
        {
            foreach(var line in Run(args, ConsoleIn()))
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
            foreach (var input in inputs)
            {
                var urls = new List<string>();
                Status status = null;
                try
                {
                    status = Status.FromBase64String(input);
                    if (status != null && status.entities != null && status.entities.urls != null)
                        urls = status.entities.urls.Select(x => x.expanded_url).ToList();
                    if (status != null && status.entities != null && status.entities.media != null)
                        urls = status.entities.media.Select(x => x.expanded_url).ToList();
                }
                catch
                {
                }

                if (urls.Count > 0 && status != null)
                {
                    int retweets = status.retweet_count ?? 0;
                    var ot = status.retweeted_status;
                    while (ot != null)
                    {
                        if (ot.retweet_count > retweets)
                            retweets = ot.retweet_count.Value;

                        ot = ot.retweeted_status;
                    }
                    retweets = retweets / urls.Count; //Split the RT love between them all
                    foreach (var x in urls)
                    {
                        yield return x + "#retweets=" + retweets;
                    }
                }
                else
                {
                    yield return "Error Handling: " + input;
                }
            }
        }
    }
}
