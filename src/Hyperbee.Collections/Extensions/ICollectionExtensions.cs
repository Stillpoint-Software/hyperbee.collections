using System.Collections;

namespace Hyperbee.Collections.Extensions;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty( this ICollection collection ) => collection == null || collection.Count == 0;
}
