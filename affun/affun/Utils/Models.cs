using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace affun.Utils
{

    public partial class OcrPayload
    {
        [JsonProperty("incomingname")]
        public string IncomingName { get; set; }

        [JsonProperty("contentload")]
        public string ContentLoad { get; set; }
    }
    public partial class FullCardInfo
    {
        [JsonProperty("Info")]
        public Info Info { get; set; }

        [JsonProperty("UnKnown")]
        public string[] UnKnown { get; set; }
    }

    public partial class Info
    {
        [JsonProperty("Company")]
        public object Company { get; set; }

        [JsonProperty("Name")]
        public object Name { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("CityStateZip")]
        public string CityStateZip { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Website")]
        public string Website { get; set; }

        [JsonProperty("Facebook")]
        public object Facebook { get; set; }

        [JsonProperty("Twitter")]
        public object Twitter { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Fax")]
        public string Fax { get; set; }

        [JsonProperty("Cell")]
        public object Cell { get; set; }

        [JsonProperty("Total")]
        public object Total { get; set; }

        [JsonProperty("Date")]
        public object Date { get; set; }
    }

    public class FullCardInfoTable : FullCardInfo
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string CardOwnerEAddr { get; set; }
    }

    public partial class FullCardInfo
    {
        public static FullCardInfo FromJson(string json) => JsonConvert.DeserializeObject<FullCardInfo>(json, affun.Utils.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FullCardInfo self) => JsonConvert.SerializeObject(self, affun.Utils.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    // For OCR Data

    public partial class FullOcrInfo
    {
        [JsonProperty("sourcelanguage")]
        public string SourceLanguage { get; set; }

        [JsonProperty("sourcetextAngle")]
        public long SourceTextAngle { get; set; }

        [JsonProperty("sourceorientation")]
        public string sourceOrientation { get; set; }

        [JsonProperty("sourceregions")]
        public SourceRegion[] SourceRegions { get; set; }
    }

    public class FullOCRInfoTable : FullOcrInfo
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }

    public partial class SourceRegion
    {
        [JsonProperty("sourceboundingBox")]
        public string SourceBoundingBox { get; set; }

        [JsonProperty("sourcelines")]
        public SourceLine[] SourceLines { get; set; }
    }

    public partial class SourceLine
    {
        [JsonProperty("sourceboundingBox")]
        public string SourceBoundingBox { get; set; }

        [JsonProperty("sourcewords")]
        public SourceWord[] SourceWords { get; set; }
    }

    public partial class SourceWord
    {
        [JsonProperty("sourceboundingBox")]
        public string SourceBoundingBox { get; set; }

        [JsonProperty("sourcetext")]
        public string SourceText { get; set; }
    }

        public partial class UnknownKnown
    {
        [JsonProperty("documents")]
        public Document[] Documents { get; set; }

        [JsonProperty("errors")]
        public object[] Errors { get; set; }
    }

    public class RawDocTableInfo : UnknownKnown
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public partial class Document
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("entities")]
        public Entity[] Entities { get; set; }
    }

    public partial class Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("matches")]
        public Match[] Matches { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("subType")]
        public object SubType { get; set; }

        [JsonProperty("wikipediaLanguage", NullValueHandling = NullValueHandling.Ignore)]
        public string WikipediaLanguage { get; set; }

        [JsonProperty("wikipediaId", NullValueHandling = NullValueHandling.Ignore)]
        public string WikipediaId { get; set; }

        [JsonProperty("wikipediaUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri WikipediaUrl { get; set; }

        [JsonProperty("bingId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? BingId { get; set; }
    }

    public partial class Match
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("length")]
        public long Length { get; set; }
        [JsonProperty("wikipediaScore")]
        public double? WikipediaScore { get; set; }

        [JsonProperty("entityTypeScore")]
        public double? EntityTypeScore { get; set; }
    }

    public partial class MailChimpSub
    {
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("merge_fields")]
        public MergeFields MergeFields { get; set; }
    }

    public partial class MergeFields
    {
        [JsonProperty("FNAME")]
        public string Fname { get; set; }

        [JsonProperty("LNAME")]
        public string Lname { get; set; }
    }

    public partial class SFDCLeads
    {
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("leadsource")]
        public string LeadSource { get; set; } = "Web";
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("mobilephone")]
        public string MobilePhone { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

}
