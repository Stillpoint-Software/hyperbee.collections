#region License
//
// Adapted from
//
// Implementation of Efficient bit manipulation routines
//
// [1] http://graphics.stanford.edu/~seander/bithacks.htm
//
// Released to the Public Domain
// 
// Original work by Sean Eron Anderson
//
#endregion

// FIX: Pulled from Hyperbee.Core which is not OpenSource yet.
using System.Runtime.CompilerServices;

namespace Hyperbee.Collections;

internal static class BitTweaks
{
    // The number of set bits.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountBits( uint n )
    {
        n -= ((n >> 1) & 0x55555555);
        n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
        n = (n + (n >> 4)) & 0x0F0F0F0F;

        return (int) ((n * 0x01010101) >> 24);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountBits( ulong n )
    {
        n -= ((n >> 1) & 0x5555555555555555);
        n = (n & 0x3333333333333333) + ((n >> 2) & 0x3333333333333333);
        n = (n + (n >> 4)) & 0xF0F0F0F0F0F0F0F;

        return (int) ((n * 0x101010101010101) >> 56);
    }

    // The total number of leading zero bits.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountLeadingZeroBits( int n )
    {
        int c = 0;
        int y = n;

L:
        if ( n < 0 )
            return c;

        if ( y == 0 )
            return 32 - c;

        c += 1;
        n <<= 1;
        y >>= 1;
        goto L;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountLeadingZeroBits( long n )
    {
        int c = 0;
        long y = n;

L:
        if ( n < 0 )
            return c;

        if ( y == 0 )
            return 64 - c;

        c += 1;
        n <<= 1;
        y >>= 1;
        goto L;
    }

    // Count the number of zero bits trailing the LSB.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountTrailingZeroBits( int n )
    {
        n = ~n & (n - 1); // Clears the lowest set bit and above, and then sets all lower bits. 

        // count
        n -= ((n >> 1) & 0x55555555);
        n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
        n = (n + (n >> 4)) & 0x0F0F0F0F;
        return (n * 0x01010101) >> 24;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int CountTrailingZeroBits( long n )
    {
        n = ~n & (n - 1); // Clears the lowest set bit and above, and then sets all lower bits. 

        // count
        n -= ((n >> 1) & 0x5555555555555555);
        n = (n & 0x3333333333333333) + ((n >> 2) & 0x3333333333333333);
        n = (n + (n >> 4)) & 0xF0F0F0F0F0F0F0F;
        return (int) ((n * 0x101010101010101) >> 56);
    }

    // The result of dividend / divisor rounded up to the next whole integer.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int DivideAndRoundUp( int dividend, int divisor ) => (dividend + (divisor - 1)) / divisor;

    // The most significant set bit value.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint GetMsb( uint n )
    {
        n |= n >> 1;
        n |= n >> 2;
        n |= n >> 4;
        n |= n >> 8;
        n |= n >> 16;

        return n - (n >> 1);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong GetMsb( ulong n )
    {
        n |= n >> 1;
        n |= n >> 2;
        n |= n >> 4;
        n |= n >> 8;
        n |= n >> 16;
        n |= n >> 32;

        return n - (n >> 1); //( n & ~( n >> 1 ) );
    }

    // The least significant set bit value.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetLsb( int n )
    {
        return n & (~n + 1); // clear all bits except the lowest set bit
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long GetLsb( long n )
    {
        return n & (~n + 1); // clear all bits except the lowest set bit
    }

    // The least significant unset bit value.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetLszb( int n )
    {
        return ~n & (n + 1);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long GetLszb( long n )
    {
        return ~n & (n + 1);
    }

    // Clear the lowest (least significant) bit.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int ClearLsb( int n )
    {
        return n & (n - 1);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long ClearLsb( long n )
    {
        return n & (n - 1);
    }

    // Set the lowest (least significant) 0 bit.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int SetLszb( int n )
    {
        return n | (n + 1);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long SetLszb( long n )
    {
        return n | (n + 1);
    }

    // Get a bitmask of all the bits trailing the lowest set bit.
    // 
    // If a zero is passed the mask will return all bits set.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetLsbTrailingMask( int n )
    {
        return ~n & (n - 1);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long GetLsbTrailingMask( long n )
    {
        return ~n & (n - 1);
    }

    // Query if 'n' is power of two.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsPowerOfTwo( uint n )
    {
        return (n & (n - 1)) == 0;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsPowerOfTwo( ulong n )
    {
        return (n & (n - 1)) == 0;
    }

    // Round up to next power of two.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint RoundUpToNextPowerOfTwo( uint n )
    {
        n--; // subtract 1 to handle edge cases (value is a power of two)
        n |= n >> 1; // handle  2 bit numbers
        n |= n >> 2; // handle  4 bit numbers
        n |= n >> 4; // handle  8 bit numbers
        n |= n >> 8; // handle 16 bit numbers
        n |= n >> 16; // handle 32 bit numbers
        n++;

        return n;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int RoundUpToNextPowerOfTwo( int n )
    {
        n--; // subtract 1 to handle edge cases (value is a power of two)
        n |= n >> 1; // handle  2 bit numbers
        n |= n >> 2; // handle  4 bit numbers
        n |= n >> 4; // handle  8 bit numbers
        n |= n >> 8; // handle 16 bit numbers
        n |= n >> 16; // handle 32 bit numbers
        n++;

        return n;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong RoundUpToNextPowerOfTwo( ulong n )
    {
        n--; // subtract 1 to handle edge cases (value is a power of two)
        n |= n >> 1; // handle  2 bit numbers
        n |= n >> 2; // handle  4 bit numbers
        n |= n >> 4; // handle  8 bit numbers
        n |= n >> 8; // handle 16 bit numbers
        n |= n >> 16; // handle 32 bit numbers
        n |= n >> 32; // handle 64 bit numbers
        n++;

        return n;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long RoundUpToNextPowerOfTwo( long n )
    {
        n--; // subtract 1 to handle edge cases (value is a power of two)
        n |= n >> 1; // handle  2 bit numbers
        n |= n >> 2; // handle  4 bit numbers
        n |= n >> 4; // handle  8 bit numbers
        n |= n >> 8; // handle 16 bit numbers
        n |= n >> 16; // handle 32 bit numbers
        n |= n >> 32; // handle 64 bit numbers
        n++;

        return n;
    }

    // Reverse byte order
    //
    // Reverse the value's endianess.

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort ReverseBytes( ushort n ) // 16bit
    {
        return (ushort) ((n & 0xFFU) << 8 | (n & 0xFF00U) >> 8);
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint ReverseBytes( uint n ) // 32bit
    {
        return (n & 0x000000FFU) << 24 | (n & 0x0000FF00U) << 8 |
               (n & 0x00FF0000U) >> 8 | (n & 0xFF000000U) >> 24;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong ReverseBytes( ulong n ) // 64bit
    {
        return (n & 0x00000000000000FFUL) << 56 | (n & 0x000000000000FF00UL) << 40 |
               (n & 0x0000000000FF0000UL) << 24 | (n & 0x00000000FF000000UL) << 8 |
               (n & 0x000000FF00000000UL) >> 8 | (n & 0x0000FF0000000000UL) >> 24 |
               (n & 0x00FF000000000000UL) >> 40 | (n & 0xFF00000000000000UL) >> 56;
    }
}
