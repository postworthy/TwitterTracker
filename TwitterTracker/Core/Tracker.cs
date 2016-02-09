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
        private Task streamTask = null;
        private Stream stream = null;
        private TextWriter log = null;
        private Action<Tweet> statusHandler = null;
        private Tracker() { }
        private Tracker(Action<Tweet> statusHandler, TextWriter log) { this.statusHandler = statusHandler; this.log = log; this.StreamResetAttempts = 0; }
        public bool IsActive { get { return stream != null && stream.CanRead; } }
        public int StreamResetAttempts { get; set; }
        public static Tracker New(Action<Tweet> statusHandler, string track = null, TextWriter log = null)
        {
            var t = new Tracker(statusHandler, log);
            t.Start(track);
            return t;
        }
        private void Start(string track = null)
        {
            streamTask = Task.Run(() =>
            {
                while (StreamResetAttempts <= 5)
                {
                    track = track ?? ConfigurationManager.AppSettings["Track"];

                    try
                    {
                        var request = OAuth.CreateSignedRequest(new Uri("https://stream.twitter.com/1.1/statuses/filter.json?delimited=length&track=" + track));
                        using (stream = request.GetResponse().GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            while (stream.CanRead && !reader.EndOfStream)
                            {
                                var length = 0;
                                var data = reader.ReadLine();
                                if(int.TryParse(data, out length))
                                {
                                    var tweetData = new char[length];
                                    reader.Read(tweetData, 0, length);
                                    
                                    try
                                    {
                                        var tweet = JsonConvert.DeserializeObject<Tweet>(new string(tweetData));
                                        if (tweet.id > 0)
                                            statusHandler(tweet);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (log != null)
                                            log.WriteLine("{0}: Error: {1}", DateTime.Now, ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (log != null)
                            log.WriteLine("{0}: Error: {1}", DateTime.Now, ex.ToString());

                        if (StreamResetAttempts > 5)
                            throw new Exception("Max Reset Attempts Reached!");
                        else
                        {
                            if (log != null)
                                log.WriteLine("{0}: Sleeping for {1} seconds before next attempt.", DateTime.Now, StreamResetAttempts * 2);
                            Thread.Sleep(2000 * StreamResetAttempts++);
                            Start(track);
                        }
                    }
                }
            });
        }
        public void Wait()
        {
            if (streamTask != null)
                streamTask.Wait();
        }
    }
}
