using System.Collections;

namespace Hyperbee.Collections;

// a dictionary that preserves insertion order for enumerations

public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    TValue this[int index] { get; set; }

    void Insert( int index, TKey key, TValue value );
    int IndexOfKey( TKey key );
    void RemoveAt( int index );
}

public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
{
    private IDictionary<TKey, TValue> _dictionary;
    private IList<KeyValuePair<TKey, TValue>> _list;
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly int _initialCapacity;

    public OrderedDictionary()
        : this( 0, null )
    {
    }

    public OrderedDictionary( int capacity )
        : this( capacity, null )
    {
    }

    public OrderedDictionary( IEqualityComparer<TKey> comparer )
        : this( 0, comparer )
    {
    }

    public OrderedDictionary( int capacity, IEqualityComparer<TKey> comparer )
    {
        ArgumentOutOfRangeException.ThrowIfNegative( capacity );

        _initialCapacity = capacity;
        _comparer = comparer;
    }

    private IDictionary<TKey, TValue> ItemDictionary => _dictionary ??= new Dictionary<TKey, TValue>( _initialCapacity, _comparer );
    private IList<KeyValuePair<TKey, TValue>> ItemList => _list ??= new List<KeyValuePair<TKey, TValue>>( _initialCapacity );

    public TValue this[int index]
    {
        get => ItemList[index].Value;

        set
        {
            if ( index >= Count || index < 0 )
                throw new ArgumentOutOfRangeException( nameof( index ) );

            var key = ItemList[index].Key;

            ItemList[index] = new KeyValuePair<TKey, TValue>( key, value );
            ItemDictionary[key] = value;
        }
    }

    public TValue this[TKey key]
    {
        get => ItemDictionary[key];

        set
        {
            if ( ItemDictionary.ContainsKey( key ) )
            {
                ItemDictionary[key] = value;
                ItemList[IndexOfKey( key )] = new KeyValuePair<TKey, TValue>( key, value );
            }
            else
            {
                Add( key, value );
            }
        }
    }

    public void Add( TKey key, TValue value )
    {
        ItemDictionary.Add( key, value );
        ItemList.Add( new KeyValuePair<TKey, TValue>( key, value ) );
    }

    public void Clear()
    {
        ItemDictionary.Clear();
        ItemList.Clear();
    }

    public int Count => ItemList.Count;

    public bool ContainsKey( TKey key ) => ItemDictionary.ContainsKey( key );

    public bool IsReadOnly => false;

    public int IndexOfKey( TKey key )
    {
        if ( null == key )
            throw new ArgumentNullException( nameof( key ) );

        for ( var index = 0; index < ItemList.Count; index++ )
        {
            var key1 = ItemList[index].Key;

            if ( _comparer != null )
            {
                if ( _comparer.Equals( key1, key ) )
                    return index;
            }
            else if ( key1.Equals( key ) )
            {
                return index;
            }
        }

        return -1;
    }

    public void Insert( int index, TKey key, TValue value )
    {
        if ( index > Count || index < 0 )
            throw new ArgumentOutOfRangeException( nameof( index ) );

        ItemDictionary.Add( key, value );
        ItemList.Insert( index, new KeyValuePair<TKey, TValue>( key, value ) );
    }

    public void RemoveAt( int index )
    {
        if ( index >= Count || index < 0 )
            throw new ArgumentOutOfRangeException( nameof( index ) );

        var key = ItemList[index].Key;

        ItemList.RemoveAt( index );
        ItemDictionary.Remove( key );
    }

    public bool Remove( TKey key )
    {
        if ( key == null )
            throw new ArgumentNullException( nameof( key ) );

        var index = IndexOfKey( key );

        if ( index < 0 || !ItemDictionary.Remove( key ) )
            return false;

        ItemList.RemoveAt( index );
        return true;
    }

    public bool TryGetValue( TKey key, out TValue value ) => ItemDictionary.TryGetValue( key, out value );

    public ICollection<TKey> Keys => ItemDictionary.Keys;
    public ICollection<TValue> Values => ItemDictionary.Values;

    void ICollection<KeyValuePair<TKey, TValue>>.Add( KeyValuePair<TKey, TValue> item ) => Add( item.Key, item.Value );

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains( KeyValuePair<TKey, TValue> item ) => ItemDictionary.Contains( item );

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex ) => ItemDictionary.CopyTo( array, arrayIndex );

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item ) => Remove( item.Key );

    IEnumerator IEnumerable.GetEnumerator() => ItemList.GetEnumerator();
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ItemList.GetEnumerator();
}
