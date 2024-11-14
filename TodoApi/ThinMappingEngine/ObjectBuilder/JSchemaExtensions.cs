using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cdp.Mapping.ObjectBuilder;

public static class JSchemaExtensions
{
    private const string ValueProviderExtensionKey = "valueProvider";
    private const string ContextProviderExtensionKey = "ctxProvider";
    public static ValueProvider? GetValueProvider(this JSchema jSchema)
    {
        if (jSchema.ContainsValueProvider())
        {
            return jSchema.ExtensionData[ValueProviderExtensionKey].ToObject<ValueProvider>();
        }

        return null;
    }
    public static ValueProvider? GetCtxProvider(this JSchema jSchema)
    {
        if (jSchema.ContainsCtxProvider())
        {
            return jSchema.ExtensionData[ContextProviderExtensionKey].ToObject<ValueProvider>();
        }

        return null;
    }

    public static bool ContainsValueProvider(this JSchema jSchema)
    {
        return jSchema.ExtensionData.ContainsKey(ValueProviderExtensionKey)
               && jSchema.ExtensionData[ValueProviderExtensionKey].Type != JTokenType.Null;
    }

    public static bool ContainsCtxProvider(this JSchema jSchema)
    {
        return jSchema.ExtensionData.ContainsKey(ContextProviderExtensionKey)
               && jSchema.ExtensionData[ContextProviderExtensionKey].Type != JTokenType.Null;
    }



}