using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace Hyperbee.Collections;

// base class to give us the ability to easily create dynamic dictionaries
// with specific value types

public abstract class DynamicDictionaryBase<TValue> : DynamicObject, IEquatable<DynamicDictionaryBase<TValue>>, IEnumerable<KeyValuePair<string, TValue>>
{
    protected readonly IDictionary<string, TValue> _values = new Dictionary<string, TValue>();
    public IDictionary<string, TValue> Source => _values;

    public virtual dynamic this[string name]
    {
        get
        {
            _values.TryGetValue( name, out var member );
            return member;
        }

        set => _values[name] = value;
    }

    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {
        if ( _values.TryGetValue( binder.Name, out var value ) )
        {
            result = value;
            return true;
        }

        result = default;
        return false;
    }

    public override bool TrySetMember( SetMemberBinder binder, object value )
    {
        this[binder.Name] = value;
        return true;
    }

    public bool Equals( DynamicDictionaryBase<TValue> other )
    {
        if ( other is null )
            return false;

        return ReferenceEquals( this, other ) || Equals( other._values, _values );
    }

    public override bool Equals( object obj )
    {
        if ( obj is null )
            return false;

        if ( ReferenceEquals( this, obj ) )
            return true;

        return obj.GetType() == typeof( DynamicDictionary ) && Equals( (DynamicDictionary) obj );
    }

    public override int GetHashCode() => _values?.GetHashCode() ?? 0;

    public override IEnumerable<string> GetDynamicMemberNames() => _values.Keys;

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class DynamicDictionary : DynamicDictionaryBase<object>
{
    public static DynamicDictionary FromObject( object instance, bool includeNonPublic = false )
    {
        ArgumentNullException.ThrowIfNull( instance );

        var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        if ( includeNonPublic )
            bindingAttr |= BindingFlags.NonPublic;

        var properties = instance.GetType().GetProperties( bindingAttr ).Where( p => p.CanRead && p.CanWrite );

        var dict = new DynamicDictionary();

        foreach ( var member in properties )
        {
            dict.Source[member.Name] = member.GetValue( instance );
        }

        return dict;
    }
}
