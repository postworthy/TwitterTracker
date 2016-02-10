using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterTracker.Core;

namespace TwitterTracker.Extract.Urls
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var status = Status.FromBase64String(input);
                    if (status.entities.urls.Count > 0)
                    {
                        int retweets = status.retweet_count ?? 0;
                        var ot = status.retweeted_status;
                        while (ot != null)
                        {
                            if (ot.retweet_count > retweets)
                                retweets = ot.retweet_count.Value;

                            ot = ot.retweeted_status;
                        }
                        retweets = retweets / status.entities.urls.Count; //Split the RT love between them all
                        status.entities.urls.ForEach(e => Console.WriteLine(retweets + "," + e.expanded_url));
                    }
                }
                catch{
                    Console.WriteLine("Error Handling: " + input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
