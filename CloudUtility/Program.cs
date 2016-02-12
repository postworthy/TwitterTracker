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
            var containerKey = ConfigurationManager.AppSettings["AzureStorageContainerKey"];
            if (string.IsNullOrEmpty(containerKey))
                throw new Exception("Config Section 'appSettings' missing AzureStorageContainerKey value!");

            var connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Config Section 'appSettings' missing AzureStorageConnectionString value!");

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerKey);

            if (!container.Exists())
                container.Create();

            var valid = new[] { "-u", "-d", "-r" };

            while (args.Length != 1 && !valid.Any(x => x == args[0]))
            {
                Console.WriteLine("What would you like to do with input (ex: (-u)pload, (-d)ownload, (-r)emove)");
                args = new[] {
                        Console.ReadLine().ToLower()
                    };

            }

            if (args[0] == "-u") //Handle Uploads
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

                        UploadBlob(container.GetBlockBlobReference(key), json);

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
            else if (args[0] == "-d") //Handle Downloads
            {
                var input = Console.ReadLine();
                while (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        Console.WriteLine(DownloadBlob(container.GetBlockBlobReference(input)));
                    }
                    catch
                    {
                        Console.WriteLine("Failed to download {0}", input);
                    }

                    input = Console.ReadLine();
                }
            }
            else if (args[0] == "-r") //Handle Removes
            {
                var input = Console.ReadLine();
                while (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        container.GetBlockBlobReference(input).Delete();
                        Console.WriteLine("Deleted {0}", input);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to delete {0}", input);
                    }

                    input = Console.ReadLine();
                }
            }
        }

        private static void UploadBlob(CloudBlockBlob blob, string obj)
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
                        blob.UploadFromStream(streamOut);
                    }
                }
            }
        }

        private static string DownloadBlob(CloudBlockBlob blob)
        {
            using (var stream = new MemoryStream())
            {
                StreamReader reader;
                try
                {
                    blob.DownloadToStream(stream, options: new BlobRequestOptions()
                    {
                        RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5), 3)
                    });
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
