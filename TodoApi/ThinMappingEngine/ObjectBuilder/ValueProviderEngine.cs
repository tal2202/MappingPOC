using System;
using System.Collections.Generic;
using System.Linq;
using Cdp.Mapping.Model;
using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public class ValueProviderEngine : IValueProviderEngine
{
    private readonly ISimplifiedJTokenTransformationEngine _transformationEngine;

    private static Func<JToken, string, JToken?> MultiTokenSelectorResolver =>
        (dataSource, selector) => new JArray(dataSource.SelectTokens(selector));

    private static Func<JToken, string, JToken?> SingleTokenSelectorResolver =>
        (dataSource, selector) => dataSource.SelectToken(selector);

    public ValueProviderEngine(ISimplifiedJTokenTransformationEngine transformationEngine)
    {
        _transformationEngine = transformationEngine;
    }
    
    public IContext? BuildContext(ValueProvider? valueProvider, IContext context)
    {
        return TraverseValueProvider(valueProvider, context);
    }
    
    public JToken? ResolveValues(ValueProvider? valueProvider, IContext context)
    {
        return TraverseValueProvider(valueProvider, context)?.Current;
    }

    private IContext? TraverseValueProvider(ValueProvider? valueProvider, IContext context)
    {
        if (valueProvider == null)
        {
            return context;
        }

        var localContext = context.Copy();
        
        var token = ResolveValue(valueProvider, localContext);
        if (token == null)
        {
            return null;
        }
            
        localContext = localContext.Push(token);
        
        if (valueProvider.TransformationsDefinition != null && valueProvider.TransformationsDefinition.Count() > 0 )
        {
            foreach (var transformation in valueProvider.TransformationsDefinition)
            {
                token = TransformToken(transformation, localContext.Current);
                localContext = localContext.Push(token);
            }

        }

        return context.Push(localContext.Current);

    }

    private JToken? ResolveValue(ValueProvider valueProvider, IContext context)
    {
        if (valueProvider.CtxRewind < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valueProvider.CtxRewind));
        }
        
        if (valueProvider.CtxRewind > 0)
        {
            context = context.Rewind((int)valueProvider.CtxRewind);
        }
        
        if (valueProvider.Type == "contextual")
        {
            if (valueProvider.IsArray == true)
            {
                return MultiTokenSelectorResolver(context.Current, valueProvider.Pointer!);
            }
            return SingleTokenSelectorResolver(context.Current, valueProvider.Pointer!);
        }

        if (valueProvider.Type == "absolute")
        {
            if (valueProvider.IsArray == true)
            {
                return MultiTokenSelectorResolver(context.Root, valueProvider.Pointer!);
            }
            return SingleTokenSelectorResolver(context.Root, valueProvider.Pointer!);
        }

        if (valueProvider.Type == "multi")
        {
            
            if (valueProvider.Providers.Count() == 0 || valueProvider.Providers == null)
            {
                throw new ArgumentOutOfRangeException(nameof(valueProvider.Type));
            }
            var providersResponse = new JArray();
            foreach (var provider in valueProvider.Providers!)
            {
                var providerResponse = ResolveValue(provider, context);
                var localContext = context.Copy();
                localContext = localContext.Push(providerResponse);

                if (provider.TransformationsDefinition != null && provider.TransformationsDefinition.Count() > 0)
                {
                    foreach (var transformation in provider.TransformationsDefinition)
                    {
                        var token = TransformToken(transformation, localContext.Current);
                        localContext = localContext.Push(token);
                    }

                }

                if (localContext.Current != null)
                {
                    providersResponse.Add(localContext.Current);
                }
            }
            //if (valueProvider.TransformationsDefinition!= null && valueProvider.TransformationsDefinition.Count() > 0)
            //{
            //    JToken transformationResult = providersResponse;
            //    foreach (var transformation in valueProvider.TransformationsDefinition)
            //    {
            //         transformationResult = TransformToken(transformation, transformationResult);
            //    }
            //    return transformationResult;
            //}
            return providersResponse;

            }



        //if (!string.IsNullOrEmpty(valueProvider.ContextualArraySelector))
        //{
        //    return MultiTokenSelectorResolver(context.Current, valueProvider.ContextualArraySelector!);
        //}

        //if (!string.IsNullOrEmpty(valueProvider.AbsoluteArraySelector))
        //{
        //    return MultiTokenSelectorResolver(context.Root, valueProvider.AbsoluteArraySelector!);
        //}

        //if (valueProvider.TupleBuilder != null)
        //{
        //    var resolvedValues = valueProvider.TupleBuilder
        //        .Select(itemValueProvider => TraverseValueProvider(itemValueProvider, context))
        //        .Select(itemValueProvider => itemValueProvider?.Current ?? JValue.CreateNull())
        //        .ToList();

        //    return new JArray(resolvedValues);
        //}



        if (valueProvider.Type == "constant")
        {
            return valueProvider.ConstantValue;
        }

        throw new ArgumentOutOfRangeException(nameof(valueProvider));
    }
    
    private  JToken TransformToken(JsonFieldTransformationDefinition transformationDefinition, JToken value)
    {
        var transformationResult =
            _transformationEngine.Transform(value, transformationDefinition);
        if (transformationResult.IsOk)
        {
            // validate type?
            return transformationResult.Token;
        }

        // TODO handle error
        throw new Exception();
    }
}