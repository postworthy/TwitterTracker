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
                    if (args?.Length > 0)
                    {
                        var status = JObject.Parse(input);
                        var fail = false;
                        foreach (var arg in args)
                        {
                            try
                            {
                                var parts = arg.Split(new[] { "==", ">=", "<=", ">", "<" }, StringSplitOptions.RemoveEmptyEntries);
                                var propertyPath = parts[0].Split('.');
                                var value = parts[1];
                                JToken propRef = status;
                                foreach (var p in propertyPath)
                                {
                                    var x = p.Split('[');
                                    if (x.Length == 1)
                                        propRef = propRef[p];
                                    else
                                        propRef = ((JArray)propRef[x[0]]).ElementAt(int.Parse(x[1].TrimEnd(']')));
                                }

                                if (arg.Contains("==") && Convert.ToString(propRef) != value)
                                    fail = true;
                                else if (arg.Contains(">=") && Convert.ToDouble(propRef) < Convert.ToDouble(value))
                                    fail = true;
                                else if (arg.Contains("<=") && Convert.ToDouble(propRef) > Convert.ToDouble(value))
                                    fail = true;
                                else if (arg.Contains(">") && Convert.ToDouble(propRef) <= Convert.ToDouble(value))
                                    fail = true;
                                else if (arg.Contains("<") && Convert.ToDouble(propRef) >= Convert.ToDouble(value))
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
