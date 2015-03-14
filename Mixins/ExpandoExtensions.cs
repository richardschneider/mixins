using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;

namespace Sepia.Mixins
{
    /// <summary>
    ///   A composite object.
    /// </summary>
    public static class ExpandoExtensions
    {
        /// <summary>
        ///   Extends the <see cref="ExpandoObject"/> with the public properties of the specified object.
        /// </summary>
        /// <param name="mixin">The <see cref="ExpandoObject"/> to extend.</param>
        /// <param name="o">The <see cref="object"/> to add.</param>
        /// <returns>
        ///   The <paramref name="mixin"/> for a fluent style.
        /// </returns>
        /// <remarks>
        ///   All public gettable properties and fields of <paramref name="o"/> are added to the
        ///   <paramref name="mixin"/>.
        /// </remarks>
        public static ExpandoObject With(this ExpandoObject mixin, object o)
        {
            var x = (IDictionary<string, object>)mixin;
            var flags = BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.GetProperty
                | BindingFlags.Instance | BindingFlags.Public;
            foreach (var property in o.GetType().GetProperties(flags))
            {
                if (property.CanRead && property.GetGetMethod() != null)
                    x[property.Name] = property.GetValue(o, null);
            }
            return mixin;
        }

        /// <summary>
        ///   Extends the <see cref="ExpandoObject"/> with the with the specified properties.
        /// </summary>
        /// <param name="mixin">The <see cref="ExpandoObject"/> to extend.</param>
        /// <param name="properties">The key value pairs to add.</param>
        /// <returns>
        ///   The <paramref name="mixin"/> for a fluent style.
        /// </returns>
        /// <remarks>
        ///   All <paramref name="properties"/> are added to the <paramref name="mixin"/>.
        /// </remarks>
        public static ExpandoObject With(this ExpandoObject mixin, IEnumerable<KeyValuePair<string, object>> properties)
        {
            var x = (IDictionary<string, object>)mixin;
            foreach (var kvp in properties)
            {
                x[kvp.Key] = kvp.Value;
            }
            return mixin;
        }

        /// <summary>
        ///   Extends the <see cref="ExpandoObject"/> with the with the specified property name and value.
        /// </summary>
        /// <param name="mixin">The <see cref="ExpandoObject"/> to extend.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>
        ///   The <paramref name="mixin"/> for a fluent style.
        /// </returns>
        /// <remarks>
        ///   Adds the property with the specified <paramref name="name"/> and <paramref name="value"/> 
        ///   to the <paramref name="mixin"/>.
        /// </remarks>
        public static ExpandoObject With(this ExpandoObject mixin, string name, object value)
        {
            ((IDictionary<string, Object>)mixin)[name] = value;
            return mixin;
        }
    }
}
