using System.Collections.Concurrent;
using Hyperbee.Collections.Extensions;

namespace Hyperbee.Collections.Tests.Extensions;

[TestClass]
public class IEnumerableExtensionsTests
{
    private List<int> _testData;

    [TestInitialize]
    public void TestInitialize()
    {
        _testData = Enumerable.Range( 1, 100 ).ToList();
    }

    [TestMethod]
    public async Task ParallelEachAsync_ShouldExecuteDelegateForAllItems()
    {
        // Arrange
        var processedItems = new ConcurrentBag<int>();

        // Act
        await _testData.ParallelEachAsync( async item =>
        {
            processedItems.Add( item );
            await Task.Yield();
        } );

        // Assert
        CollectionAssert.AreEquivalent( _testData, processedItems.ToList() );
    }

    [TestMethod]
    public async Task ParallelEachAsync_ShouldHandleEmptyCollection()
    {
        // Arrange
        var emptyData = new List<int>();
        var processedItems = new ConcurrentBag<int>();

        // Act
        await emptyData.ParallelEachAsync( async item =>
        {
            processedItems.Add( item ); // Should never be called
            await Task.Yield();
        } );

        // Assert
        Assert.IsEmpty( processedItems, "No items should be processed for an empty collection." );
    }

    [TestMethod]
    public async Task ParallelEachAsync_ShouldThrowArgumentNullException_ForNullSource()
    {
        // Act & Assert
        var exception = false;
        try
        {
            await ((IEnumerable<int>) null).ParallelEachAsync( async item => await Task.Yield() );
        }
        catch ( ArgumentNullException )
        {
            exception = true;
        }

        Assert.IsTrue( exception, "Expected ArgumentNullException was not thrown" );
    }

    [TestMethod]
    public async Task ParallelEachAsyncc_ShouldThrowArgumentNullException_ForNullDelegate()
    {
        // Act & Assert
        var exception = false;
        try
        {
            await _testData.ParallelEachAsync( null );
        }
        catch ( ArgumentNullException )
        {
            exception = true;
        }

        Assert.IsTrue( exception, "Expected ArgumentNullException was not thrown" );
    }

    [TestMethod]
    public async Task ParallelEachAsync_ShouldRespectCancellation()
    {
        // Arrange
        var processedItems = new ConcurrentBag<int>();
        var cts = new CancellationTokenSource();
        cts.CancelAfter( 50 ); // Cancel after 50ms

        // Act
        try
        {
            await _testData.ParallelEachAsync( async item =>
            {
                await Task.Delay( 10, cts.Token );
                processedItems.Add( item );
            }, cancellationToken: cts.Token );
        }
        catch ( OperationCanceledException )
        {
            // Expected
        }

        // Assert
        Assert.IsLessThan( _testData.Count, processedItems.Count, "Not all items should be processed after cancellation." );
    }

    [TestMethod]
    public async Task ParallelEachAsync_ShouldPropagateExceptions()
    {
        // Arrange
        var exceptionThrown = false;

        // Act
        try
        {
            await _testData.ParallelEachAsync( async item =>
            {
                if ( item == 50 )
                {
                    throw new InvalidOperationException( "Test exception" );
                }

                await Task.Yield();
            } );
        }
        catch ( InvalidOperationException ex )
        {
            exceptionThrown = true;
            Assert.AreEqual( "Test exception", ex.Message );
        }

        // Assert
        Assert.IsTrue( exceptionThrown, "The exception should be propagated." );
    }
}
