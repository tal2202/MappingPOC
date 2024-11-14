using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cdp.Mapping.ObjectBuilder;

public class ObjectBuilder : IObjectBuilder
{
    private readonly IValueProviderEngine _valueProviderEngine;

    public ObjectBuilder(IValueProviderEngine valueProviderEngine)
    {
        _valueProviderEngine = valueProviderEngine;
    }
    
    public JToken Build(JSchema targetSchema, JToken dataSource)
    {
        JToken target;
        var context = new SimpleContext(dataSource);
        switch (targetSchema.Type)
        {
            case JSchemaType.Array:
                target = BuildArrayRecursive(targetSchema, context);
                break;
            case JSchemaType.Object:
                target = BuildObjectRecursive(targetSchema, context);
                break;
            default:
                throw new NotImplementedException();
        }

        return target;
    }

    private JArray BuildArrayRecursive(JSchema targetSchema, IContext context)
    {
        if (targetSchema.Default != null && targetSchema.Default is not JArray)
        {
            throw new ArgumentOutOfRangeException(nameof(targetSchema.Default));
        }
        
        if (targetSchema.Items.Count == 0)
        {
            return (targetSchema.Default as JArray) ?? new JArray();
        }

        var resultArray = targetSchema.Items.Count > 1
            ? BuildTuple(targetSchema, context)
            : BuildArray(targetSchema, context);

        if (resultArray.Count == 0 && targetSchema.Default != null)
        {
            return (targetSchema.Default as JArray)!;
        }
        
        return resultArray;
    }
    
    private JArray BuildArray(JSchema targetSchema, IContext context)
    {
        var itemSchema = targetSchema.Items.First();
    
        var target = new JArray();
        
        if (!targetSchema.ContainsCtxProvider())
        {
            var array = BuildToken(itemSchema, context);
            if (array != null)
            {
                target.Add(array);
            }
    
            return target;
        }

        var newContext = _valueProviderEngine.BuildContext(targetSchema.GetCtxProvider()!, context);

        if (newContext == null)
        {
            return target;
        }
        
        if (newContext.Current is not JArray arrayOfTokens)
        {
            throw new ArgumentOutOfRangeException(nameof(newContext));
        }

        foreach (var token in arrayOfTokens)
        {
            var item = BuildToken(itemSchema, context.Push(token));
            if (item != null)
            {
                target.Add(item);
            }
        }
    
        return target;
    }
    
    private JArray BuildTuple(JSchema targetSchema, IContext context)
    {
        var target = new JArray();

        if (targetSchema.ContainsCtxProvider())
        {
            var tupleContext = _valueProviderEngine.BuildContext(targetSchema.GetValueProvider(), context);
            if (tupleContext == null)
            {
                return target;
            }

            context = tupleContext;
        }
        
        foreach (var itemSchema in targetSchema.Items)
        {
            var token = BuildToken(itemSchema, context);
            target.Add(token ?? JValue.CreateNull());
        }

        return target;
    }

    private JToken BuildObjectRecursive(JSchema targetSchema, IContext context)
    {
        if (targetSchema.Default != null && targetSchema.Default is not JObject) 
        {
            throw new ArgumentOutOfRangeException(nameof(targetSchema.Default));
        }
        
        if (targetSchema.Properties.Count == 0)
        {
            return (targetSchema.Default as JObject) ?? context.Current;
        }
        
        var target = new JObject();
        foreach (var propertyName in targetSchema.Properties.Keys)
        {
            var schema = targetSchema.Properties[propertyName];
            var token = BuildToken(schema, context);
            if (token != null)
            {
                target[propertyName] = token;
            }
        }

        if (!target.HasValues && targetSchema.Default != null)
        {
            return (targetSchema.Default as JObject)!;
        }

        return target;
    }

    private JToken? BuildToken(JSchema schema, IContext context)
    {
        switch (schema.Type)
        {
            case JSchemaType.Array:
                var array = BuildArrayRecursive(schema, context);
                if (array != null)
                {
                    return array;
                }
                break;
                
            case JSchemaType.Object:
                return BuildObjectRecursive(schema, context);
                break;

            case JSchemaType.String:
            case JSchemaType.Number:
            case JSchemaType.Integer:
            case JSchemaType.Boolean:

                if (!schema.ContainsValueProvider())
                {
                    return schema.Default ?? throw new Exception("Stuff not mapped");
                }

                var valueProvider = schema.GetValueProvider();
                var value = _valueProviderEngine.ResolveValues(valueProvider, context);
                return value ?? schema.Default;
                break;
                
            default:
                throw new NotImplementedException();
        }

        return null;
    }   
}