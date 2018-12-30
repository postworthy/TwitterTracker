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
					var urls = new List<string>();
					if(status != null && status.entities!= null && status.entities.urls != null)
						urls = status.entities.urls.Select(x=>x.expanded_url).ToList();
					if(status != null && status.entities!= null && status.entities.media != null)
						urls = status.entities.media.Select(x=>x.expanded_url).ToList();
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
						urls.ForEach(x => Console.WriteLine(x + "#retweets=" + retweets));
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
