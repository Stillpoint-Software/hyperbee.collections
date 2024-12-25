using System.Collections;
using System.Collections.Concurrent;

namespace Hyperbee.Collections;

// a dictionary comprised of a stack of dictionaries

public enum KeyScope
{
    Current,
    Closest,
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

    IEnumerable<LinkedDictionaryNode<TKey, TValue>> Scopes();
    IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( KeyScope keyScope = KeyScope.Closest );
    IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( KeyScope keyScope, Predicate<KeyValuePair<TKey, TValue>> filter );
    IEnumerable<TKey> EnumerateKeys( KeyScope keyScope = KeyScope.Closest );
    IEnumerable<TValue> EnumerateValues( KeyScope keyScope = KeyScope.Closest );

    TValue this[TKey key, KeyScope keyScope] { set; }
    void ClearValues( KeyScope keyScope );
    bool Remove( TKey key, KeyScope keyScope );

    void Push( IEnumerable<KeyValuePair<TKey, TValue>> collection = default );
    void Push( string name, IEnumerable<KeyValuePair<TKey, TValue>> collection = default );
    LinkedDictionaryNode<TKey, TValue> Pop();
    bool TryPop( out LinkedDictionaryNode<TKey, TValue> scope );
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
        _nodes = new ConcurrentStack<LinkedDictionaryNode<TKey, TValue>>( inner.Scopes() );

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
        TryPop( out var scope );
        return scope;
    }

    public bool TryPop( out LinkedDictionaryNode<TKey, TValue> scope )
    {
        return _nodes.TryPop( out scope );
    }

    // Counting

    public int CountKeys( KeyScope keyScope )
    {
        return keyScope switch
        {
            KeyScope.Current => _nodes.TryPeek( out var top ) ? top.Dictionary.Count : 0,
            KeyScope.Closest => GetUniqueCount(),
            KeyScope.All => GetTotalCount(),
            _ => throw new ArgumentOutOfRangeException( nameof( keyScope ) )
        };
    }

    public int CountKeys( KeyScope keyScope, Func<KeyValuePair<TKey, TValue>, bool> predicate )
    {
        ArgumentNullException.ThrowIfNull( predicate );

        return keyScope switch
        {
            KeyScope.Current => _nodes.TryPeek( out var top ) ? top.Dictionary.Count( predicate ) : 0,
            KeyScope.Closest => GetUniqueCount( predicate ),
            KeyScope.All => GetTotalCount( predicate ),
            _ => throw new ArgumentOutOfRangeException( nameof( keyScope ) )
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

    public string Name => _nodes.TryPeek( out var top ) ? top.Name : throw new InvalidOperationException( "No scopes available." );

    public TValue this[TKey key, KeyScope keyScope]
    {
        set
        {
            // support both 'let' and 'set' style assignments
            //
            //  'set' will assign value to the nearest existing key, or to the current node if no key is found. 
            //  'let' will assign value to the current node dictionary.

            if ( keyScope != KeyScope.Current )
            {
                // find and set if exists in an inner node
                foreach ( var scope in _nodes )
                {
                    if ( !scope.Dictionary.ContainsKey( key ) )
                        continue;

                    scope.Dictionary[key] = value;
                    return;
                }
            }

            // set in current node

            if ( !_nodes.TryPeek( out var current ) )
            {
                throw new InvalidOperationException( "No scopes available to set the value." );
            }

            current.Dictionary[key] = value;
        }
    }

    public IEnumerable<LinkedDictionaryNode<TKey, TValue>> Scopes() => _nodes;

    public IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( KeyScope keyScope = KeyScope.Closest ) => EnumerateItems( keyScope, null );

    public IEnumerable<KeyValuePair<TKey, TValue>> EnumerateItems( KeyScope keyScope, Predicate<KeyValuePair<TKey, TValue>> filter )
    {
        var keys = keyScope == KeyScope.Closest ? new HashSet<TKey>( Comparer ) : null;

        foreach ( var scope in _nodes )
        {
            foreach ( var pair in scope.Dictionary )
            {
                if ( filter != null && !filter( pair ) )
                    continue;

                if ( keyScope == KeyScope.Closest )
                {
                    if ( keys!.Contains( pair.Key ) )
                        continue;

                    keys.Add( pair.Key );
                }

                yield return pair;
            }

            if ( keyScope == KeyScope.Current )
                break;
        }
    }

    public IEnumerable<TKey> EnumerateKeys( KeyScope keyScope = KeyScope.Closest ) => EnumerateItems( keyScope ).Select( kvp => kvp.Key );

    public IEnumerable<TValue> EnumerateValues( KeyScope keyScope = KeyScope.Closest ) => EnumerateItems( keyScope ).Select( kvp => kvp.Value );

    public void ClearValues( KeyScope options )
    {
        switch ( options )
        {
            case KeyScope.All:
                {
                    foreach ( var node in _nodes )
                    {
                        node.Dictionary.Clear();
                    }

                    break;
                }

            case KeyScope.Current:
                {
                    if ( _nodes.TryPeek( out var node ) )
                    {
                        node.Dictionary.Clear();
                    }

                    break;
                }

            case KeyScope.Closest:
                {
                    throw new NotSupportedException( "Clearing values by closest key is not supported." );
                }
        }
    }

    public bool Remove( TKey key, KeyScope keyScope )
    {
        var result = false;

        foreach ( var scope in _nodes )
        {
            if ( !scope.Dictionary.Remove( key ) )
                continue;

            result = true;

            if ( keyScope == KeyScope.Closest )
                break;
        }

        return result;
    }

    // IDictionary

    public int Count => CountKeys( KeyScope.Closest );

    public TValue this[TKey key]
    {
        get
        {
            if ( !TryGetValue( key, out var result ) )
                throw new KeyNotFoundException();

            return result;
        }

        set => this[key, KeyScope.Closest] = value;
    }

    public bool IsReadOnly => false;

    public void Add( TKey key, TValue value )
    {
        if ( ContainsKey( key ) )
            throw new ArgumentException( "Key already exists." );

        this[key, KeyScope.Closest] = value;
    }

    public void Clear() => _nodes.Clear();

    public bool ContainsKey( TKey key ) => _nodes.Any( scope => scope.Dictionary.ContainsKey( key ) );

    public bool Remove( TKey key ) => Remove( key, KeyScope.Closest );

    public bool TryGetValue( TKey key, out TValue value )
    {
        foreach ( var scope in _nodes )
        {
            if ( scope.Dictionary.TryGetValue( key, out value ) )
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
        foreach ( var scope in _nodes )
        {
            if ( scope.Dictionary.TryGetValue( item.Key, out var value ) && EqualityComparer<TValue>.Default.Equals( value, item.Value ) )
                return true;
        }
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
    {
        ArgumentNullException.ThrowIfNull( array, nameof( array ) );

        if ( arrayIndex < 0 || arrayIndex > array.Length )
            throw new ArgumentOutOfRangeException( nameof( arrayIndex ) );

        if ( array.Length - arrayIndex < CountKeys( KeyScope.All ) )
            throw new ArgumentException( "Insufficient space in the target array." );

        foreach ( var pair in EnumerateItems( KeyScope.All ) )
        {
            array[arrayIndex++] = pair;
        }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item )
    {
        foreach ( var scope in _nodes )
        {
            if ( !scope.Dictionary.TryGetValue( item.Key, out var value ) || !EqualityComparer<TValue>.Default.Equals( value, item.Value ) )
                continue;

            scope.Dictionary.Remove( item.Key );
            return true;
        }

        return false;
    }

    // IEnumerable

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => EnumerateItems( KeyScope.All ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
