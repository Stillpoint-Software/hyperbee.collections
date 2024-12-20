namespace Hyperbee.Collections.Extensions;

public static class SetExtensions
{
    public static bool AddOrUpdate<T>( this ISet<T> set, T item )
    {
        ArgumentNullException.ThrowIfNull( set, nameof( set ) );

        if ( set.Contains( item ) )
            set.Remove( item );

        return set.Add( item );
    }
}
