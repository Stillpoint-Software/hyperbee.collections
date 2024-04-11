using System.Collections.ObjectModel;

namespace Hyperbee.Collections;

public interface IOrderedSet<T> : IList<T>
{
    bool TryGetValue( T key, out T item );
    IEqualityComparer<T> Comparer { get; }
    T this[T key] { get; }
}

public class OrderedSet<T> : KeyedCollection<T, T>, IOrderedSet<T>
{
    protected override T GetKeyForItem( T item ) => item;
}
