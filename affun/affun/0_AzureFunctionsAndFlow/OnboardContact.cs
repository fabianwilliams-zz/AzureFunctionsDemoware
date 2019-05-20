using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using FabianSpeakingData.shared;
using Microsoft.Azure.WebJobs.Host;
using System.Web.Http;
using System.Collections.Generic;
using SendGrid.Helpers.Mail;
using System.Net.Http;

namespace affun
{
    public static class OnboardContact
    {
        public static object email { get; private set; }
        [FunctionName("EmailNewContact")]
        public static void EmailNewContact(
[QueueTrigger("fabsqueue-sendgridemail", Connection = "AzureWebJobsStorage")]Contact myQueueItem,
TraceWriter log,
[SendGrid()] out SendGridMessage message)
        {
            string emailBody = File.ReadAllText(@"D:\home\site\wwwroot\emailBody.txt");
            string b64vCard = Environment.GetEnvironmentVariable("FabB64EncodedVCard");
            message = new SendGridMessage();
            message.AddTo(myQueueItem.EmailAddress);
            message.AddContent("text/html", emailBody);
            message.SetFrom(new EmailAddress("fabian@adotob.com"));
            message.SetSubject($"Great connecting with you {myQueueItem.FirstName} from Fabian Williams ");
            message.AddAttachment(@"FabianWilliams_PersonalvCard.vcf", b64vCard);

        }

        private static Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(() => new HttpClient());

        [FunctionName("ProcessMSTeamsQueue")]
        public static async Task ProcessMSTeamsQueue(
            [QueueTrigger("fabsqueue-msteamscard")] string myQueueItem,
            ILogger log)
        {
            var data = JsonConvert.DeserializeObject<Contact>(myQueueItem);

            string WebhookUrl = Environment.GetEnvironmentVariable("FWorldNewContactTeamWebHook");
            log.LogInformation("Sending to Microsoft Teams Channel Now");
            var teamsResult = await HttpClient.Value.PostAsync(WebhookUrl,
                new StringContent($"{{\"@type\": \"MessageCard\",\"@context\": \"http://schema.org/extensions\",\"summary\": \"I Met a new Contact\",\"themeColor\": \"0075FF\",\"sections\": [{{\"startGroup\": true,\"title\": \"**New Contact Details:**\",\"text\": \"While speaking I met {data.FirstName} {data.LastName} they have already been saved into our CRM but in the meantime here are some details for follow up: Email: {data.EmailAddress}, Mobile: {data.MobileNumber}, Additional Details: {data.TellMeAboutYou} All the other social informaion is in CRM\"}}]}}"));

            teamsResult.EnsureSuccessStatusCode();
            log.LogInformation($"Result is {teamsResult.StatusCode}");
            log.LogInformation($"C# Fabian New Contact Queue Function completded..: {myQueueItem}");
        }

        [FunctionName("CreateNewContact")]
        [Display(Name = "Create New Contact", Description = "Function creates a New Contact Individual in a Cosmoss DB Database")]
        public static async Task<IActionResult> CreateNewContact([HttpTrigger(AuthorizationLevel.Function, "post",
            Route = "contact")] HttpRequest req,
            [CosmosDB(
            databaseName: "FabianSessions",
            collectionName: "Contacts",
            ConnectionStringSetting = "FabsSessionCosmosDBConn")] IAsyncCollector<Contact> createSessionOut,
            TraceWriter log,
            [Queue("fabsqueue-msteamscard", Connection = "AzureWebJobsStorage")] IAsyncCollector<Contact> outputQueueContact,
            [Queue("fabsqueue-sendgridemail", Connection = "AzureWebJobsStorage")] IAsyncCollector<Contact> outputQueueEmail)
        {
            log.Info("Entry into the Create Function has occured...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Contact input = JsonConvert.DeserializeObject<Contact>(requestBody);

            log.Info($"Input Payload is: {input}");

            var newContact = new Contact()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                EmailAddress = input.EmailAddress,
                MobileNumber = input?.MobileNumber,
                TellMeAboutYou = input?.TellMeAboutYou,
                TwitterHandle = "https://twitter.com/" + input?.TwitterHandle,
                FaceBookHandle = "https://www.facebook.com/" + input?.FaceBookHandle,
                LinkedInHandle = "https://www.linkedin.com/in/" + input?.LinkedInHandle,
                InstagramHandle = "https://www.instagram.com/" + input?.InstagramHandle,
                GitHubHandle = "https://github.com/" + input?.GitHubHandle
            };
            await createSessionOut.AddAsync(newContact);

            log.Info($"New Contact Record Added to CosmosDB... Work is Finished");

            //write the Review to Azure Queue for Teams Message
            await outputQueueContact.AddAsync(newContact);

            log.Info($"New Contact Record Added... Just got added to Queue Storage");

            //write the Review to Azure Queue for Email Sendgrid
            await outputQueueEmail.AddAsync(newContact);

            log.Info($"New Contact Record Added... Just got added to Queue Storage");


            return newContact != null
                ? (ActionResult)new OkObjectResult(newContact)
                : new BadRequestObjectResult("Please pass a valid Contact Individual JSON Payload in the request body");
        }
        /*
        [FunctionName("GetContacts")]
        public static IActionResult GetContacts([HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "session")] [FromUri] string req,
        [CosmosDB(
            databaseName: "FabianSessions",
            collectionName: "Contacts",
            ConnectionStringSetting = "FabsSessionCosmosDBConn",
            SqlQuery = "SELECT * FROM c order by c._ts desc")] IEnumerable<Contact> allContacts,
         TraceWriter log)
        {
            log.Info("Getting all of  Contacts...");
            return new OkObjectResult(allContacts);
        }
        */

    }
}
