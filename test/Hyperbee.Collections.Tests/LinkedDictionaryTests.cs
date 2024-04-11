using System.Collections.Generic;
using System.Linq;
using Hyperbee.Collections.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests;

[TestClass]
public class LinkedDictionaryTests
{
    private const char Separator = ',';

    [DataTestMethod]
    [DataRow( "aa1,bb1,cc1,dd1", "aa2,bb2,cc2,dd2", "0:aa2,0:bb2,0:cc2,0:dd2,1:aa1,1:bb1,1:cc1,1:dd1" )]
    public void Should_select_all( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyValueOptions.All ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee" )]
    public void Should_select_current( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyValueOptions.Current ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee,1:bb,1:cc" )]
    public void Should_select_first( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyValueOptions.First ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    private static string[] CreateArray( string input )
    {
        return input.Split( Separator );
    }

    private static IDictionary<string, string> CreateDictionary( string input )
    {
        var collection = CreateArray( input )
            .Select( x => new KeyValuePair<string, string>( x, x ) )
            .ToArray();

        return new Dictionary<string, string>( collection );
    }
}
