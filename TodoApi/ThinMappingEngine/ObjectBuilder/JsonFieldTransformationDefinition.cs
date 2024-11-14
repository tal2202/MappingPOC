using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;

namespace Cdp.Mapping.Model
{
    public enum MissingSourceValueHandling
    {
        DefaultTargetValue,
        Ignore,
        Fail
    }

    [Serializable]
    [DebuggerDisplay("Type={Type} , MissingSource={MissingSourceValueHandling.ToString()}")]
    public class JsonFieldTransformationDefinition
    {
        public JsonFieldTransformationDefinition()
        {

        }
        public JsonFieldTransformationDefinition(string type, MissingSourceValueHandling missingSourceValueHandling, object defaultTargetValue = null, JObject configuration = null)
        {
            Type = type;
            MissingSourceValueHandling = missingSourceValueHandling;
            DefaultTargetValue = defaultTargetValue;
            Configuration = configuration;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("missingSourceValueHandling"), JsonConverter(typeof(StringEnumConverter))]
        public MissingSourceValueHandling MissingSourceValueHandling { get; set; }

        [JsonProperty("defaultTargetValue")]
        public object DefaultTargetValue { get; set; }

        [JsonProperty("configuration")]
        public JObject Configuration { get; set; }
    }
}