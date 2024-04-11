namespace Hyperbee.Collections.Extensions;

public static class ArrayExtensions
{
    public static T[] Copy<T>( this T[] array, int startIndex )
    {
        // Copies the elements from an array starting at the specified source index 

        ArgumentNullException.ThrowIfNull( array );

        return array.Copy( startIndex, array.Length - startIndex );
    }

    public static T[] Copy<T>( this T[] array, int startIndex, int items )
    {
        // Copies a range of elements from an array starting at the specified source index 
        // to a new array object.

        ArgumentNullException.ThrowIfNull( array );

        if ( items <= 0 )
            throw new IndexOutOfRangeException();

        var subset = new T[items];
        Array.Copy( array, startIndex, subset, 0, items );

        return subset;
    }

    public static long[] GetDimensions( this Array array )
    {
        return array.GetDimensions( out _ );
    }

    public static long[] GetDimensions( this Array array, out long arrayLength )
    {
        var dimensions = new long[array.Rank];
        arrayLength = 0;

        for ( var dimension = 0; dimension < array.Rank; dimension++ )
        {
            dimensions[dimension] = array.GetLength( dimension );
            arrayLength = arrayLength == 0 ? dimensions[dimension] : arrayLength * dimensions[dimension];
        }

        return dimensions;
    }
}
