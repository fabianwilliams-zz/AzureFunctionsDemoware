using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FabianSpeakingData.shared
{
    public class Contact
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }
        [JsonProperty("tellMeAboutYou")]
        public string TellMeAboutYou { get; set; }
        [JsonProperty("twitterHandle")]
        public string TwitterHandle { get; set; }
        [JsonProperty("faceBookHandle")]
        public string FaceBookHandle { get; set; }
        [JsonProperty("linkedInHandle")]
        public string LinkedInHandle { get; set; }
        [JsonProperty("instagramHandle")]
        public string InstagramHandle { get; set; }
        [JsonProperty("githubHandle")]
        public string GitHubHandle { get; set; }
        [JsonProperty("whomDidIMeet")]
        public string WhomDidIMeet { get; set; } //not yet implemented.. If i want to scale this must be able to figure out who's vCard tosend.

    }

    public class InstaContact : Contact
    {
        [JsonProperty("uId")]
        public string UId { get; set; }
        [JsonProperty("locationLon")]
        public string LocationLon { get; set; }
        [JsonProperty("locationLat")]
        public string LocationLat { get; set; }
        [JsonProperty("streetName")]
        public string StreetName { get; set; }
        [JsonProperty("localityCity")]
        public string LocalityCity { get; set; }
        [JsonProperty("adminArea")]
        public string AdminArea { get; set; }
        [JsonProperty("homeCountry")]
        public string HomeCountry { get; set; }
        [JsonProperty("defaultPic")]
        public string DefaultPic { get; set; }
        [JsonProperty("verified")]
        public string Verified { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("isBusiness")]
        public string IsBusiness { get; set; }
        [JsonProperty("joined")]
        public string Joined { get; set; }
    }

    public class Result<T>
    {
        public Result(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }

    public class IncomingText
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
    public class OutgoingEmail
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class Selfie
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("photoBase64")]
        public string PhotoBase64 { get; set; }
    }

}

