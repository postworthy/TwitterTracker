using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Filter
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
                    if (args != null && args.Length > 0)
                    {
                        var tweet = JObject.Parse("{root: [" + ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(input)) + "]}");
                        var fail = false;
                        foreach (var arg in args)
                        {
                            try
                            {
                                /* Args must use JSONPath here are some resources to get you started:
                                    1) http://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
                                    2) http://goessner.net/articles/JsonPath/

                                    Examples:
                                    Only pass tweets that have retweeted_status that is not null: "$..retweeted_status" 
                                    Only pass tweets that have hashtags of value news: "$..hashtags[?(@.text=='news')]"
                                    Only pass tweets that have a user above a selected followers_count" "$.root[?(@.user.followers_count>400)]"
                                    Only pass tweets that use english language: "$.root[?(@.lang=='en')]"
                                    Only pass tweets that use english or spanish language: $.root[?(@.lang=='en' || @.lang=='es')]
                                */
                                var val = tweet.SelectTokens(arg); 

                                if (val == null || val.Count() == 0)
                                    fail = true;
                            }
                            catch { fail = true; }
                        }

                        if (!fail)
                            Console.WriteLine(input);
                    }
                    else
                        Console.WriteLine(input);
                }
                catch { }
                input = Console.ReadLine();
            }

        }
    }
}
