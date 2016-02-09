using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterTracker.Core
{
    public class Tracker
    {
        private Stream stream = null;
        private TextWriter log = null;
        private Tracker() { }
        private Tracker(TextWriter log) { this.log = log; this.StreamResetAttempts = 0; }
        private int StreamResetAttempts { get; set; }
        private string track { get; set; }
        public static Tracker New(string track = null, TextWriter log = null)
        {
            var t = new Tracker(log);
            t.track = track ?? ConfigurationManager.AppSettings["Track"];
            t.track = string.Join(",", t.track.Split(',').Select(x => x.Trim()));
            return t;
        }
        public IEnumerable<Status> ResultStream(int maxResets = 5)
        {
            Status tweet = null;
            IEnumerator<Status> tweetEnumerator = null;
            while (StreamResetAttempts <= maxResets)
            {
                try
                {
                    if(tweetEnumerator == null)
                        tweetEnumerator = ResultStream(track).GetEnumerator();

                    tweetEnumerator.MoveNext();
                    tweet = tweetEnumerator.Current;

                    if (tweet == null)
                        throw new Exception("Null Tweet");
                }
                catch (Exception ex)
                {
                    if (tweetEnumerator != null)
                        tweetEnumerator.Dispose();

                    if (log != null)
                        log.WriteLine("{0}: Error: {1}", DateTime.Now, ex.ToString());

                    if (StreamResetAttempts >= maxResets)
                        throw new Exception("Max Reset Attempts Reached!");
                    else
                    {
                        if (log != null)
                            log.WriteLine("{0}: Sleeping for {1} seconds before next attempt.", DateTime.Now, StreamResetAttempts * 2);

                        Thread.Sleep(2000 * StreamResetAttempts++);
                    }
                }

                yield return tweet;
            }
        }

        private IEnumerable<Status> ResultStream(string track)
        {
            var request = OAuth.CreateSignedRequest(new Uri("https://stream.twitter.com/1.1/statuses/filter.json?delimited=length&track=" + Uri.EscapeDataString(track)));
            using (stream = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                while (stream.CanRead && !reader.EndOfStream)
                {
                    var length = 0;
                    var data = reader.ReadLine();
                    if (int.TryParse(data, out length))
                    {
                        var tweetData = new char[length];
                        reader.Read(tweetData, 0, length);
                        Status tweet = null;
                        try
                        {
                            tweet = JsonConvert.DeserializeObject<Status>(new string(tweetData));
                        }
                        catch (Exception ex)
                        {
                            if (log != null)
                                log.WriteLine("{0}: Error: {1}", DateTime.Now, ex.ToString());
                        }

                        if (tweet != null && tweet.id > 0)
                            yield return tweet;
                    }
                }
            }
        }
    }
}
