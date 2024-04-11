#region License
//
// Match multiple phrases in text using an Aho Corasick Trie structure.
//
// [1] https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm
// [2] https://cp-algorithms.com/string/aho_corasick.html
// [3] https://www.toptal.com/algorithms/aho-corasick-algorithm
// [4] https://github.com/pdonald/aho-corasick
// 
// The MIT License
//
// Copyright (c) 2019 Brent Farmer
// Copyright (c) 2013 Pēteris Ņikiforovs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using System.Collections;
using System.Runtime.CompilerServices;

namespace Hyperbee.Collections;

public struct FindResult<TTag>
{
    public int Index;
    public int Length;
    public int Overlap;
    public TTag Tag;
}

// Let there be a set of strings with the total length m (sum of all lengths).
// The Aho-Corasick algorithm constructs a data structure similar to a trie with
// some additional links, and then constructs a finite state machine (automaton)
// in O(mk) time, where k is the size of the used alphabet.
//
// Search takes O(N + L + Z), where Z is the count of matches.

public class AhoCorasickTrie : AhoCorasickTrie<string>
{
    public AhoCorasickTrie()
    {
    }

    public AhoCorasickTrie( bool ignoreCase ) : base( ignoreCase )
    {
    }

    public void Add( string match ) => Add( match, default );
}

// Trie that will find strings or phrases and return values of type TData
// for each string or phrase found.
//
// type param "TData" is the type of the data returned when a match is found.

public class AhoCorasickTrie<TTag>( bool ignoreCase )
{
    private readonly Node _root = new( ignoreCase );
    private bool _built;

    public AhoCorasickTrie() : this( false )
    {
    }

    // Add a search phrase to the tree.
    //
    // A match phrase consists of letters. A node is built for each character.
    //
    // param "match" is the phrase that will be searched for.
    // param "tag" is the tag object that will be returned when a match is found.

    public void Add( string match, TTag tag )
    {
        _built = false;

        // build a branch for the match phrase, one letter at a time.
        // if a letter node doesn't exist, add it.

        var node = _root; // start at the root
        var matchLength = 0;

        foreach ( var c in match )
        {
            var child = node[c] ?? (node[c] = new Node( c, node, ignoreCase ));

            node = child;
            matchLength++;
        }

        // mark the end of the branch by adding a value that will be
        // returned when this word is found in a text.

        node.SetValue( (tag, matchLength) );
    }

    private void Build()
    {
        // construct suffix links using breadth-first-search.
        // https://en.wikipedia.org/wiki/Breadth-first_search

        var queue = new Queue<Node>();
        queue.Enqueue( _root );

        while ( queue.Count > 0 )
        {
            var current = queue.Dequeue();

            // visit children
            foreach ( var child in current )
                queue.Enqueue( child );

            // the suffix link of root is root
            if ( current == _root )
            {
                _root.Suffix = _root;
                continue;
            }

            // find the node's suffix link          
            Node nodeC;
            var node = current.Parent.Suffix;

            while ( (nodeC = node[current.Character]) == null && node != _root )
                node = node.Suffix;

            current.Suffix = nodeC == null || nodeC == current ? _root : nodeC; // suffix link of null and self are _root
        }

        _built = true;
    }

    public IEnumerable<FindResult<TTag>> Find( string text )
    {
        if ( !_built )
            Build();

        var node = _root;
        var endIndex = 0;

        foreach ( var c in text )
        {
            // while from the current node of the trie there is no transition
            // using the current letter (or until we reach root), follow the
            // suffix link

            Node nodeC;

            while ( (nodeC = node[c]) == null && node != _root )
                node = node.Suffix;

            node = nodeC ?? _root;

            var overlap = 0;
            for ( var t = node; t != _root; t = t.Suffix )
            {
                if ( t.Values != null )
                {
                    foreach ( var (tag, length) in t.Values )
                    {
                        var startIndex = endIndex - (length - 1);

                        var result = new FindResult<TTag>
                        {
                            Index = startIndex,
                            Length = length,
                            Overlap = overlap,
                            Tag = tag
                        };

                        yield return result;
                    }
                }

                overlap++;
            }

            endIndex++;
        }
    }

    private class Node : IEnumerable<Node>
    {
        private readonly Dictionary<char, Node> _children;
        private IList<(TTag Tag, int Length)> _values;

        public Node( bool ignoreCase )
        {
            var comparer = ignoreCase ? CharComparerOrdinalIgnoreCase.Comparer : null;
            _children = new Dictionary<char, Node>( comparer );
        }

        public Node( char character, Node parent, bool ignoreCase )
            : this( ignoreCase )
        {
            Character = character;
            Parent = parent;
        }

        public Node this[char c]
        {
            get => _children.GetValueOrDefault( c );
            set => _children[c] = value;
        }

        public void SetValue( (TTag Tag, int Length) item ) => (_values ??= []).Add( item );

        public char Character { get; }

        public Node Parent { get; }

        public Node Suffix { get; set; }

        public IEnumerable<(TTag Data, int Length)> Values => _values;

        public IEnumerator<Node> GetEnumerator() => _children.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Character.ToString();
    }
}


internal class CharComparerOrdinalIgnoreCase : IEqualityComparer<char>
{
    public static readonly IEqualityComparer<char> Comparer = new CharComparerOrdinalIgnoreCase();

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool Equals( char x, char y ) => char.ToLowerInvariant( x ) == char.ToLowerInvariant( y );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public int GetHashCode( char obj ) => char.ToLowerInvariant( obj ).GetHashCode();
}
