using System.Collections.Generic;
using Cdp.Mapping.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public class ValueProvider
{

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("pointer")]
    public string? Pointer { get; set; }
    
    [JsonProperty("transformations")]
    public IEnumerable<JsonFieldTransformationDefinition>? TransformationsDefinition { get; set; }
    [JsonProperty("ctxRewind")]
    public int? CtxRewind { get; set; }

    [JsonProperty("constantValue")]
    public JToken? ConstantValue { get; set; }

    [JsonProperty("isArray")]
    public bool? IsArray { get; set; }
    
    [JsonProperty("providers")]
    public IEnumerable<ValueProvider>? Providers { get; set; }


}
