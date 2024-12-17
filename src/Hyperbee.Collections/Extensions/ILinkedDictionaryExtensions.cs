
namespace Hyperbee.Collections.Extensions;

public static class ILinkedDictionaryExtensions
{
    // create scope on linked dictionary operations

    public static IDisposable Enter<TKey, TValue>( this ILinkedDictionary<TKey, TValue> linked, IEnumerable<KeyValuePair<TKey, TValue>> collection )
    {
        linked.Push( collection );
        return new Disposable( () => linked.Pop() );
    }

    // projection across linked dictionary layers

    public static IEnumerable<TOutput> Select<TKey, TValue, TOutput>( this ILinkedDictionary<TKey, TValue> linked, Func<int, KeyValuePair<TKey, TValue>, TOutput> selector, KeyScope options = KeyScope.All )
    {
        var offset = 0;

        var keys = options == KeyScope.Closest ? new HashSet<TKey>( linked.Comparer ) : null;

        foreach ( var scope in linked.Nodes() )
        {
            foreach ( var pair in scope.Dictionary )
            {
                if ( options == KeyScope.Closest )
                {
                    if ( keys!.Contains( pair.Key ) )
                        continue;

                    keys.Add( pair.Key );
                }

                yield return selector( offset, pair );
            }

            if ( options == KeyScope.Current )
                break;

            offset++;
        }
    }
}

// FIX: Pulled from Hyperbee.Core which is not OpenSource yet.
public sealed class Disposable( Action dispose ) : IDisposable
{
    private int _disposed;
    private Action Disposer { get; } = dispose;

    public void Dispose()
    {
        if ( Interlocked.CompareExchange( ref _disposed, 1, 0 ) == 0 )
            Disposer.Invoke();
    }
}
