using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Core
{
    public static class OAuth
    {
        private static string CreateAuthorizationHeaderParameter(
            string url,
            Dictionary<string, string> parameters,
            string oauth_consumer_key,
            string oauth_consumer_secret,
            string oauth_token,
            string oauth_token_secret,
            string method = "GET"
            )
        {
            var dictionary = new SortedDictionary<string, string>
                                 {
                                     {"oauth_version", "1.0"},
                                     {"oauth_consumer_key", oauth_consumer_key},
                                     {"oauth_nonce", Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()))},
                                     {"oauth_signature_method", "HMAC-SHA1"},
                                     {"oauth_timestamp", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString()},
                                     {"oauth_token", oauth_token}
                                 };

            foreach (var p in parameters)
            {
                dictionary.Add(p.Key, p.Value);
            }

            string baseString =
                method.ToUpper() +
                "&" +
                Uri.EscapeDataString(url) +
                "&" +
                Uri.EscapeDataString(string.Join("&", dictionary.Select(x => string.Format("{0}={1}", Uri.EscapeDataString(x.Key), Uri.EscapeDataString(x.Value)))));

            string signingKey = Uri.EscapeDataString(oauth_consumer_secret) + "&" + Uri.EscapeDataString(oauth_token_secret);

            var hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));

            string signatureString = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));

            string authorizationHeaderParams = "";
            authorizationHeaderParams += "OAuth ";
            authorizationHeaderParams += "oauth_consumer_key=" + "\"" + Uri.EscapeDataString(dictionary["oauth_consumer_key"]) + "\", ";
            authorizationHeaderParams += "oauth_nonce=" + "\"" + Uri.EscapeDataString(dictionary["oauth_nonce"]) + "\", ";
            authorizationHeaderParams += "oauth_signature=" + "\"" + Uri.EscapeDataString(signatureString) + "\", ";
            authorizationHeaderParams += "oauth_signature_method=" + "\"" + Uri.EscapeDataString(dictionary["oauth_signature_method"]) + "\", ";
            authorizationHeaderParams += "oauth_timestamp=" + "\"" + Uri.EscapeDataString(dictionary["oauth_timestamp"]) + "\", ";
            authorizationHeaderParams += "oauth_token=" + "\"" + Uri.EscapeDataString(dictionary["oauth_token"]) + "\", ";
            authorizationHeaderParams += "oauth_version=" + "\"" + Uri.EscapeDataString(dictionary["oauth_version"]) + "\"";

            return authorizationHeaderParams;
        }

        public static HttpWebRequest CreateSignedRequest(Uri uri, Dictionary<string, string> postParams = null)
        {
            var method = postParams == null ? "GET" : "POST";
            var requestParams = postParams ?? new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(uri.Query))
            {
                foreach (var x in uri.Query.TrimStart('?').Split('&'))
                {
                    requestParams.Add(x.Split('=')[0], x.Split('=')[1]);
                }
            }

            var hwr = (HttpWebRequest)WebRequest.Create(uri);
            hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            hwr.Headers.Add("Authorization",
                CreateAuthorizationHeaderParameter(
                    uri.OriginalString.Split('?')[0],
                    requestParams,
                    ConfigurationManager.AppSettings["ConsumerKey"],
                    ConfigurationManager.AppSettings["ConsumerSecret"],
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthTokenSecret"],
                    method));
            hwr.Method = method;
            hwr.Timeout = 3 * 60 * 1000;
            return hwr;
        }
    }
}
