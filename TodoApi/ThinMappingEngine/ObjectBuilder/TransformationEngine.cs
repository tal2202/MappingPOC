using Cdp.Mapping;
using Cdp.Mapping.Model;
using Newtonsoft.Json.Linq;

namespace TodoApi.ThinMappingEngine.ObjectBuilder
{
    public class TransformationEngine : ISimplifiedJTokenTransformationEngine
    {
        public TransformationEngine()
        {
           
        }

        public JTokenTransformResult Transform(JToken token, IList<JsonFieldTransformationDefinition> transformationDefinitions)
        {
            throw new NotImplementedException("Transfomations are not supported in the thin engine");
        }

        public JTokenTransformResult Transform(JToken token, JsonFieldTransformationDefinition transformationDefinition)
        {
            throw new NotImplementedException("Transfomations are not supported in the thin engine");
        }
    }
}
