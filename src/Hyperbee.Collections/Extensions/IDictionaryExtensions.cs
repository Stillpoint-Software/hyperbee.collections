using System.Collections;
using System.Dynamic;

namespace Hyperbee.Collections.Extensions;

public static class DictionaryExtensions
{
    public static ExpandoObject ToExpando( this IDictionary<string, object> dictionary )
    {
        // https://gist.github.com/theburningmonk/2221646

        var expandoObject = new ExpandoObject();
        var expandoDictionary = (IDictionary<string, object>) expandoObject;

        foreach ( var pair in dictionary )
        {
            switch ( pair.Value )
            {
                case IDictionary<string, object> objects:
                    {
                        var value = objects.ToExpando();
                        expandoDictionary.Add( pair.Key, value );
                        continue;
                    }

                case ICollection collection:
                    {
                        var items = new List<object>();

                        foreach ( var item in collection )
                        {
                            if ( item is IDictionary<string, object> objects )
                                items.Add( objects.ToExpando() );
                            else
                                items.Add( item );
                        }

                        expandoDictionary.Add( pair.Key, items );
                        continue;
                    }

                default:
                    expandoDictionary.Add( pair );
                    break;
            }
        }

        return expandoObject;
    }
}
