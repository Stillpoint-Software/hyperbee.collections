#if NETSTANDARD

// Compiler-required types that netstandard does not define. Both are shape-only
// contracts: the compiler needs IsExternalInit to exist by name before it will
// emit an init accessor, and it reads ParameterName off
// CallerArgumentExpressionAttribute. An empty class and one property are
// therefore the whole contract.
//
// These replace the Polyfill package, which supplied them but compiled its
// entire polyfill surface into the assembly as unused internal types the C#
// compiler does not strip. Dropping it cut the netstandard2.1 asset from
// 370 KB to 59 KB. Hyperbee.Expressions made the same swap, measuring a
// comparable delta independently.
//
// Everything here is internal, so it cannot collide with a consumer's own
// shims or with the real types on a framework that has them.

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit;

    [AttributeUsage( AttributeTargets.Parameter, AllowMultiple = false, Inherited = false )]
    internal sealed class CallerArgumentExpressionAttribute( string parameterName ) : Attribute
    {
        public string ParameterName { get; } = parameterName;
    }
}

#endif
