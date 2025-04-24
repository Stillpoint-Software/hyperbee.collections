using System.Collections;

namespace Hyperbee.Collections;

public interface IDisjointSet<T> : IReadOnlyCollection<T>
{
    public IEnumerable<T> Keys { get; }

    bool AreConnected( T item1, T item2 );
    void Clear();
    bool TryAdd( T item );
    bool TryAdd( IEnumerable<T> items );
    T Find( T item );
    IEnumerable<T> FindGroup( T item );
    bool Union( T item1, T item2 );
}

public class DisjointSet<T> : IDisjointSet<T>
{
    private readonly Dictionary<T, T> _parent;
    private readonly IEqualityComparer<T> _comparer;
    private readonly OnDisjointUnion _onDisjointUnion;

    public delegate void OnDisjointUnion( T representative, T retired );

    public DisjointSet( IEqualityComparer<T> comparer = null )
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _parent = new Dictionary<T, T>( _comparer );
    }

    public DisjointSet( OnDisjointUnion onDisjointUnion, IEqualityComparer<T> comparer = null )
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _parent = new Dictionary<T, T>( _comparer );
        _onDisjointUnion = onDisjointUnion;
    }

    public int Count => _parent.Count;

    public void Clear()
    {
        _parent.Clear();
    }

    public bool AreConnected( T item1, T item2 )
    {
        // Check if both items are in the same group by comparing their representatives
        return _comparer.Equals( Find( item1 ), Find( item2 ) );
    }

    public bool TryAdd( T item )
    {
        return _parent.TryAdd( item, item );
    }

    public bool TryAdd( IEnumerable<T> items )
    {
        if ( items == null )
            return false;

        using var enumerator = items.GetEnumerator();

        if ( !enumerator.MoveNext() )
            return false;

        // Use first item as the representative
        var root = enumerator.Current;

        var added = _parent.TryAdd( root, root );

        // Add the remaining items and union them with the representative
        while ( enumerator.MoveNext() )
        {
            var item = enumerator.Current;

            if ( _parent.TryAdd( item, root ) )
            {
                added = true;
            }
            else if ( !Equals( Find( item ), root ) ) // Avoid unnecessary Union if already in the same group
            {
                Union( root, item );
            }
        }

        return added;
    }

    public T Find( T item )
    {
        if ( !_parent.ContainsKey( item ) )
            throw new InvalidOperationException( $"Item '{item}' is not part of the set." );

        // Traverse up the tree to find the root
        var root = item;

        while ( true )
        {
            var parent = _parent[root];

            if ( _comparer.Equals( parent, root ) )
                break;

            // Path compression
            var grandparent = _parent[parent];
            _parent[root] = grandparent;

            root = parent;
        }

        return root;
    }

    public IEnumerable<T> FindGroup( T item )
    {
        // expensive
        var representative = Find( item );
        return _parent.Keys.Where( x => Find( x ).Equals( representative ) );
    }

    public bool Union( T item1, T item2 )
    {
        var root1 = Find( item1 );
        var root2 = Find( item2 );

        if ( _comparer.Equals( root1, root2 ) )
            return false; // Items were already in the same group

        _parent[root2] = root1;
        _onDisjointUnion?.Invoke( root1, root2 );

        return true;
    }

    public IEnumerable<T> Keys => _parent.Keys;

    public IEnumerator<T> GetEnumerator()
    {
        return _parent.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
