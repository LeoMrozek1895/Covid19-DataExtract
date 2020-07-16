using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataExtract
{
    /// <summary>
    /// Class that is the data structure of the JSON data returned from World.Data. 
    /// </summary>
	public partial class Covid19Data
    {
        [JsonProperty("head")]
        public Head Head { get; set; }

        [JsonProperty("metadata")]
        public List<Metadatum> Metadata { get; set; }

        [JsonProperty("results")]
        public Results Results { get; set; }
    }

    public partial class Head
    {
        [JsonProperty("vars")]
        public List<string> Vars { get; set; }
    }

    public partial class Metadatum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dataType")]
        public Uri DataType { get; set; }

        [JsonProperty("formatString", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatString { get; set; }
    }

    public partial class Results
    {
        [JsonProperty("bindings")]
        public List<Binding> Bindings { get; set; }
    }

    public partial class Binding
    {
        [JsonProperty("case_type")]
        public CaseType CaseType { get; set; }

        [JsonProperty("cases")]
        public Cases Cases { get; set; }

        [JsonProperty("difference")]
        public Cases Difference { get; set; }

        [JsonProperty("date")]
        public Cases Date { get; set; }

        [JsonProperty("country_region")]
        public CaseType CountryRegion { get; set; }

        [JsonProperty("province_state")]
        public CaseType ProvinceState { get; set; }

        [JsonProperty("lat")]
        public Cases Lat { get; set; }

        [JsonProperty("long")]
        public Cases Long { get; set; }

        [JsonProperty("location")]
        public Cases Location { get; set; }

        [JsonProperty("prep_flow_runtime")]
        public Cases PrepFlowRuntime { get; set; }

        [JsonProperty("table_names")]
        public CaseType TableNames { get; set; }
    }

    public partial class CaseType
    {
        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Cases
    {
        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("datatype")]
        public Uri Datatype { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public enum TypeEnum { Literal };

    public partial class Covid19Data
    {
        public static Covid19Data FromJson(string json) => JsonConvert.DeserializeObject<Covid19Data>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Covid19Data self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "literal")
            {
                return TypeEnum.Literal;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.Literal)
            {
                serializer.Serialize(writer, "literal");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}

