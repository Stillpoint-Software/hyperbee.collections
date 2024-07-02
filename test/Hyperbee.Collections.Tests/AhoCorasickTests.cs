using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyperbee.Collections.Tests;

[TestClass]
public class AhoCorasickTests
{
    private static string Match<TType>( FindResult<TType> match, string source ) => source.Substring( match.Index, match.Length );

    [TestMethod]
    public void Should_find_entries()
    {
        const string haystack = "all your bbbaselless belongs to us base base!";

        var trie = new AhoCorasickTrie();
        trie.Add( "all" );
        trie.Add( "bbaselless" );
        trie.Add( "base" );
        trie.Add( "ase" );
        trie.Add( "sell" );

        var matches = trie.Find( haystack ).ToArray();

        Assert.AreEqual( 9, matches.Length );

        Assert.IsTrue( "all" == Match( matches[0], haystack ) && 0 == matches[0].Index );
        Assert.IsTrue( "base" == Match( matches[1], haystack ) && 11 == matches[1].Index );
        Assert.IsTrue( "ase" == Match( matches[2], haystack ) && 12 == matches[2].Index );
        Assert.IsTrue( "sell" == Match( matches[3], haystack ) && 13 == matches[3].Index );
        Assert.IsTrue( "bbaselless" == Match( matches[4], haystack ) && 10 == matches[4].Index );
        Assert.IsTrue( "base" == Match( matches[5], haystack ) && 35 == matches[5].Index );
        Assert.IsTrue( "ase" == Match( matches[6], haystack ) && 36 == matches[6].Index );
        Assert.IsTrue( "base" == Match( matches[7], haystack ) && 40 == matches[7].Index );
        Assert.IsTrue( "ase" == Match( matches[8], haystack ) && 41 == matches[8].Index );
    }

    [TestMethod]
    public void Should_find_selected_entries()
    {
        const string haystack = "all your bbbaselless belongs to us base base!";

        var trie = new AhoCorasickTrie();
        trie.Add( "all" );
        trie.Add( "bbaselless" );
        trie.Add( "base" );
        trie.Add( "ase" );
        trie.Add( "sell" );

        var matches = trie.Find( haystack ).Where( x => x.Overlap == 0 ).ToArray();

        Assert.AreEqual( 4, matches.Length );

        Assert.IsTrue( "all" == Match( matches[0], haystack ) && 0 == matches[0].Index );
        Assert.IsTrue( "bbaselless" == Match( matches[1], haystack ) && 10 == matches[1].Index );
        Assert.IsTrue( "base" == Match( matches[2], haystack ) && 35 == matches[2].Index );
        Assert.IsTrue( "base" == Match( matches[3], haystack ) && 40 == matches[3].Index );
    }

    [TestMethod]
    public void Should_find_selected1_entries()
    {
        const string haystack = "all your Base belongs to us! living in a tent fortification base.";

        var counter = new Dictionary<string, int>();

        var trie = new AhoCorasickTrie<string>( true );
        trie.Add( "base", "key_base" );
        trie.Add( "fortification", "key_base" );
        trie.Add( "tent", "key_tent" );

        var matches = trie.Find( haystack )
            .Where( x =>
            {
                counter.TryGetValue( x.Tag, out var value );
                value++;
                counter[x.Tag] = value;

                return value < 3;
            } )
            .ToArray();

        Assert.AreEqual( 3, matches.Length );

        Assert.IsTrue( Match( matches[0], haystack ) == "Base" && "key_base" == matches[0].Tag && 9 == matches[0].Index );
        Assert.IsTrue( Match( matches[1], haystack ) == "tent" && "key_tent" == matches[1].Tag && 41 == matches[1].Index );
        Assert.IsTrue( Match( matches[2], haystack ) == "fortification" && "key_base" == matches[2].Tag && 46 == matches[2].Index );
    }

    [TestMethod]
    public void Should_find_caseinsensistive_entries()
    {
        const string haystack = "all your Base belongs to us base!";

        var trie = new AhoCorasickTrie( true );
        trie.Add( "base" );

        var matches = trie.Find( haystack ).ToArray();

        Assert.AreEqual( 2, matches.Length );
    }

    [TestMethod]
    public void Should_return_associated_value()
    {
        const string haystack = "all your base are belongs to us!";
        var words = new[] { "belongs", "base" };

        var trie = new AhoCorasickTrie<int>();

        for ( var i = 0; i < words.Length; i++ )
            trie.Add( words[i], i );

        var matches = trie.Find( haystack );
        var lines = matches.Select( x => x.Tag ).ToArray();

        Assert.AreEqual( 2, lines.Length );
        Assert.AreEqual( 1, lines[0] );
        Assert.AreEqual( 0, lines[1] );
    }

    [TestMethod]
    public void Should_return_projected_value()
    {
        const string haystack = "all your base are belongs to us!";

        var guid0 = Guid.NewGuid();
        var guid1 = Guid.NewGuid();
        var guidRef = Guid.NewGuid();

        var trie = new AhoCorasickTrie<Guid>();
        trie.Add( "belongs", guid0 );
        trie.Add( "base", guid1 );

        var matches = trie.Find( haystack )
            .Select( x => new
            {
                x.Index,
                x.Length,
                Word = haystack.Substring( x.Index, x.Length ),
                GuidTag = x.Tag,
                GuidRef = guidRef
            } )
            .ToArray();

        Assert.AreEqual( 2, matches.Length );
        Assert.AreEqual( "base", matches[0].Word );
        Assert.AreEqual( guid1, matches[0].GuidTag );
        Assert.AreEqual( guidRef, matches[1].GuidRef );
    }
}
