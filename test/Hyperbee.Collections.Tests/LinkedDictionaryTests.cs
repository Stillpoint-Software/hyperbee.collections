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
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyScope.All ).OrderBy( x => x ).ToArray();

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
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyScope.Current ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee,1:bb,1:cc" )]
    public void Should_select_closest( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", KeyScope.Closest ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [TestMethod]
    public void Should_clear_without_exception_when_empty()
    {
        var ld = new LinkedDictionary<string, string>();
        ld.Clear();
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
