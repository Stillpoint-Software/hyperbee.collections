using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hyperbee.Collections.ArrayPool;

[DebuggerDisplay( "Count = {Count}" )]
[DebuggerTypeProxy( typeof( PooledStack<>.DebuggerView ) )]
public class PooledStack<T> : IDisposable
{
    private readonly PooledArray<T> _array;
    private int _top;
    private bool _disposed;

    public PooledStack( int initialCapacity = 16 )
    {
        _array = new PooledArray<T>( initialCapacity );
        _top = 0;
        _disposed = false;
    }

    public int Count
    {
        get
        {
            ThrowIfDisposed();
            return _top;
        }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Push( T item )
    {
        ThrowIfDisposed();

        _array[_top++] = item; // `PooledArray` will automatically resize if needed 
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public T Pop()
    {
        ThrowIfDisposed();

        if ( _top == 0 )
            throw new InvalidOperationException( "Stack is empty." );

        T item = _array[--_top];
        _array[_top] = default; // Clear the reference
        return item;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public T Peek()
    {
        ThrowIfDisposed();

        if ( _top == 0 )
            throw new InvalidOperationException( "Stack is empty." );

        return _array[_top - 1];
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Clear()
    {
        ThrowIfDisposed();

        _array.Resize( 0 );
        _top = 0;
    }

    public void Dispose()
    {
        if ( _disposed )
        {
            return;
        }

        _array.Dispose();
        _disposed = true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfDisposed()
    {
        if ( _disposed )
            throw new ObjectDisposedException( nameof( PooledArray<T> ), "Cannot access a disposed object." );
    }

    private class DebuggerView
    {
        private readonly PooledStack<T> _pooledStack;

        public DebuggerView( PooledStack<T> pooledStack )
        {
            _pooledStack = pooledStack ?? throw new ArgumentNullException( nameof( pooledStack ) );
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public T[] Items
        {
            get
            {
                var array = new T[_pooledStack.Count];
                for ( int i = 0; i < _pooledStack.Count; i++ )
                {
                    // Stack displays elements in reverse (LIFO)
                    array[i] = _pooledStack._array[_pooledStack.Count - i - 1];
                }

                return array;
            }
        }
    }
}
