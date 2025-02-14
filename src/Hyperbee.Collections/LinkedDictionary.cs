using System.Collections;
using System.Collections.Concurrent;

namespace Hyperbee.Collections;

// a dictionary comprised of a stack of dictionaries

public enum LinkedNode
{
    Current,
    Single,
    All
}

public readonly record struct LinkedDictionaryNode<TKey, TValue>
{
    public string Name { get; init; }
    public IDictionary<TKey, TValue> Dictionary { get; init; }
}

public interface ILinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    IEqualityComparer<TKey> Comparer { get; }

    string Name { get; }

    IEnumerable<LinkedDictionaryNode<TKey, TValue>> Nodes();
    IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( LinkedNode linkedNode = LinkedNode.Single );
    IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( LinkedNode linkedNode, Predicate<KeyValuePair<TKey, TValue>> filter );
    IEnumerable<TKey> EnumerateKeys( LinkedNode linkedNode = LinkedNode.Single );
    IEnumerable<TValue> EnumerateValues( LinkedNode linkedNode = LinkedNode.Single );

    TValue this[LinkedNode linkedNode, TKey key] { set; }
    bool ContainsKey( LinkedNode linkedNode, TKey key );
    void Add( LinkedNode linkedNode, TKey key, TValue value );
    void Clear( LinkedNode linkedNode );
    bool Remove( LinkedNode linkedNode, TKey key );

    void Push( IEnumerable<KeyValuePair<TKey, TValue>> collection = default );
    void Push( string name, IEnumerable<KeyValuePair<TKey, TValue>> collection = default );
    LinkedDictionaryNode<TKey, TValue> Pop();
    bool TryPop( out LinkedDictionaryNode<TKey, TValue> node );
}

public class LinkedDictionary<TKey, TValue> : ILinkedDictionary<TKey, TValue>
{
    private readonly ConcurrentStack<LinkedDictionaryNode<TKey, TValue>> _nodes = new();

    public IEqualityComparer<TKey> Comparer { get; }

    // ctors

    public LinkedDictionary()
        : this( default( IEqualityComparer<TKey> ) )
    {
    }

    public LinkedDictionary( IEqualityComparer<TKey> comparer )
    {
        Comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    public LinkedDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection )
        : this( collection, null )
    {
    }

    public LinkedDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer )
    {
        Comparer = comparer ?? EqualityComparer<TKey>.Default;

        if ( collection != null )
            Push( collection );
    }

    public LinkedDictionary( ILinkedDictionary<TKey, TValue> inner )
        : this( inner, null )
    {
    }

    public LinkedDictionary( ILinkedDictionary<TKey, TValue> inner, IEnumerable<KeyValuePair<TKey, TValue>> collection )
    {
        ArgumentNullException.ThrowIfNull( inner );

        Comparer = inner.Comparer;
        _nodes = new ConcurrentStack<LinkedDictionaryNode<TKey, TValue>>( inner.Nodes() );

        if ( collection != null )
            Push( collection );
    }

    // Stack

    public void Push( IEnumerable<KeyValuePair<TKey, TValue>> collection = default )
    {
        Push( null, collection );
    }

    public void Push( string name, IEnumerable<KeyValuePair<TKey, TValue>> collection = default )
    {
        var dictionary = collection == null
            ? new ConcurrentDictionary<TKey, TValue>( Comparer )
            : new ConcurrentDictionary<TKey, TValue>( collection, Comparer );

        var newNode = new LinkedDictionaryNode<TKey, TValue>
        {
            Name = name ?? Guid.NewGuid().ToString(),
            Dictionary = dictionary
        };

        _nodes.Push( newNode );
    }

    public LinkedDictionaryNode<TKey, TValue> Pop()
    {
        TryPop( out var node );
        return node;
    }

    public bool TryPop( out LinkedDictionaryNode<TKey, TValue> node )
    {
        return _nodes.TryPop( out node );
    }

    // Counting

    public int CountKeys( LinkedNode linkedNode )
    {
        return linkedNode switch
        {
            LinkedNode.Current => _nodes.TryPeek( out var top ) ? top.Dictionary.Count : 0,
            LinkedNode.Single => GetUniqueCount(),
            LinkedNode.All => GetTotalCount(),
            _ => throw new ArgumentOutOfRangeException( nameof( linkedNode ) )
        };
    }

    public int CountKeys( LinkedNode linkedNode, Func<KeyValuePair<TKey, TValue>, bool> predicate )
    {
        ArgumentNullException.ThrowIfNull( predicate );

        return linkedNode switch
        {
            LinkedNode.Current => _nodes.TryPeek( out var top ) ? top.Dictionary.Count( predicate ) : 0,
            LinkedNode.Single => GetUniqueCount( predicate ),
            LinkedNode.All => GetTotalCount( predicate ),
            _ => throw new ArgumentOutOfRangeException( nameof( linkedNode ) )
        };
    }

    private int GetTotalCount()
    {
        return _nodes.Sum( node => node.Dictionary.Count );
    }

    private int GetTotalCount( Func<KeyValuePair<TKey, TValue>, bool> predicate )
    {
        return _nodes.Sum( node => node.Dictionary.Count( predicate ) );
    }

    private int GetUniqueCount()
    {
        var keys = new HashSet<TKey>( Comparer );

        foreach ( var node in _nodes )
        {
            foreach ( var key in node.Dictionary.Keys )
            {
                keys.Add( key );
            }
        }

        return keys.Count;
    }

    private int GetUniqueCount( Func<KeyValuePair<TKey, TValue>, bool> predicate )
    {
        var keys = new HashSet<TKey>( Comparer );

        foreach ( var node in _nodes )
        {
            foreach ( var pair in node.Dictionary )
            {
                if ( predicate( pair ) )
                    keys.Add( pair.Key );
            }
        }

        return keys.Count;
    }

    // ILinkedDictionary

    public string Name => _nodes.TryPeek( out var current ) ? current.Name : throw new InvalidOperationException( "No nodes available." );

    public TValue this[LinkedNode linkedNode, TKey key]
    {
        set
        {
            // support both 'let' and 'set' style assignments
            //
            //  'set' will assign value to the nearest existing key, or to the current node if no key is found. 
            //  'let' will assign value to the current node dictionary.

            if ( linkedNode != LinkedNode.Current )
            {
                // find and set if exists in an inner node
                foreach ( var node in _nodes )
                {
                    if ( !node.Dictionary.ContainsKey( key ) )
                        continue;

                    node.Dictionary[key] = value;

                    if ( linkedNode == LinkedNode.Single )
                        return;
                }
            }

            // set in current node

            if ( !_nodes.TryPeek( out var current ) )
            {
                throw new InvalidOperationException( "No nodes available to set the value." );
            }

            current.Dictionary[key] = value;
        }
    }

    public bool ContainsKey( LinkedNode linkedNode, TKey key )
    {
        return linkedNode switch { 
            LinkedNode.All => _nodes.All( node => node.Dictionary.ContainsKey( key ) ),
            LinkedNode.Single => _nodes.Any( node => node.Dictionary.ContainsKey( key ) ),
            LinkedNode.Current => _nodes.TryPeek( out var current ) && current.Dictionary.ContainsKey( key ),
            _ => false
        };
    }

    public void Add( LinkedNode linkedNode, TKey key, TValue value )
    {
        if ( ContainsKey( linkedNode, key ) )
            throw new ArgumentException( "Key already exists." );

        this[linkedNode, key] = value;
    }

    public IEnumerable<LinkedDictionaryNode<TKey, TValue>> Nodes() => _nodes;

    public IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( LinkedNode linkedNode = LinkedNode.Single ) => EnumerateItems( linkedNode, null );

    public IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( LinkedNode linkedNode, Predicate<KeyValuePair<TKey, TValue>> filter )
    {
        var keys = linkedNode == LinkedNode.Single ? new HashSet<TKey>( Comparer ) : null;

        foreach ( var node in _nodes )
        {
            foreach ( var pair in node.Dictionary )
            {
                if ( filter != null && !filter( pair ) )
                    continue;

                if ( linkedNode == LinkedNode.Single )
                {
                    if ( keys!.Contains( pair.Key ) )
                        continue;

                    keys.Add( pair.Key );
                }

                yield return pair;
            }

            if ( linkedNode == LinkedNode.Current )
                break;
        }
    }

    public IEnumerable<TKey> EnumerateKeys( LinkedNode linkedNode = LinkedNode.Single ) => EnumerateItems( linkedNode ).Select( kvp => kvp.Key );

    public IEnumerable<TValue> EnumerateValues( LinkedNode linkedNode = LinkedNode.Single ) => EnumerateItems( linkedNode ).Select( kvp => kvp.Value );

    public void Clear( LinkedNode linkedNode )
    {
        switch ( linkedNode )
        {
            case LinkedNode.All:
                {
                    foreach ( var node in _nodes )
                    {
                        node.Dictionary.Clear();
                    }

                    break;
                }

            case LinkedNode.Current:
                {
                    if ( _nodes.TryPeek( out var node ) )
                    {
                        node.Dictionary.Clear();
                    }

                    break;
                }

            case LinkedNode.Single:
                {
                    throw new NotSupportedException( "Clearing values by closest key is not supported." );
                }
        }
    }

    public bool Remove( LinkedNode linkedNode, TKey key )
    {
        var result = false;

        if ( linkedNode == LinkedNode.Current )
        {
            if ( _nodes.TryPeek( out var node ) )
            {
                result = node.Dictionary.Remove( key );
            }
            return result;
        }

        foreach ( var node in _nodes )
        {
            if ( !node.Dictionary.Remove( key ) )
                continue;

            result = true;

            if ( linkedNode == LinkedNode.Single )
                break;
        }

        return result;
    }

    // IDictionary

    public int Count => CountKeys( LinkedNode.Single );

    public TValue this[TKey key]
    {
        get
        {
            if ( !TryGetValue( key, out var result ) )
                throw new KeyNotFoundException();

            return result;
        }

        set => this[LinkedNode.Current, key] = value;
    }

    public bool IsReadOnly => false;

    public void Add( TKey key, TValue value )
    {
        if ( ContainsKey( LinkedNode.Current, key ) )
            throw new ArgumentException( "Key already exists." );

        this[LinkedNode.Current, key] = value;
    }

    public void Clear() => _nodes.Clear();

    public bool ContainsKey( TKey key ) => ContainsKey( LinkedNode.Single, key );

    public bool Remove( TKey key ) => Remove( LinkedNode.Current, key );

    public bool TryGetValue( TKey key, out TValue value )
    {
        foreach ( var node in _nodes )
        {
            if ( node.Dictionary.TryGetValue( key, out value ) )
                return true;
        }

        value = default;
        return false;
    }

    // ICollection

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => EnumerateItems().Select( kvp => kvp.Key ).ToArray();
    ICollection<TValue> IDictionary<TKey, TValue>.Values => EnumerateItems().Select( kvp => kvp.Value ).ToArray();
    void ICollection<KeyValuePair<TKey, TValue>>.Add( KeyValuePair<TKey, TValue> item ) => Add( item.Key, item.Value );

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains( KeyValuePair<TKey, TValue> item )
    {
        foreach ( var node in _nodes )
        {
            if ( node.Dictionary.TryGetValue( item.Key, out var value ) && EqualityComparer<TValue>.Default.Equals( value, item.Value ) )
                return true;
        }
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
    {
        ArgumentNullException.ThrowIfNull( array, nameof( array ) );

        if ( arrayIndex < 0 || arrayIndex > array.Length )
            throw new ArgumentOutOfRangeException( nameof( arrayIndex ) );

        if ( array.Length - arrayIndex < CountKeys( LinkedNode.All ) )
            throw new ArgumentException( "Insufficient space in the target array." );

        foreach ( var pair in EnumerateItems( LinkedNode.All ) )
        {
            array[arrayIndex++] = pair;
        }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item )
    {
        foreach ( var node in _nodes )
        {
            if ( !node.Dictionary.TryGetValue( item.Key, out var value ) || !EqualityComparer<TValue>.Default.Equals( value, item.Value ) )
                continue;

            node.Dictionary.Remove( item.Key );
            return true;
        }

        return false;
    }

    // IEnumerable

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => EnumerateItems( LinkedNode.All ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
