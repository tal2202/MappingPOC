using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public interface IContext
{
    JToken Current { get; }
    JToken Root { get; }
    IContext Push(JToken newContext);
    IContext Rewind(int offset);
    IContext Copy();
}