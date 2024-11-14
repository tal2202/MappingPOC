using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Cdp.Mapping.ObjectBuilder;

public class LinkedListContext : IContext
{
    private readonly LinkedList<JToken> _context;

    public LinkedListContext(JToken initialContext)
    {
        _context = new LinkedList<JToken>();
        _context.AddFirst(initialContext);
    }

    private LinkedListContext(IEnumerable<JToken> initialContext)
    {
        _context = new LinkedList<JToken>(initialContext);
    }
    
    private LinkedListContext(LinkedList<JToken> initialContext)
    {
        _context = initialContext;
    }
    
    private LinkedListContext(LinkedListContext other)
    {
        _context = new LinkedList<JToken>(other._context);
    }
    
    public JToken Current => _context.Last.Value;
    
    public JToken Root => _context.First.Value;
    
    public IContext Push(JToken newContext)
    {
        var context = new LinkedList<JToken>(_context);
        context.AddLast(newContext);
        return new LinkedListContext(context);
    }

    public IContext Rewind(int offset)
    {
        if (_context.Count < offset + 1)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        var newContext = new LinkedListContext(
            _context
                .Select(contextItem => contextItem)
                .Take(_context.Count - offset));

        return newContext;
    }

    public IContext Copy()
    {
        return new LinkedListContext(this);
    }
}