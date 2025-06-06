﻿using BenchmarkDotNet.Attributes;
using Hyperbee.Collections.Extensions;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
#pragma warning disable CA1806

namespace Hyperbee.Collections.Benchmark;
public class CollectionsBenchmark
{
    private const char Separator = ',';

    [Params( "aa1,bb1,cc1,dd1", "aa2,bb2,cc2,dd2" )]
    public string List { get; set; }

    [Benchmark]
    public void LinkedDirectorySelectAll()
    {
        var d1 = CreateDictionary( List );
        var d2 = CreateDictionary( List );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        string output = string.Empty;

        var expected = CreateArray( output );
        ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.All ).OrderBy( x => x ).ToArray();
    }

    public void LinkedDirectorySelectCurrent()
    {
        var d1 = CreateDictionary( List );
        var d2 = CreateDictionary( List );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        string output = string.Empty;

        var expected = CreateArray( output );
        ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.Current ).OrderBy( x => x ).ToArray();
    }

    // Helpers

    private static string[] CreateArray( string input )
    {
        return input.Split( Separator );
    }

    private static Dictionary<string, string> CreateDictionary( string input )
    {
        var collection = CreateArray( input )
            .Select( x => new KeyValuePair<string, string>( x, x ) )
            .ToArray();

        return new Dictionary<string, string>( collection );
    }
}
