using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests;

[TestClass]
public class DisjointSetTests
{
    [TestMethod]
    public void TryAdd_SingleItem_ShouldAddSuccessfully()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();

        // Act
        var result = disjointSet.TryAdd( 1 );

        // Assert
        Assert.IsTrue( result );
        Assert.AreEqual( 1, disjointSet.Count );
        Assert.AreEqual( 1, disjointSet.Find( 1 ) );
    }

    [TestMethod]
    public void TryAdd_DuplicateItem_ShouldNotAdd()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();
        disjointSet.TryAdd( 1 );

        // Act
        var result = disjointSet.TryAdd( 1 );

        // Assert
        Assert.IsFalse( result );
        Assert.AreEqual( 1, disjointSet.Count );
    }

    [TestMethod]
    public void TryAdd_MultipleItems_ShouldAddAndGroupCorrectly()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();

        // Act
        var result = disjointSet.TryAdd( [1, 2, 3] );

        // Assert
        Assert.IsTrue( result );
        Assert.AreEqual( 3, disjointSet.Count );
        Assert.AreEqual( 1, disjointSet.Find( 1 ) );
        Assert.AreEqual( 1, disjointSet.Find( 2 ) );
        Assert.AreEqual( 1, disjointSet.Find( 3 ) );
    }

    [TestMethod]
    public void Union_TwoItems_ShouldMergeGroups()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();
        disjointSet.TryAdd( 1 );
        disjointSet.TryAdd( 2 );

        // Act
        var result = disjointSet.Union( 1, 2 );

        // Assert
        Assert.IsTrue( result );
        Assert.AreEqual( 1, disjointSet.Find( 1 ) );
        Assert.AreEqual( 1, disjointSet.Find( 2 ) );
    }

    [TestMethod]
    public void Union_AlreadyInSameGroup_ShouldReturnFalse()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();
        disjointSet.TryAdd( [1, 2] );
        disjointSet.Union( 1, 2 );

        // Act
        var result = disjointSet.Union( 1, 2 );

        // Assert
        Assert.IsFalse( result );
    }

    [TestMethod]
    public void FindGroup_ShouldReturnAllItemsInSameGroup()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();
        disjointSet.TryAdd( [1, 3, 5] );
        disjointSet.TryAdd( [2, 4] );
        disjointSet.Union( 1, 2 );

        // Act
        var group = disjointSet.FindGroup( 1 ).ToList();

        // Assert
        CollectionAssert.AreEquivalent( (int[]) [1, 2, 3, 4, 5], group );
    }

    [TestMethod]
    [ExpectedException( typeof( InvalidOperationException ) )]
    public void Find_NonExistentItem_ShouldThrowException()
    {
        // Arrange
        var disjointSet = new DisjointSet<int>();

        // Act
        disjointSet.Find( 1 );
    }

    [TestMethod]
    public void OnDisjointUnion_Callback_ShouldBeInvoked()
    {
        // Arrange
        var callbackInvoked = false;
        var disjointSet = new DisjointSet<int>( ( rep, retired ) =>
        {
            callbackInvoked = true;
            Assert.AreEqual( 1, rep );
            Assert.AreEqual( 2, retired );
        } );

        disjointSet.TryAdd( 1 );
        disjointSet.TryAdd( 2 );

        // Act
        disjointSet.Union( 1, 2 );

        // Assert
        Assert.IsTrue( callbackInvoked );
    }
}

