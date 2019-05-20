using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using affun.Utils;

namespace sc._5_MailChimp
{
    public static class MailChimpSubscriber
    {
        [FunctionName("MailChimpSubscriber")]
        public static void Run([QueueTrigger("scnewmailchimpsub", Connection = "AzureWebJobsStorage")]
                        FullCardInfoTable myQueueItem, 
                        ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var method = Environment.GetEnvironmentVariable("CartWiselyMailChimpListId"); ;
            var key = Environment.GetEnvironmentVariable("CartWiselyMailChimpKey"); ;

            var fullname = myQueueItem.Info.Name.ToString();
            var tmpFullName = fullname.Split(new char[] { ' ' });
            var fn = tmpFullName[0];
            var ln = tmpFullName[1];

            MailChimpSub subscribeRequest = new MailChimpSub()
            {
                EmailAddress = myQueueItem.Info.Email,
                Status = "subscribed",
                MergeFields = new MergeFields()
                {
                    Fname = fn,
                    Lname = ln
                }
            };

            string payload = JsonConvert.SerializeObject(subscribeRequest);
            log.LogInformation($"Working on Payload: {payload}");
            var endpoint = String.Format("https://{0}.api.mailchimp.com/3.0/{1}", "us20", method);
            byte[] dataStream = Encoding.UTF8.GetBytes(payload);
            var responsetext = string.Empty;
            WebRequest request = HttpWebRequest.Create(endpoint);
            WebResponse response = null;
            try
            {
                request.ContentType = "application/json";
                SetBasicAuthHeader(request, "anystring", key);  // BASIC AUTH
                request.Method = "POST";
                request.ContentLength = dataStream.Length;
                Stream newstream = request.GetRequestStream();

                newstream.Write(dataStream, 0, dataStream.Length);
                newstream.Close();

                response = request.GetResponse();

                // get the result
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    JsonSerializer json = new JsonSerializer();
                    JObject content = JObject.Parse(reader.ReadToEnd());

                    responsetext = reader.ReadToEnd();
                }

                response.Close();
            }

            catch (WebException ex)
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responsetext = sr.ReadToEnd();

                }
                log.LogInformation($"Error is {ex.ToString()}");
            }
        }

        private static void SetBasicAuthHeader(WebRequest request, string username, string password)
        {
            string auth = username + ":" + password;
            auth = Convert.ToBase64String(Encoding.Default.GetBytes(auth));
            request.Headers["Authorization"] = "Basic " + auth;
        }
    }
}
