namespace Hyperbee.Collections.Tests;

// ThrowIf is a forwarding shim: on net10.0+ it calls the framework helpers, and on
// netstandard2.1 it throws directly. These tests pin the observable contract that
// must hold identically on every target, in particular the ParamName carried by
// CallerArgumentExpression.

[TestClass]
public class ThrowIfTests
{
    [TestMethod]
    public void Null_ShouldThrowWithInferredParamName()
    {
        object candidate = null;

        var ex = Assert.ThrowsExactly<ArgumentNullException>( () => ThrowIf.Null( candidate ) );

        Assert.AreEqual( nameof( candidate ), ex.ParamName );
    }

    [TestMethod]
    public void Null_ShouldThrowWithExplicitParamName()
    {
        var ex = Assert.ThrowsExactly<ArgumentNullException>( () => ThrowIf.Null( null, "source" ) );

        Assert.AreEqual( "source", ex.ParamName );
    }

    [TestMethod]
    public void Null_ShouldNotThrowForNonNull()
    {
        ThrowIf.Null( new object() );
    }

    [TestMethod]
    public void Negative_ShouldThrowWithInferredParamName()
    {
        var capacity = -1;

        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>( () => ThrowIf.Negative( capacity ) );

        Assert.AreEqual( nameof( capacity ), ex.ParamName );
        Assert.AreEqual( -1, ex.ActualValue );
    }

    [TestMethod]
    [DataRow( 0 )]
    [DataRow( 1 )]
    [DataRow( int.MaxValue )]
    public void Negative_ShouldNotThrowForNonNegative( int value )
    {
        ThrowIf.Negative( value );
    }

    [TestMethod]
    [DataRow( 0u, 0u )]
    [DataRow( 1u, 1u )]
    [DataRow( 2u, 2u )]
    [DataRow( 3u, 4u )]
    [DataRow( 16u, 16u )]
    [DataRow( 17u, 32u )]
    [DataRow( 1000u, 1024u )]
    [DataRow( 0x4000_0001u, 0x8000_0000u )]
    public void RoundUpToPowerOf2_ShouldMatchFrameworkSemantics( uint value, uint expected )
    {
        Assert.AreEqual( expected, BitOps.RoundUpToPowerOf2( value ) );
    }
}
