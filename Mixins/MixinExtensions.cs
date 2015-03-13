using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Sepia.Mixins
{
    /// <summary>
    ///   A composite object.
    /// </summary>
    public static class MixinExtensions
    {
        public static ExpandoObject With(this ExpandoObject expando, object o)
        {
            var x = (IDictionary<string, object>)expando;
            foreach (var property in o.GetType().GetProperties())
            {
                x[property.Name] = property.GetValue(o, null);
            }
            return expando;
        }

        public static ExpandoObject With(this ExpandoObject expando, string name, object o)
        {
            ((IDictionary<string, Object>)expando)[name] = o;
            return expando;
        }
    }
}
