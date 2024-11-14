using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cdp.Mapping.ObjectBuilder;

public interface IObjectBuilder
{
    JToken Build(JSchema targetSchema, JToken dataSource);
}
