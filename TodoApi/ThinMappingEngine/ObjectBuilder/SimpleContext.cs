using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public class SimpleContext : IContext
{
    public JToken Current { get; private set; }
    public JToken Root { get; }

    public SimpleContext(JToken initialContext)
    {
        Root = initialContext;
        Current = initialContext;
    }

    private SimpleContext(IContext other)
    {
        Root = other.Root;
        Current = other.Current;
    }
    
    public IContext Push(JToken newContext)
    {
        return new SimpleContext(this)
        {
            Current = newContext
        };
    }

    public IContext Rewind(int offset)
    {
        // SimpleContext does not support rewind
        throw new System.NotImplementedException();
    }

    public IContext Copy()
    {
        return new SimpleContext(this);
    }
}