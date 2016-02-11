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

            var input = Console.ReadLine();
            string key = null;
            while (!string.IsNullOrEmpty(input))
            {
                string json = null;
                try
                {
                    json = ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(input));

                    if (string.IsNullOrEmpty(key))
                        key = Math.Abs(json.GetHashCode()).ToString();

                    UploadBlob(container.GetBlockBlobReference(key), json);

                    key = null;
                }
                catch
                {
                    if (string.IsNullOrEmpty(json))
                        key = input;
                    else
                        key = null;
                }

                input = Console.ReadLine();
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
    }
}
