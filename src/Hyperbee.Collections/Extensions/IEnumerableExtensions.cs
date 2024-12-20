using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Hyperbee.Collections.Extensions;

public static class IEnumerableExtensions
{
    // ParallelEachAsync
    //
    // Executes the specified asynchronous action on each element of the
    // enumerable in parallel, with support for limiting concurrency and cancellation.
    //
    // Parameters:
    // - source: The enumerable to process.
    // - asyncAction: The asynchronous action to execute on each element.
    // - maxDegreeOfParallelism: The maximum number of concurrent tasks. 
    // - cancellationToken: The cancellation token to monitor for cancellation requests.
    //
    // Returns:
    // A task that represents the completion of all asynchronous operations.
    //
    // Example:
    // var testData = Enumerable.Range(1, 100);
    // await testData.ParallelEachAsync(async item =>
    // {
    //     await ProcessItemAsync(item);
    // }, maxDegreeOfParallelism: 4);
    //

    public static Task ParallelEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> asyncAction,
        CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( source, nameof( source ) );
        ArgumentNullException.ThrowIfNull( asyncAction, nameof( asyncAction ) );

        return ParallelEachAsync( source, asyncAction, Environment.ProcessorCount, cancellationToken );
    }

    public static Task ParallelEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> asyncAction,
        ParallelOptions options )
    {
        ArgumentNullException.ThrowIfNull( source, nameof( source ) );
        ArgumentNullException.ThrowIfNull( asyncAction, nameof( asyncAction ) );

        options ??= new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        if ( options.TaskScheduler != null && options.TaskScheduler != TaskScheduler.Default )
            return ParallelEachAsync( source, asyncAction, options.TaskScheduler, options.MaxDegreeOfParallelism, options.CancellationToken );

        return ParallelEachAsync( source, asyncAction, options.MaxDegreeOfParallelism, options.CancellationToken );
    }

    public static Task ParallelEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> asyncAction,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( source, nameof( source ) );
        ArgumentNullException.ThrowIfNull( asyncAction, nameof( asyncAction ) );

        if ( maxDegreeOfParallelism <= 0 )
            maxDegreeOfParallelism = Environment.ProcessorCount;

        var cts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

        var tasks = Partitioner
            .Create( source )
            .GetPartitions( maxDegreeOfParallelism )
            .Select( partition => Task.Run( async () =>
            {
                using var enumerator = partition;
                while ( enumerator.MoveNext() )
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await asyncAction( enumerator.Current ).ConfigureAwait( false );
                }
            }, cts.Token ) );

        return Task.WhenAll( tasks ).ContinueWith( task =>
        {
            cts.Dispose();

            if ( task.IsFaulted )
            {
                // Unwrap and rethrow the first exception
                ExceptionDispatchInfo.Capture(
                    task.Exception!.InnerExceptions.First()
                ).Throw();
            }

            if ( task.IsCanceled )
                throw new OperationCanceledException();

        }, TaskScheduler.Default ); // Run continuation on the default scheduler
    }

    public static Task ParallelEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> asyncAction,
        TaskScheduler scheduler,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( source, nameof( source ) );
        ArgumentNullException.ThrowIfNull( asyncAction, nameof( asyncAction ) );
        ArgumentNullException.ThrowIfNull( scheduler, nameof( scheduler ) );

        if ( maxDegreeOfParallelism <= 0 )
            maxDegreeOfParallelism = Environment.ProcessorCount;

        var cts = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );

        var tasks = Partitioner
            .Create( source )
            .GetPartitions( maxDegreeOfParallelism )
            .Select( partition => Task.Factory.StartNew( async () =>
            {
                using var enumerator = partition;
                while ( enumerator.MoveNext() )
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await asyncAction( enumerator.Current ).ConfigureAwait( false );
                }
            }, cts.Token, TaskCreationOptions.DenyChildAttach, scheduler ).Unwrap() );

        return Task.WhenAll( tasks ).ContinueWith( task =>
        {
            cts.Dispose();

            if ( task.IsFaulted )
            {
                // Unwrap and rethrow the first exception
                ExceptionDispatchInfo.Capture(
                    task.Exception!.InnerExceptions.First()
                ).Throw();
            }

            if ( task.IsCanceled )
                throw new OperationCanceledException();

        }, TaskScheduler.Default ); // Run continuation on the default scheduler
    }
}
