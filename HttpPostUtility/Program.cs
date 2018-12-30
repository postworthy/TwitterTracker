using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpPostUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri destination = null;
            string postParameterName = null;
            if (args.Length == 2 && Uri.TryCreate(args[0], UriKind.Absolute, out destination))
            {
                postParameterName = args[1];
                var input = Console.ReadLine();
                while (!string.IsNullOrEmpty(input))
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.Headers["Content-Type"] = "application/json";
                                byte[] response =
                                client.UploadData(
                                    destination, 
                                    "POST",
                                    Encoding.Default.GetBytes("{\"" + postParameterName + "\":\"" + input + "\"}"));
                            }
                        }
                        catch(Exception ex)
                        {
                            ex = ex;
                        }
                    });
                    input = Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Usage: HttpPostUtility <URL_TO_POST_DATA> <POST_PARAMETER_NAME>");
                Console.WriteLine("EX: HttpPostUtility http://yourawesomedomainname.tld/api/endpoint postdata");
            }
        }
    }
}
