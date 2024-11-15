using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hyperbee.Collections.ArrayPool;

[DebuggerDisplay( "Count = {Count}" )]
[DebuggerTypeProxy( typeof(PooledArray<>.DebuggerView ) )]
public class PooledArray<T> : IDisposable, IReadOnlyList<T>
{
    private T[] _array;
    private int _count;
    private bool _disposed;

    public PooledArray( int initialCapacity = 16 )
    {
        _array = ArrayPool<T>.Shared.Rent( initialCapacity );
        _count = 0;
        _disposed = false;
    }

    public ReadOnlySpan<T> AsReadOnlySpan()
    {
        ThrowIfDisposed();
        return new ReadOnlySpan<T>(_array, 0, _count);
    }

    public ReadOnlySpan<T> AsReadOnlySpan( int start, int count )
    {
        ThrowIfDisposed();
        return new ReadOnlySpan<T>(_array, start, Math.Min( count, _count ));
    }

    public Span<T> AsSpan()
    {
        ThrowIfDisposed();
        return new Span<T>(_array, 0, _count);
    }

    public Span<T> AsSpan( int start, int count )
    {
        ThrowIfDisposed();
        return new Span<T>(_array, start, Math.Min( count, _count ));
    }

    public int Count
    {
        get
        {
            ThrowIfDisposed();
            return _count;
        }
    }

    public T this[int index]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            ThrowIfDisposed();

            if ( index < 0 || index >= _count )
                throw new ArgumentOutOfRangeException( nameof( index ) );

            return _array[index];
        }
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        set
        {
            ThrowIfDisposed();

            EnsureCapacity( index + 1 );
            _array[index] = value;

            if ( index >= _count )
                _count = index + 1;
        }
    }

    private void EnsureCapacity( int requiredCapacity )
    {
        if ( requiredCapacity <= _array.Length )
            return;

        // ArrayPool already has smarts re-allocation size
        // so we can be simpler.
        var newArray = ArrayPool<T>.Shared.Rent( requiredCapacity );

        Array.Copy( _array, newArray, _count );
        ArrayPool<T>.Shared.Return( _array, clearArray: true );

        _array = newArray;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Add( T item )
    {
        ThrowIfDisposed();

        EnsureCapacity( _count + 1 );
        _array[_count++] = item;
    }

    public PooledArray<T> CopyTo( Func<T, bool> predicate )
    {
        ThrowIfDisposed();

        var result = new PooledArray<T>( _count );

        for ( var i = 0; i < _count; i++ )
        {
            if ( predicate( _array[i] ) )
            {
                result.Add( _array[i] );
            }
        }

        return result;
    }

    public void CopyTo( PooledArray<T> destination, int sourceIndex, int destinationIndex, int count )
    {
        ThrowIfDisposed();

        if ( destination == null )
            throw new ArgumentNullException( nameof( destination ) );

        if ( count == 0 )
            return;

        if ( sourceIndex < 0 || sourceIndex >= _count )
            throw new ArgumentOutOfRangeException( nameof( sourceIndex ), "Source index is out of range." );

        if ( count < 0 )
            throw new ArgumentOutOfRangeException( nameof( count ), "Count cannot be negative." );

        if ( sourceIndex + count > _count )
            throw new ArgumentException( "The source array does not have enough elements." );

        if ( destinationIndex + count > destination.Count )
            throw new ArgumentException( "Destination array is too small to hold the copied elements." );

        var sourceSpan = AsSpan( sourceIndex, count );
        var destinationSpan = destination.AsSpan( destinationIndex, count );

        sourceSpan.CopyTo( destinationSpan );
    }

    public void CopyTo( T[] destination, int sourceIndex, int destinationIndex, int count )
    {
        ThrowIfDisposed();

        if ( destination == null )
            throw new ArgumentNullException( nameof( destination ) );

        if ( count == 0 )
            return;

        if ( sourceIndex < 0 || sourceIndex >= _count )
            throw new ArgumentOutOfRangeException( nameof( sourceIndex ), "Start index is out of range." );

        if ( count < 0 )
            count = _count - sourceIndex; // Default to the remaining elements

        if ( sourceIndex + count > _count )
            throw new ArgumentOutOfRangeException( nameof( count ), "Count exceeds available elements from the start index." );

        if ( destinationIndex + count > destination.Length )
            throw new ArgumentException( "Destination array is too small to hold the copied elements." );

        Array.Copy( _array, sourceIndex, destination, 0, count );
    }

    public void Insert( int index, T item )
    {
        ThrowIfDisposed();

        if ( index < 0 || index > _count )
            throw new ArgumentOutOfRangeException( nameof( index ) );

        EnsureCapacity( _count + 1 );

        for ( var i = _count; i > index; i-- )
        {
            _array[i] = _array[i - 1];
        }

        _array[index] = item;
        _count++;
    }

    public void Remove( int index )
    {
        ThrowIfDisposed();

        if ( index < 0 || index >= _count )
            throw new ArgumentOutOfRangeException( nameof( index ) );

        for ( var i = index; i < _count - 1; i++ )
        {
            _array[i] = _array[i + 1];
        }

        _array[--_count] = default;
    }

    public void Remove( Func<T, bool> predicate )
    {
        ThrowIfDisposed();

        var shiftIndex = 0;

        for ( var i = 0; i < _count; i++ )
        {
            if ( predicate( _array[i] ) )
            {
                continue;
            }

            if ( shiftIndex != i )
            {
                _array[shiftIndex] = _array[i];
            }

            shiftIndex++;
        }

        Array.Clear( _array, shiftIndex, _count - shiftIndex );
        _count = shiftIndex;
    }

    public void Remove( Func<T, int, bool> predicate )
    {
        ThrowIfDisposed();

        if ( predicate == null )
            throw new ArgumentNullException( nameof( predicate ) );

        var shiftIndex = 0;

        for ( var i = 0; i < _count; i++ )
        {
            if ( predicate( _array[i], shiftIndex ) ) // Pass the current item and the projected index
            {
                continue;
            }

            if ( shiftIndex != i )
            {
                _array[shiftIndex] = _array[i];
            }

            shiftIndex++;
        }

        Array.Clear( _array, shiftIndex, _count - shiftIndex );
        _count = shiftIndex;
    }

    public void Resize( int newSize )
    {
        ThrowIfDisposed();

        if ( newSize < 0 )
            throw new ArgumentOutOfRangeException( nameof( newSize ), "Size cannot be negative." );

        if ( newSize < _count )
        {
            // Shrink the array
            Array.Clear( _array, newSize, _count - newSize );
        }
        else if ( newSize > _array.Length )
        {
            EnsureCapacity( newSize );
        }

        _count = newSize;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Sort( Comparison<T> comparison )
    {
        ThrowIfDisposed();

        Array.Sort( _array, 0, _count, Comparer<T>.Create( comparison ) );
    }

    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        for ( var i = 0; i < _count; i++ )
        {
            yield return _array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        if ( _disposed )
        {
            return;
        }

        ArrayPool<T>.Shared.Return( _array, clearArray: true );
        _array = null;
        _disposed = true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfDisposed()
    {
        if ( _disposed )
            throw new ObjectDisposedException( nameof(PooledArray<T>), "Cannot access a disposed object." );
    }

    private class DebuggerView
    {
        private readonly PooledArray<T> _pooledArray;

        public DebuggerView( PooledArray<T> pooledArray )
        {
            _pooledArray = pooledArray ?? throw new ArgumentNullException( nameof(pooledArray) );
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public T[] Items
        {
            get
            {
                var array = new T[_pooledArray.Count];
                for ( var i = 0; i < _pooledArray.Count; i++ )
                {
                    array[i] = _pooledArray[i];
                }

                return array;
            }
        }
    }
}
