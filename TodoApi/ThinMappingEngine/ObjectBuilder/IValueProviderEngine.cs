using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public interface IValueProviderEngine
{
    IContext? BuildContext(ValueProvider? valueProvider, IContext context);
    JToken? ResolveValues(ValueProvider? valueProvider, IContext context);
}