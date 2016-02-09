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
                    status.entities.urls.ForEach(e => Console.WriteLine(e.expanded_url));
                }
                catch{
                    Console.WriteLine("Error Handling: " + input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
