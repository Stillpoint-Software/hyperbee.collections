namespace Hyperbee.Collections.Extensions;

public static class ArrayExtensions
{
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
