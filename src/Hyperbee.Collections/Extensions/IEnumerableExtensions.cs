using System.Collections.Concurrent;

namespace Hyperbee.Collections.Extensions;

public static class EnumerableExtensions
{
    public static Task ForEachAsync<T>( this IEnumerable<T> source, Func<T, Task> function, ParallelOptions options = default )
    {
        options ??= new ParallelOptions();

        if ( options.TaskScheduler != null && options.TaskScheduler != TaskScheduler.Default )
            return source.ForEachAsync( function, options.TaskScheduler, options.MaxDegreeOfParallelism, options.CancellationToken );

        return source.ForEachAsync( function, options.MaxDegreeOfParallelism, options.CancellationToken );
    }

    public static Task ForEachAsync<T>( this IEnumerable<T> source, Func<T, Task> function, int maxDegreeOfParallelism, CancellationToken cancellationToken = default )
    {
        if ( maxDegreeOfParallelism <= 0 )
            maxDegreeOfParallelism = Environment.ProcessorCount;

        return Task.WhenAll( Partitioner
            .Create( source )
            .GetPartitions( maxDegreeOfParallelism )
            .Select( partition => Task.Run( async () =>
            {
                using var enumerator = partition;
                while ( partition.MoveNext() )
                {
                    await function( partition.Current ).ConfigureAwait( false );
                }
            }, cancellationToken ) ) );
    }

    public static Task ForEachAsync<T>( this IEnumerable<T> source, Func<T, Task> function, TaskScheduler scheduler, int maxDegreeOfParallelism, CancellationToken cancellationToken = default )
    {
        if ( maxDegreeOfParallelism <= 0 )
            maxDegreeOfParallelism = Environment.ProcessorCount;

        scheduler ??= TaskScheduler.Current;

        return Task.WhenAll( Partitioner
            .Create( source )
            .GetPartitions( maxDegreeOfParallelism )
            .Select( partition => TaskExtensions.Run( async () =>
            {
                using var enumerator = partition;
                while ( partition.MoveNext() )
                {
                    await function( partition.Current ).ConfigureAwait( false );
                }
            }, scheduler, cancellationToken ) ) );
    }
}

// FIX: Pulled from Hyperbee.Tasks which is not OpenSource yet.
public static class TaskExtensions
{
    public static Task Run( Func<Task> function, TaskScheduler scheduler, CancellationToken cancellationToken )
    {
        ArgumentNullException.ThrowIfNull( function );

        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled( cancellationToken )
            : Task.Factory.StartNew( function, cancellationToken, TaskCreationOptions.DenyChildAttach, scheduler ?? TaskScheduler.Default ).Unwrap();
    }
}
