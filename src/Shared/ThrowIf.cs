using System.Runtime.CompilerServices;

namespace Hyperbee.Collections;

// Guard-clause helpers that work on every target framework.
//
// netstandard predates the static throw helpers on the exception types
// themselves (ArgumentNullException.ThrowIfNull, ArgumentOutOfRangeException
// .ThrowIfNegative), and C# cannot add static members to an existing type, so
// no polyfill package can supply them. On net10.0 and net11.0 every method here
// forwards to the framework implementation, so behavior on those targets is
// unchanged; only the netstandard build takes the local path.
//
// This is deliberately a forwarding helper rather than a type that shadows
// System.ArgumentNullException: a shadowing shim would have to derive from the
// real exception, and exact-type exception assertions (for example MSTest's
// Assert.ThrowsExactly<T>) would then fail for consumers.
//
// The name matches the equivalent helper in Hyperbee.Expressions so the two
// repositories stay parallel.

internal static class ThrowIf
{
    public static void Null( object value, [CallerArgumentExpression( nameof( value ) )] string paramName = null )
    {
#if NETSTANDARD
        if ( value is null )
            throw new ArgumentNullException( paramName );
#else
        ArgumentNullException.ThrowIfNull( value, paramName );
#endif
    }

    public static void Negative( int value, [CallerArgumentExpression( nameof( value ) )] string paramName = null )
    {
#if NETSTANDARD
        if ( value < 0 )
            throw new ArgumentOutOfRangeException( paramName, value, $"{paramName} ('{value}') must be a non-negative value." );
#else
        ArgumentOutOfRangeException.ThrowIfNegative( value, paramName );
#endif
    }
}
