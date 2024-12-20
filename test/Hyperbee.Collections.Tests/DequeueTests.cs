using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests;

[TestClass]
public class DequeueTests
{
    private const char Separator = ',';

    [TestMethod]
    public void Constructor_should_create_empty_queue()
    {
        var deque = new Deque<int>();
        Assert.IsTrue( deque.IsEmpty );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,bb,cc,dd" )]
    public void Constructor_should_initialize_with_collection( string input, string output )
    {
        var expected = output.Split( Separator );
        var collection = input.Split( Separator );

        var deque = new Deque<string>( collection );
        var result = deque.ToArray();

        CollectionAssert.AreEqual( expected, result );
    }

    [TestMethod]
    public void Constructor_should_set_capacity()
    {
        var deque = new Deque<int>();
        Assert.AreEqual( 16, deque.Capacity );

        deque = new Deque<int>( 8 );
        Assert.AreEqual( 8, deque.Capacity );

        var collection = new[] { 0, 1, 2, 3, 4, 5 };
        deque = new Deque<int>( collection );
        Assert.AreEqual( (int) BitOperations.RoundUpToPowerOf2( (uint) collection.Length ), deque.Capacity );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "xx", 1, "aa,xx,bb,cc,dd" )] // test InsertRange first half insert
    [DataRow( "aa,bb,cc,dd", "xx,yy", 1, "aa,xx,yy,bb,cc,dd" )]

    [DataRow( "aa,bb,cc,dd", "xx", 2, "aa,bb,xx,cc,dd" )] // test InsertRange second half insert
    [DataRow( "aa,bb,cc,dd", "xx,yy", 2, "aa,bb,xx,yy,cc,dd" )]
    public void Should_insert_range( string input, string insert, int index, string output )
    {
        var expected = output.Split( Separator );
        var values = insert.Split( Separator );
        var collection = input.Split( Separator );

        var deque = new Deque<string>( collection );
        deque.InsertRange( index, values );
        var result = deque.ToArray();

        CollectionAssert.AreEqual( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "xx,yy,zz", 1, 0, -1, "aa,xx,yy,zz,bb,cc,dd" )]
    [DataRow( "aa,bb,cc,dd", "xx,yy,zz", 1, 0, 0, "aa,bb,cc,dd" )]
    [DataRow( "aa,bb,cc,dd", "xx,yy,zz", 1, 1, -1, "aa,yy,zz,bb,cc,dd" )]
    [DataRow( "aa,bb,cc,dd", "xx,yy,zz", 1, 1, 1, "aa,yy,bb,cc,dd" )]
    [DataRow( "aa,bb,cc,dd", "xx,yy,zz", 1, 1, 99, "aa,yy,zz,bb,cc,dd" )]
    public void Should_insert_range_scoped( string input, string insert, int index, int fromIndex, int count, string output )
    {
        var expected = output.Split( Separator );
        var values = insert.Split( Separator );
        var collection = input.Split( Separator );

        if ( count < 0 )
            count = values.Length;

        var deque = new Deque<string>( collection );
        deque.InsertRange( index, values, fromIndex, count );
        var result = deque.ToArray();

        CollectionAssert.AreEqual( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd,ee", "xx", false )]
    [DataRow( "aa,bb,cc,dd,ee", "ee", true )]
    public void Contains_should_return_correct_result( string input, string value, bool expected )
    {
        var collection = input.Split( Separator );
        var deque = new Deque<string>( collection );
        var result = deque.Contains( value );

        Assert.IsTrue( result == expected );
    }

    [TestMethod]
    public void Should_add_single_value()
    {
        const int expected = 1;

        // ReSharper disable once UseObjectOrCollectionInitializer
        var deque = new Deque<int>
        {
            expected
        };

        var result = deque[0];

        Assert.IsTrue( deque.Contains( expected ) );
        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    public void Should_add_last()
    {
        const int first = 1;
        const int expected = 2;

        var deque = new Deque<int>
        {
            first
        };

        deque.AddLast( expected );

        Assert.IsTrue( deque.Count == 2 );
        Assert.IsTrue( deque.Contains( expected ) );

        var result = deque[1];
        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    public void Should_add_front()
    {
        const int first = 1;
        const int expected = -1;

        var deque = new Deque<int>
        {
            first
        };

        deque.AddFirst( expected );

        Assert.IsTrue( deque.Count == 2 );
        Assert.IsTrue( deque.Contains( expected ) );

        var result = deque[0];
        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    public void Should_add_range_last()
    {
        // arrange
        var deque = new Deque<int>();

        var initRange = new[] { 3, 4, 5, 6 };
        deque.AddRange( initRange );

        CollectionAssert.AreEqual( initRange, deque.ToArray() );

        // act
        var offset = deque.Count;
        var range = new[] { 7, 8, 9, 10 };
        deque.AddRangeLast( range );

        foreach ( var item in range )
        {
            Assert.AreEqual( item, deque[offset] );
            offset++;
        }
    }

    [TestMethod]
    public void Should_add_range_first()
    {
        var deque = new Deque<int>();
        var range = new[] { 3, 4, 5, 6 };
        deque.AddRangeFirst( range );

        CollectionAssert.AreEqual( range, deque.ToArray() );
    }

    [TestMethod]
    public void Should_remove_items()
    {
        var collection = new[] { 0, 1, 2, 3 };
        var deque = new Deque<int>( collection );

        Assert.IsTrue( deque.Contains( 0 ) );
        Assert.IsTrue( deque.Remove( 0 ) );
        Assert.IsFalse( deque.Contains( 0 ) );
        Assert.AreEqual( 1, deque.RemoveFirst() );
        Assert.AreEqual( 3, deque.RemoveLast() );

        deque.Clear();
        deque.AddRange( collection );
        Assert.IsTrue( deque.Contains( 2 ) );

        deque.RemoveAt( 2 );
        Assert.IsFalse( deque.Contains( 2 ) );
    }

    [TestMethod]
    public void Should_remove_range()
    {
        var collection = new[] { 0, 1, 2, 3 };
        var deque = new Deque<int>( collection );

        foreach ( var item in collection )
        {
            Assert.IsTrue( deque.Contains( item ) );
        }

        deque.RemoveRange( 1, 2 );

        Assert.IsTrue( deque.Contains( 0 ) );
        Assert.IsFalse( deque.Contains( 1 ) );
        Assert.IsFalse( deque.Contains( 2 ) );
        Assert.IsTrue( deque.Contains( 3 ) );
    }

    [TestMethod]
    public void Should_get_and_set()
    {
        var items = new[] { 0, 1, 2, 3 };
        var deque = new Deque<int>( items );

        for ( var i = 0; i < deque.Count - 1; i++ )
        {
            var item = deque[i];
            item++;
            Assert.AreEqual( item, deque[i + 1] );

            deque[i] = item;
            Assert.AreEqual( item, deque[i] );
        }
    }

    [TestMethod]
    public void Should_clear()
    {
        var bulkAddCount = 100;
        var deque = new Deque<int>();
        for ( var i = bulkAddCount - 1; i >= 0; i-- )
        {
            deque.AddFirst( i );
        }

        deque.Clear();
        Assert.AreEqual( 0, deque.Count );
    }
}
