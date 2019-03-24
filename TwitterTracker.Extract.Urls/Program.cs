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
            Run(args, Console.In, Console.Out);
        }

        public static void Run(string[] args, TextReader inputStream,  TextWriter outputStream)
        {
            var input = inputStream.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var status = Status.FromBase64String(input);
                    var urls = new List<string>();
                    if (status != null && status.entities != null && status.entities.urls != null)
                        urls = status.entities.urls.Select(x => x.expanded_url).ToList();
                    if (status != null && status.entities != null && status.entities.media != null)
                        urls = status.entities.media.Select(x => x.expanded_url).ToList();
                    if (urls.Count > 0)
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
                        urls.ForEach(x => outputStream.WriteLine(x + "#retweets=" + retweets));
                    }
                }
                catch
                {
                    outputStream.WriteLine("Error Handling: " + input);
                }
                input = inputStream.ReadLine();
            }
        }
    }
}
