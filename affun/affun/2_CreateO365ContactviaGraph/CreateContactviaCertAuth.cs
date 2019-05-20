using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using affun.Utils;

namespace sc._7_NewO365Contat
{
    public static class CreateContactviaCertAuth
    {
        private static GraphServiceClient _graphServiceClient;

        [FunctionName("CreateContactviaCertAuth")]
        public static async Task AddNewOffice365Contact([QueueTrigger("scnewgraphcontact", Connection = "AzureWebJobsStorage")]
                            FullCardInfoTable myQueueItem, 
                            ILogger log)
        {
            var config = LoadAppSettings();
            if (null == config)
            {
                log.LogInformation("Missing or invalid local.settings.json file.");
                return;
            }

            GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);
            try
            {
                if (graphClient != null)
                {
                    var fullname = myQueueItem.Info.Name.ToString();
                    var tmpFullName = fullname.Split(new char[] { ' ' });
                    var fn = tmpFullName[0];
                    var ln = tmpFullName[1];
                    string newLeadRaw = JsonConvert.SerializeObject(myQueueItem);

                    var businessPhonesList = new List<String>();
                    businessPhonesList.Add(myQueueItem.Info.Phone?.ToString());
                    var emailAddresses = new EmailAddress
                    {
                        Address = myQueueItem.Info.Email?.ToString(),
                        Name = myQueueItem.Info.Name?.ToString(),
                    };
                    var bizAddresses = new PhysicalAddress
                    {
                        City = "ToDo Item",
                        CountryOrRegion = "US",
                        PostalCode = "0000",
                        State = "Md",
                        Street = "ToDoItem"
                    };
                    var emailAddressesList = new List<EmailAddress>();
                    emailAddressesList.Add(emailAddresses);

                    var contact = new Contact
                    {
                        CompanyName = myQueueItem.Info.Company?.ToString(),
                        //BusinessAddress = bizAddresses,
                        JobTitle = myQueueItem.Info.Title?.ToString(),
                        //Manager = "Dan Holme",
                        //NickName = "Bill",
                        GivenName = fn,
                        Surname = ln,
                        PersonalNotes = newLeadRaw,
                        EmailAddresses = emailAddressesList,
                        BusinessPhones = businessPhonesList,
                    };
                    //graphClient.Me.Contacts
                    var NewlyCreatedContact = await graphClient.Users["65926cde-172b-47fd-948c-bd734718900f"].Contacts
                       .Request()
                       .AddAsync(contact);         
                    string newContact = JsonConvert.SerializeObject(NewlyCreatedContact);
                    Console.WriteLine($"Added Contact: {newContact}");

                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                //.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .SetBasePath(@"D:\home\site\wwwroot\")
                .AddJsonFile("certappsettings.json", false, true)
                .Build();

                // Validate required settings
                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["applicationSecret"]) ||
                    string.IsNullOrEmpty(config["redirectUri"]) ||
                    string.IsNullOrEmpty(config["tenantId"]) ||
                    string.IsNullOrEmpty(config["domain"]))
                {
                    return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            //this specific scope means that application will default to what is defined in the application registration rather than using dynamic scopes
            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithRedirectUri(redirectUri)
                                                    .WithClientSecret(clientSecret)
                                                    .Build();
            return new MsalAuthenticationProvider(cca, scopes.ToArray());
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        public async static Task CreateNewO365Contact(GraphServiceClient cl)
        {
            if (cl != null)
            {
                var businessPhonesList = new List<String>();
                businessPhonesList.Add("+1 425 555 0102");

                var emailAddresses = new EmailAddress
                {
                    Address = "will.bar1@microsoft.com",
                    Name = "Will Bar1",
                };

                var bizAddresses = new PhysicalAddress
                {
                    City = "Bellevue",
                    CountryOrRegion = "US",
                    PostalCode = "80987",
                    State = "Wa",
                    Street = "1234 Pullwah Lane"
                };

                var emailAddressesList = new List<EmailAddress>();
                emailAddressesList.Add(emailAddresses);

                var contact = new Contact
                {
                    CompanyName = "Baer Enterprises",
                    BusinessAddress = bizAddresses,
                    JobTitle = "Senior Program Manager",
                    Manager = "Dan Holme",
                    NickName = "Bill",
                    GivenName = "Will",
                    Surname = "Bar",
                    PersonalNotes = "blah blah blah",
                    EmailAddresses = emailAddressesList,
                    BusinessPhones = businessPhonesList,
                };

                //graphClient.Me.Contacts
                var NewlyCreatedContact = await cl.Users["65926cde-172b-47fd-948c-bd734718900f"].Contacts
                   .Request()
                   .AddAsync(contact);

                string newContact = JsonConvert.SerializeObject(NewlyCreatedContact);
                Console.WriteLine($"Added Contact: {newContact}");

            }
        }
    }

}
