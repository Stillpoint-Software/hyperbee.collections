using System.Collections.Specialized;

namespace Hyperbee.Collections.Extensions;

public static class NameValueCollectionExtensions
{
    public static IDictionary<string, object> ToDictionary( this NameValueCollection collection, StringComparer comparer = default )
    {
        comparer ??= StringComparer.OrdinalIgnoreCase;
        var values = new Dictionary<string, object>( comparer );

        foreach ( var key in collection.AllKeys )
        {
            var value = collection[key];

            if ( !values.ContainsKey( key! ) )
            {
                values[key] = value;
                continue;
            }

            if ( values[key] is IList<string> list )
            {
                list.Add( value );
                continue;
            }

            values[key] = new List<string>
            {
                (string) values[key],
                value
            };
        }

        return values;
    }
}
