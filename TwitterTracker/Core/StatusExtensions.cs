using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Core
{
    public static class StatusExtensions
    {
        private static Status Submit(this Status s, Uri uri, Dictionary<string,string> postParams = null, TextWriter log = null)
        {
            var request = OAuth.CreateSignedRequest(uri, postParams);
            using (var stream = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                while (stream.CanRead && !reader.EndOfStream)
                {
                    var tweetData = reader.ReadToEnd();
                    Status tweet = null;
                    try
                    {
                        tweet = JsonConvert.DeserializeObject<Status>(tweetData);
                    }
                    catch (Exception ex)
                    {
                        if (log != null)
                            log.WriteLine("{0}: Error: {1}", DateTime.Now, ex.ToString());
                    }

                    if (tweet != null && tweet.id > 0)
                        return tweet;
                }
            }

            return null;
        }

        public static Status Submit(this Status s, TextWriter log = null)
        {
            if (!string.IsNullOrEmpty(s.text))
                return s.Submit(new Uri("https://api.twitter.com/1.1/statuses/update.json?status=" + Uri.EscapeDataString(s.text)), new Dictionary<string, string>(), log);
            else
                return null;
        }

        public static Status Delete(this Status s, TextWriter log = null)
        {
            if (s.id > 0)
                return s.Submit(new Uri("https://api.twitter.com/1.1/statuses/destroy/" + s.id + ".json"), new Dictionary<string, string>(), log);
            else
                return null;
        }
    }
}
