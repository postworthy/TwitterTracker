using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Core
{
    public class FrequencyItem
    {
        public string Value { get; set; }
        public int Count { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime Created { get; set; }

        public FrequencyItem()
        {
            Created = DateTime.Now;
        }

        public void Seen(int c = 0)
        {
            Count++;
            if (c > Count)
                Count = c;
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

    public class FrequencyOutput
    {
        public DateTime OutputTime { get; set; }
        public FrequencyItem[] Items { get; set; }
    }
}
