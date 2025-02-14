using Hyperbee.Collections.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests;

[TestClass]
public class LinkedDictionaryTests
{
    private const char Separator = ',';

    [DataTestMethod]
    [DataRow("aa1,bb1,cc1,dd1", "aa2,bb2,cc2,dd2", "0:aa2,0:bb2,0:cc2,0:dd2,1:aa1,1:bb1,1:cc1,1:dd1")]
    public void Should_select_all(string input1, string input2, string output)
    {
        var d1 = CreateDictionary(input1);
        var d2 = CreateDictionary(input2);

        var ld = new LinkedDictionary<string, string>();

        ld.Push(d1);
        ld.Push(d2);

        var expected = CreateArray(output);
        var result = ld.Select((offset, pair) => $"{offset}:{pair.Key}", LinkedNode.All).OrderBy(x => x).ToArray();

        CollectionAssert.AreEquivalent(expected, result);
    }

    [DataTestMethod]
    [DataRow("aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee")]
    public void Should_select_current(string input1, string input2, string output)
    {
        var d1 = CreateDictionary(input1);
        var d2 = CreateDictionary(input2);

        var ld = new LinkedDictionary<string, string>();

        ld.Push(d1);
        ld.Push(d2);

        var expected = CreateArray(output);
        var result = ld.Select((offset, pair) => $"{offset}:{pair.Key}", LinkedNode.Current).OrderBy(x => x).ToArray();

        CollectionAssert.AreEquivalent(expected, result);
    }

    [DataTestMethod]
    [DataRow("aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee,1:bb,1:cc")]
    public void Should_select_single(string input1, string input2, string output)
    {
        var d1 = CreateDictionary(input1);
        var d2 = CreateDictionary(input2);

        var ld = new LinkedDictionary<string, string>();

        ld.Push(d1);
        ld.Push(d2);

        var expected = CreateArray(output);
        var result = ld.Select((offset, pair) => $"{offset}:{pair.Key}", LinkedNode.Single).OrderBy(x => x).ToArray();

        CollectionAssert.AreEquivalent(expected, result);
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee" )]
    public void Should_add_current( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push();  // empty new scope

        // manually add values
        foreach ( var pair in d2 )
        {
            ld.Add( LinkedNode.Current, pair.Key, pair.Value );
        }

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.Current ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "ee", "0:ee,1:aa,1:bb,1:cc,1:dd" )]
    public void Should_add_single( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push();  // empty new scope

        // manually add values
        foreach ( var pair in d2 )
        {
            ld.Add( LinkedNode.Single, pair.Key, pair.Value );
        }

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.Single ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "bb" )]
    [ExpectedException( typeof( ArgumentException ), "Key already exists." )]
    public void Should_add_single_with_same_key( string input1, string input2 )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push();  // empty new scope

        // manually add values
        foreach ( var pair in d2 )
        {
            ld.Add( LinkedNode.Single, pair.Key, pair.Value );
        }
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "0:aa,0:dd,0:ee,1:aa,1:bb,1:cc,1:dd" )]
    public void Should_add_all( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push();  // empty new scope

        // manually add values
        foreach ( var pair in d2 )
        {
            ld.Add( LinkedNode.All, pair.Key, pair.Value );
        }

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.All ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [TestMethod]
    public void Should_clear_without_exception_when_empty()
    {
        var ld = new LinkedDictionary<string, string>();
        ld.Clear();
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee" )]
    public void Should_clear_all_items( string input1, string input2 )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        ld.Clear( LinkedNode.All );

        Assert.AreEqual(0, ld.Count);
    }

    [DataTestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "1:aa,1:bb,1:cc,1:dd" )]
    public void Should_clear_current_node( string input1, string input2, string output )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        ld.Clear( LinkedNode.Current );

        var expected = CreateArray( output );
        var result = ld.Select( ( offset, pair ) => $"{offset}:{pair.Key}", LinkedNode.Single ).OrderBy( x => x ).ToArray();

        CollectionAssert.AreEquivalent( expected, result );
    }

    [TestMethod]
    public void Should_remove_item()
    {
        var d1 = CreateDictionary("aa,bb,cc,dd");
        var ld = new LinkedDictionary<string, string>();

        ld.Push(d1);
        ld.Remove("bb");

        var expected = CreateArray("aa,cc,dd");
        var result = ld.Select((offset, pair) => pair.Key, LinkedNode.Current).OrderBy(x => x).ToArray();

        CollectionAssert.AreEquivalent(expected, result);
    }

    [TestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "dd", true )]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "bb", false )]
    public void Should_contain_key_current( string input1, string input2, string key, bool expected )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var result = ld.ContainsKey( LinkedNode.Current, key );

        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,ee", "dd", true )]
    [DataRow( "aa,bb,cc", "aa,ee,dd", "dd", true )]
    [DataRow( "aa,cc,dd", "aa,dd,ee", "bb", false )]
    public void Should_contain_key_single( string input1, string input2, string key, bool expected )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var result = ld.ContainsKey( LinkedNode.Single, key );

        Assert.AreEqual( expected, result );
    }

    [TestMethod]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "dd", true )]
    [DataRow( "aa,bb,cc,dd", "aa,dd,ee", "ff", false )]
    public void Should_contain_key_all( string input1, string input2, string key, bool expected )
    {
        var d1 = CreateDictionary( input1 );
        var d2 = CreateDictionary( input2 );

        var ld = new LinkedDictionary<string, string>();

        ld.Push( d1 );
        ld.Push( d2 );

        var result = ld.ContainsKey( LinkedNode.All, key );

        Assert.AreEqual( expected, result );
    }

    // Helpers

    private static string[] CreateArray(string input)
    {
        return input.Split(Separator);
    }

    private static Dictionary<string, string> CreateDictionary(string input)
    {
        var collection = CreateArray(input)
            .Select(x => new KeyValuePair<string, string>(x, x))
            .ToArray();

        return new Dictionary<string, string>(collection);
    }


}
