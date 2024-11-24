using System.Collections.Concurrent;
using Hyperbee.Collections.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests.Extensions;

[TestClass]
public class IEnumerableExtensionsTests
{
    private List<int> _testData;

    [TestInitialize]
    public void TestInitialize()
    {
        _testData = Enumerable.Range(1, 100).ToList();
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldExecuteDelegateForAllItems()
    {
        // Arrange
        var processedItems = new ConcurrentBag<int>();

        // Act
        await _testData.ForEachAsync(async item =>
        {
            processedItems.Add(item); 
            await Task.Yield(); 
        });

        // Assert
        CollectionAssert.AreEquivalent(_testData, processedItems.ToList());
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldHandleEmptyCollection()
    {
        // Arrange
        var emptyData = new List<int>();
        var processedItems = new ConcurrentBag<int>();

        // Act
        await emptyData.ForEachAsync(async item =>
        {
            processedItems.Add(item); // Should never be called
            await Task.Yield();
        });

        // Assert
        Assert.AreEqual(0, processedItems.Count, "No items should be processed for an empty collection.");
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldThrowArgumentNullException_ForNullSource()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            ((IEnumerable<int>)null).ForEachAsync(async item => await Task.Yield()));
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldThrowArgumentNullException_ForNullDelegate()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            _testData.ForEachAsync(null));
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldRespectCancellation()
    {
        // Arrange
        var processedItems = new ConcurrentBag<int>();
        var cts = new CancellationTokenSource();
        cts.CancelAfter(50); // Cancel after 50ms

        // Act
        try
        {
            await _testData.ForEachAsync(async item =>
            {
                await Task.Delay(10, cts.Token ); 
                processedItems.Add(item);
            }, cancellationToken: cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        Assert.IsTrue(processedItems.Count < _testData.Count, "Not all items should be processed after cancellation.");
    }

    [TestMethod]
    public async Task ForEachAsync_ShouldPropagateExceptions()
    {
        // Arrange
        var exceptionThrown = false;

        // Act
        try
        {
            await _testData.ForEachAsync(async item =>
            {
                if (item == 50)
                {
                    throw new InvalidOperationException("Test exception");
                }

                await Task.Yield(); 
            });
        }
        catch (InvalidOperationException ex)
        {
            exceptionThrown = true;
            Assert.AreEqual("Test exception", ex.Message);
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "The exception should be propagated.");
    }
}
