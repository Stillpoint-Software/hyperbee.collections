using Hyperbee.Collections.ArrayPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests.ArrayPool;

[TestClass]
public class PooledStackTests
{
    [TestMethod]
    public void Push_ShouldIncreaseCount()
    {
        // Arrange
        using var stack = new PooledStack<int>();

        // Act
        stack.Push( 10 );

        // Assert
        Assert.AreEqual( 1, stack.Count );
    }

    [TestMethod]
    public void Pop_ShouldReturnLastItem()
    {
        // Arrange
        using var stack = new PooledStack<int>();
        stack.Push( 10 );
        stack.Push( 20 );

        // Act
        var result = stack.Pop();

        // Assert
        Assert.AreEqual( 20, result );
        Assert.AreEqual( 1, stack.Count );
    }

    [TestMethod]
    public void Pop_ShouldThrow_WhenStackIsEmpty()
    {
        // Arrange
        using var stack = new PooledStack<int>();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>( () => stack.Pop() );
    }

    [TestMethod]
    public void Peek_ShouldReturnLastItemWithoutRemoval()
    {
        // Arrange
        using var stack = new PooledStack<int>();
        stack.Push( 10 );
        stack.Push( 20 );

        // Act
        var result = stack.Peek();

        // Assert
        Assert.AreEqual( 20, result );
        Assert.AreEqual( 2, stack.Count );
    }

    [TestMethod]
    public void Clear_ShouldEmptyTheStack()
    {
        // Arrange
        using var stack = new PooledStack<int>();
        stack.Push( 10 );
        stack.Push( 20 );

        // Act
        stack.Clear();

        // Assert
        Assert.AreEqual( 0, stack.Count );
    }

    [TestMethod]
    public void Dispose_ShouldReleaseMemory()
    {
        // Arrange
        var stack = new PooledStack<int>();

        // Act
        stack.Dispose();

        // Assert
        Assert.ThrowsException<ObjectDisposedException>( () => stack.Push( 1 ) );
    }
}
