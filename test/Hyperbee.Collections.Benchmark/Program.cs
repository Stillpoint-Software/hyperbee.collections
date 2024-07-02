using BenchmarkDotNet.Running;
using static Hyperbee.Collections.Benchmark.BenchmarkConfig;

namespace Hyperbee.Collections.Benchmark;

internal class Program
{
    static void Main( string[] args )
    {
        BenchmarkSwitcher.FromAssembly( typeof( Program ).Assembly ).Run( args, new Config() );
    }
}
