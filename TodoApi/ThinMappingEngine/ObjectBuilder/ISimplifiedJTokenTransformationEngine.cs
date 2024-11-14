using System.Collections.Generic;
using Cdp.Mapping.Model;
using Newtonsoft.Json.Linq;

namespace Cdp.Mapping;

public interface ISimplifiedJTokenTransformationEngine
{
    JTokenTransformResult Transform(JToken token, IList<JsonFieldTransformationDefinition> transformationDefinitions);
    JTokenTransformResult Transform(JToken token, JsonFieldTransformationDefinition transformationDefinition);
}
