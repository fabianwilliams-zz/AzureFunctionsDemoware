using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using affun.Utils;

namespace af
    ._4_SendTeamsMessage
{
    public static class NewContactTeamsMsg
    {
        private static Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(() => new HttpClient());

        [FunctionName("NewContactTeamsMsg")]
        public static async Task SendNewContactTeamsMessage([QueueTrigger("YOURTeamsListenerQUEUENAMEHERE", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            var data = JsonConvert.DeserializeObject<FullCardInfoTable>(myQueueItem);
            string dString = ($"payload: {myQueueItem}").Replace("\"","|");

            string WebhookUrl = Environment.GetEnvironmentVariable("FWorldNewContactTeamWebHook");
            log.LogInformation("Sending to Microsoft Teams Channel Now");
            var teamsResult = await HttpClient.Value.PostAsync(WebhookUrl,
                new StringContent($"{{\"@type\": \"MessageCard\",\"@context\": \"http://schema.org/extensions\",\"summary\": \"I Met a new Contact\",\"themeColor\": \"0075FF\",\"sections\": [{{\"startGroup\": true,\"title\": \"**New Contact Details:**\",\"text\": \"I met {data.Info.Name} : Email: {data.Info.Email}, Website: {data.Info.Website}, Additional Details: {dString}\"}}]}}"));

            teamsResult.EnsureSuccessStatusCode();
            log.LogInformation($"Result is {teamsResult.StatusCode}");
            log.LogInformation($"C# Fabian New Contact Queue Function completded..: {myQueueItem}");
        }
    }
}
