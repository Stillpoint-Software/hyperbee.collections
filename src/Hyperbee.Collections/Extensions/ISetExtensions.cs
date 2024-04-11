namespace Hyperbee.Collections.Extensions;

public static class SetExtensions
{
    public static bool AddOrUpdate<T>( this ISet<T> set, T item )
    {
        if ( set.Contains( item ) )
            set.Remove( item );

        return set.Add( item );
    }
}
