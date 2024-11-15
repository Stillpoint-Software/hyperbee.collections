using Hyperbee.Collections.ArrayPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests.ArrayPool;

[TestClass]
public class PooledArrayTests
{
    [TestMethod]
    public void Add_ShouldIncreaseCount()
    {
        // Arrange
        using var array = new PooledArray<int>();

        // Act
        array.Add( 10 );

        // Assert
        Assert.AreEqual( 1, array.Count );
        Assert.AreEqual( 10, array[0] );
    }

    [TestMethod]
    public void Indexer_ShouldThrow_WhenOutOfRange()
    {
        // Arrange
        using var array = new PooledArray<int>();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>( () => _ = array[0] );
    }

    [TestMethod]
    public void CopyTo_ShouldCopyFilteredItems()
    {
        // Arrange
        using var source = new PooledArray<int>();
        source.Add( 1 );
        source.Add( 2 );
        source.Add( 3 );

        // Act
        using var result = source.CopyTo( x => x > 1 );

        // Assert
        Assert.AreEqual( 2, result.Count );
        Assert.AreEqual( 2, result[0] );
        Assert.AreEqual( 3, result[1] );
    }

    [TestMethod]
    public void Dispose_ShouldReleaseMemory()
    {
        // Arrange
        var array = new PooledArray<int>();

        // Act
        array.Dispose();

        // Assert
        Assert.ThrowsException<ObjectDisposedException>( () => array.Add( 1 ) );
    }

    [TestMethod]
    public void Insert_ShouldShiftElements()
    {
        // Arrange
        using var array = new PooledArray<int>();
        array.Add( 1 );
        array.Add( 2 );

        // Act
        array.Insert( 1, 99 );

        // Assert
        Assert.AreEqual( 3, array.Count );
        Assert.AreEqual( 1, array[0] );
        Assert.AreEqual( 99, array[1] );
        Assert.AreEqual( 2, array[2] );
    }

    [TestMethod]
    public void Remove_ShouldRemoveElement()
    {
        // Arrange
        using var array = new PooledArray<int>();
        array.Add( 1 );
        array.Add( 2 );
        array.Add( 3 );

        // Act
        array.Remove( 1 );

        // Assert
        Assert.AreEqual( 2, array.Count );
        Assert.AreEqual( 1, array[0] );
        Assert.AreEqual( 3, array[1] );
    }
}
