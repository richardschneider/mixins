using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;

namespace Sepia.Mixins
{
    /// <summary>
    ///   Contains a combination of members from other class instances and runtime properties.
    /// </summary>
    public class Mixin : DynamicObject
    {
        Dictionary<string, object> members = new Dictionary<string, object>();
        List<object> instances = new List<object>();

        /// <summary>
        ///   Extends the <see cref="Mixin"/> with the public members of the specified object.
        /// </summary>
        /// <param name="instance">The <see cref="object"/> to add.</param>
        /// <returns>
        ///   <c>this</c> for a fluent style.
        /// </returns>
        public Mixin With(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException();
            if (instances.Contains(instance))
                throw new ArgumentException("The object is alread mixed in.");

            instances.Insert(0, instance);
            return this;
        }

        /// <summary>
        ///   Extends the <see cref="Mixin"/> with the other <b>Mixin</b>.
        /// </summary>
        /// <param name="other">The other <see cref="Mixin"/> to add.</param>
        /// <returns>
        ///   <c>this</c> for a fluent style.
        /// </returns>
        public Mixin With(Mixin other)
        {
            foreach (var name in other.members.Keys)
            {
                members[name] = other.members[name];
            }
            foreach (var instance in other.instances)
            {
                this.With(instance);
            }

            return this;
        }

        /// <summary>
        ///   Extends the <see cref="Mixin"/> with the with the specified property name and value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>
        ///   <c>this</c> for a fluent style.
        /// </returns>
        /// <remarks>
        ///   Adds the property with the specified <paramref name="name"/> and <paramref name="value"/> 
        ///   to the <b>Mixin</b>.
        /// </remarks>
        public Mixin With(string name, object value)
        {
            members[name] = value;
            return this;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return instances.SelectMany(i => i.GetType().GetMembers().Select(m => m.Name))
                .Union(members.Keys);
        }

        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (members.TryGetValue(binder.Name, out result))
                return true;

            foreach (var instance in instances)
            {
                var info = instance.GetType().GetProperty(binder.Name);
                if (info != null)
                {
                    result = info.GetValue(instance, null);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (members.ContainsKey(binder.Name))
            {
                members[binder.Name] = value;
                return true;
            }

            foreach (var instance in instances)
            {
                var info = instance.GetType().GetProperty(binder.Name);
                if (info != null)
                {
                    info.SetValue(instance, value, null);
                    return true;
                }
            }

            members[binder.Name] = value;
            return true;
        }
    }
}
