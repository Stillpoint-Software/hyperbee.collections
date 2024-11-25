#region License
//
// A Generic Double-Ended Queue (Deque) class. 
// 
// [1] https://github.com/tejacques/Deque
//
// The MIT License
//
// Copyright (c) 2019 Brent Farmer
// Copyright (c) 2013 Tom Jacques
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using System.Collections;
using System.Numerics;

// ReSharper disable PossibleMultipleEnumeration

// A generic double-ended queue class.
// https://en.wikipedia.org/wiki/Double-ended_queue

namespace Hyperbee.Collections;

public class Deque<T> : IList<T>
{
    private const int DefaultCapacity = 16;
    private int _capacityClosestPowerOfTwoMinusOne;
    private int _startOffset;
    private T[] _buffer;

    public Deque()
        : this( DefaultCapacity )
    {
    }

    public Deque( int capacity )
    {
        if ( capacity < 0 )
            throw new ArgumentOutOfRangeException( nameof( capacity ), "Capacity cannot be less than 0." );

        Capacity = capacity;
    }

    public Deque( IEnumerable<T> collection )
        : this( collection.Count() )
    {
        InsertRange( 0, collection );
    }

    public int Capacity
    {
        get => _buffer.Length;

        set
        {
            if ( value < 0 )
                throw new ArgumentOutOfRangeException( nameof( value ), "Capacity cannot be less than 0." );

            if ( value < Count )
                throw new InvalidOperationException( "Capacity cannot be set to a value less than Count." );

            if ( _buffer != null && value == _buffer.Length )
                return;

            // create a new array and copy the old values.
            value = (int) BitOperations.RoundUpToPowerOf2( (uint) value );

            var newBuffer = new T[value];
            CopyTo( newBuffer, 0 );

            // set up to use the new buffer.
            _buffer = newBuffer;
            _startOffset = 0;
            _capacityClosestPowerOfTwoMinusOne = value - 1;
        }
    }

    public bool IsEmpty => 0 == Count;

    private void EnsureCapacity( int numElements )
    {
        if ( Count + numElements > Capacity )
            Capacity = Count + numElements;
    }

    private int ToBufferIndex( int index )
    {
        // convert index to a buffer index.
        return (index + _startOffset) & _capacityClosestPowerOfTwoMinusOne;
    }

    private void ThrowIfIndexOutOfRange( int index )
    {
        if ( index >= Count )
            throw new IndexOutOfRangeException( $"The supplied index [{index}] is invalid." );
    }

    private static void ThrowIfArgumentsOutOfRange( int length, int offset, int count )
    {
        if ( offset < 0 )
            throw new ArgumentOutOfRangeException( nameof( offset ), $"Invalid offset {offset}." );

        if ( count < 0 )
            throw new ArgumentOutOfRangeException( nameof( count ), $"Invalid count {count}." );

        if ( length - offset < count )
            throw new ArgumentException( $"Invalid offset ({offset}) or count + ({count}) for source length {length}." );
    }

    private int ShiftStartOffset( int value )
    {
        _startOffset = ToBufferIndex( value );
        return _startOffset;
    }

    private int PreShiftStartOffset( int value )
    {
        var offset = _startOffset;
        ShiftStartOffset( value );

        return offset;
    }

    private int PostShiftStartOffset( int value )
    {
        return ShiftStartOffset( value );
    }

    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        // The below is done for performance reasons.
        // Rather than doing bounds checking and modulo arithmetic
        // that would go along with calls to Get(index), we can skip
        // all of that by referencing the underlying array.

        if ( _startOffset + Count > Capacity )
        {
            for ( var i = _startOffset; i < Capacity; i++ )
                yield return _buffer[i];

            var endIndex = ToBufferIndex( Count );
            for ( var i = 0; i < endIndex; i++ )
                yield return _buffer[i];
        }
        else
        {
            var endIndex = _startOffset + Count;
            for ( var i = _startOffset; i < endIndex; i++ )
                yield return _buffer[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region ICollection

    public bool IsReadOnly => false;

    public int Count { get; private set; }

    private void IncrementCount( int value ) => Count += value;

    private void DecrementCount( int value ) => Count = Math.Max( Count - value, 0 );

    public void Add( T item ) => AddLast( item );

    private void ClearBuffer( int logicalIndex, int length )
    {
        var offset = ToBufferIndex( logicalIndex );

        if ( offset + length > Capacity )
        {
            var len = Capacity - offset;
            Array.Clear( _buffer, offset, len );

            len = ToBufferIndex( logicalIndex + length );
            Array.Clear( _buffer, 0, len );
        }
        else
        {
            Array.Clear( _buffer, offset, length );
        }
    }

    public void Clear()
    {
        if ( Count > 0 )
            ClearBuffer( 0, Count );

        Count = 0;
        _startOffset = 0;
    }

    public bool Contains( T item ) => IndexOf( item ) != -1;

    public void CopyTo( T[] array, int arrayIndex )
    {
        ArgumentNullException.ThrowIfNull( array, nameof( array ) );

        if ( _buffer == null )
            return;

        ThrowIfArgumentsOutOfRange( array.Length, arrayIndex, Count );

        if ( _startOffset != 0 && _startOffset + Count >= Capacity )
        {
            var lengthFromStart = Capacity - _startOffset;
            var lengthFromEnd = Count - lengthFromStart;

            Array.Copy( _buffer, _startOffset, array, 0, lengthFromStart );
            Array.Copy( _buffer, 0, array, lengthFromStart, lengthFromEnd );
        }
        else
        {
            Array.Copy( _buffer, _startOffset, array, 0, Count );
        }
    }

    public bool Remove( T item )
    {
        var result = true;
        var index = IndexOf( item );

        if ( index == -1 )
            result = false;
        else
            RemoveAt( index );

        return result;
    }

    #endregion

    #region List<T>

    public T this[int index]
    {
        get => GetAt( index );
        set => SetAt( index, value );
    }

    public void Insert( int index, T item )
    {
        EnsureCapacity( 1 );

        if ( index == 0 )
        {
            AddFirst( item );
            return;
        }

        if ( index == Count )
        {
            AddLast( item );
            return;
        }

        InsertRange( index, [item] );
    }

    public int IndexOf( T item )
    {
        var index = this.TakeWhile( myItem => !myItem.Equals( item ) ).Count();

        if ( index == Count )
            index = -1;

        return index;
    }

    public void RemoveAt( int index )
    {
        if ( index == 0 )
        {
            RemoveFirst();
            return;
        }

        if ( index == Count - 1 )
        {
            RemoveLast();
            return;
        }

        RemoveRange( index, 1 );
    }

    #endregion

    public void AddFirst( T item )
    {
        EnsureCapacity( 1 );
        _buffer[PostShiftStartOffset( -1 )] = item;
        IncrementCount( 1 );
    }

    public void AddLast( T item )
    {
        EnsureCapacity( 1 );
        _buffer[ToBufferIndex( Count )] = item;
        IncrementCount( 1 );
    }

    public T RemoveFirst()
    {
        if ( IsEmpty )
            throw new InvalidOperationException( "The queue is empty." );

        var result = _buffer[_startOffset];
        _buffer[PreShiftStartOffset( 1 )] = default;
        DecrementCount( 1 );

        return result;
    }

    public T RemoveLast()
    {
        if ( IsEmpty )
            throw new InvalidOperationException( "The queue is empty." );

        DecrementCount( 1 );
        var endIndex = ToBufferIndex( Count );
        var result = _buffer[endIndex];
        _buffer[endIndex] = default;

        return result;
    }

    public void AddRange( IEnumerable<T> collection ) => AddRangeLast( collection );

    public void AddRangeFirst( IEnumerable<T> collection ) => AddRangeFirst( collection, 0, collection.Count() );

    public void AddRangeFirst( IEnumerable<T> collection, int fromIndex, int count ) => InsertRange( 0, collection, fromIndex, count );

    public void AddRangeLast( IEnumerable<T> collection ) => AddRangeLast( collection, 0, collection.Count() );

    public void AddRangeLast( IEnumerable<T> collection, int fromIndex, int count ) => InsertRange( Count, collection, fromIndex, count );

    public void InsertRange( int index, IEnumerable<T> collection ) => InsertRange( index, collection, 0, collection.Count() );

    public void InsertRange( int index, IEnumerable<T> collection, int fromIndex, int count )
    {
        ThrowIfIndexOutOfRange( index - 1 );

        if ( count <= 0 )
            return;

        var size = collection.Count();

        if ( fromIndex < 0 || fromIndex > size - 1 )
            throw new ArgumentOutOfRangeException( nameof( fromIndex ) );

        var insertCount = Math.Min( size - fromIndex, count );

        if ( insertCount == 0 )
            return;

        EnsureCapacity( insertCount );

        // shift items to make room for the insert
        if ( index < Count / 2 )
        {
            // inserting into the first half of the list

            if ( index > 0 )
            {
                var shiftCount = index;
                var shiftIndex = Capacity - insertCount;

                for ( var j = 0; j < shiftCount; j++ )
                    _buffer[ToBufferIndex( shiftIndex + j )] = _buffer[ToBufferIndex( j )];
            }

            // shift the starting offset
            ShiftStartOffset( -insertCount );
        }
        else
        {
            // inserting into the second half of the list

            if ( index < Count )
            {
                var shiftCount = Count - index;
                var shiftIndex = index + insertCount;

                for ( var j = shiftCount - 1; j >= 0; j-- )
                    _buffer[ToBufferIndex( shiftIndex + j )] = _buffer[ToBufferIndex( index + j )];
            }
        }

        // insert new items into place
        var i = index;
        var items = collection.Skip( fromIndex ).Take( insertCount );

        foreach ( var item in items )
        {
            _buffer[ToBufferIndex( i )] = item;
            ++i;
        }

        // adjust item count
        IncrementCount( insertCount );
    }

    public void RemoveRange( int index, int count )
    {
        if ( IsEmpty )
            throw new InvalidOperationException( "The Dequeue is empty." );

        if ( index > Count - count )
            throw new IndexOutOfRangeException( "The supplied index is greater than the Count." );

        // clear out the underlying array
        ClearBuffer( index, count );

        if ( index == 0 )
        {
            // removing from the beginning: shift the start offset
            ShiftStartOffset( count );
            Count -= count;
            return;
        }

        if ( index == Count - count )
        {
            // removing from the ending: trim the existing view
            Count -= count;
            return;
        }

        if ( index + count / 2 < Count / 2 )
        {
            // removing from first half of list
            var shiftCount = index;
            var writeIndex = count;

            for ( var j = 0; j < shiftCount; j++ )
                _buffer[ToBufferIndex( writeIndex + j )] = _buffer[ToBufferIndex( j )];

            // rotate to new view
            ShiftStartOffset( count );
        }
        else
        {
            // removing from second half of list
            var shiftCount = Count - count - index;
            var readIndex = index + count;

            for ( var j = 0; j < shiftCount; ++j )
                _buffer[ToBufferIndex( index + j )] = _buffer[ToBufferIndex( readIndex + j )];
        }

        // adjust item count
        DecrementCount( count );
    }

    public T GetAt( int index )
    {
        ThrowIfIndexOutOfRange( index );
        return _buffer[ToBufferIndex( index )];
    }

    public void SetAt( int index, T item )
    {
        ThrowIfIndexOutOfRange( index );
        _buffer[ToBufferIndex( index )] = item;
    }
}
