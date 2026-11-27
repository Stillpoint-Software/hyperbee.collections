#if !NETSTANDARD
using System.Numerics;
#endif

namespace Hyperbee.Collections.Internal;

// netstandard has no System.Numerics.BitOperations. RoundUpToPowerOf2 is the
// only member this library uses, so this helper supplies just that one method
// rather than reproducing the type. On net10.0 and net11.0 it forwards to the
// intrinsic-backed framework implementation, so behavior on those targets is
// unchanged.

internal static class BitOps
{
    public static uint RoundUpToPowerOf2( uint value )
    {
#if NETSTANDARD
        // Hacker's Delight, Figure 3-3. Matches BitOperations.RoundUpToPowerOf2:
        // 0 rounds to 0, and inputs above 2^31 overflow to 0.
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
#else
        return BitOperations.RoundUpToPowerOf2( value );
#endif
    }
}
