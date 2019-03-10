using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace CloudUtility
{
    class Program
    {

        static void Main(string[] args)
        {
            Task.Factory.StartNew(async () =>
            {
                //Bypass Cert Validation
                if (args[0].Contains("x"))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }

                var containerKey = Environment.GetEnvironmentVariable("AzureStorageContainerKey") ?? ConfigurationManager.AppSettings["AzureStorageContainerKey"];
                if (string.IsNullOrEmpty(containerKey))
                    throw new Exception("Missing AzureStorageContainerKey value!");
                else
                    containerKey = containerKey.ToLower();

                var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString") ?? ConfigurationManager.AppSettings["AzureStorageConnectionString"];
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("Missing AzureStorageConnectionString value!");

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerKey);

                if (!(await container.ExistsAsync()))
                    await container.CreateAsync();

                var valid = new[] { "-u", "-d", "-r", "-ux", "-dx", "-rx", "-xu", "-xd", "-xr", };

                while (args.Length != 1 && !valid.Any(x => x == args[0]))
                {
                    Console.WriteLine("What would you like to do with input (ex: (-u)pload, (-d)ownload, (-r)emove)");
                    args = new[] {
                        Console.ReadLine().ToLower()
                    };

                }

                if (args[0].Contains("u")) //Handle Uploads
                {
                    var input = Console.ReadLine();
                    string key = null;
                    while (!string.IsNullOrEmpty(input))
                    {
                        string json = null;
                        try
                        {
                            json = ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(input));

                            //In the event that we do not have a key then assign the container key as the item name
                            if (string.IsNullOrEmpty(key))
                                key = containerKey;

                            await UploadBlob(container.GetBlockBlobReference(key), json);

                            key = null;
                        }
                        catch
                        {
                            //If you can't base64 decode the string then assume it is the key
                            if (string.IsNullOrEmpty(json))
                                key = input;
                            else
                                key = null;
                        }

                        input = Console.ReadLine();
                    }
                }
                else if (args[0].Contains("d")) //Handle Downloads
                {
                    var input = Console.ReadLine();
                    while (!string.IsNullOrEmpty(input))
                    {
                        try
                        {
                            Console.WriteLine(await DownloadBlob(container.GetBlockBlobReference(input)));
                        }
                        catch
                        {
                            Console.WriteLine("Failed to download {0}", input);
                        }

                        input = Console.ReadLine();
                    }
                }
                else if (args[0].Contains("r")) //Handle Removes
                {
                    var input = Console.ReadLine();
                    while (!string.IsNullOrEmpty(input))
                    {
                        try
                        {
                            await container.GetBlockBlobReference(input).DeleteAsync();
                            Console.WriteLine("Deleted {0}", input);
                        }
                        catch
                        {
                            Console.WriteLine("Failed to delete {0}", input);
                        }

                        input = Console.ReadLine();
                    }
                }
            }).Wait();
        }

        private static async Task UploadBlob(CloudBlockBlob blob, string obj)
        {
            using (var streamCompressed = new MemoryStream())
            {
                using (var gzip = new GZipStream(streamCompressed, CompressionMode.Compress))
                {
                    var data = Encoding.UTF8.GetBytes(obj);
                    gzip.Write(data, 0, data.Length);
                    gzip.Flush();
                    gzip.Close();

                    using (var streamOut = new MemoryStream(streamCompressed.ToArray()))
                    {
                        await blob.UploadFromStreamAsync(streamOut);
                    }
                }
            }
        }

        private static async Task<string> DownloadBlob(CloudBlockBlob blob)
        {
            using (var stream = new MemoryStream())
            {
                StreamReader reader;
                try
                {
                    await blob.DownloadToStreamAsync(stream);
                }
                catch
                {
                    return null;
                }
                try
                {
                    stream.Seek(0, 0);
                    reader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress));
                    var json = reader.ReadToEnd();
                    return json;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
