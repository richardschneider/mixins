using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;

namespace Sepia.Dynamic
{
    /// <summary>
    ///   Contains a combination of members from other class instances and runtime properties.
    /// </summary>
    public partial class Mixin : DynamicObject
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

        /// <summary>
        ///   Converts the dynamic mixin into a specific type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to cast to.</typeparam>
        /// <returns>
        ///   An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///   If an instance of <typeparamref name="T"/> is already associated with the mixin (via the <see cref="With(object)"/>
        ///   method, then it is returned.  Otherwise, a new instance of <typeparamref name="T"/> is created and any property is
        ///   set that has a corresponding name in the mixin.
        ///   
        ///   The <b>Mixin</b> is bound to the returned instance.  Any changes to an instance property is also reflected
        ///   in the mixin object.
        /// </remarks>
        public T As<T>() where T : new()
        {
            // Is an instance of this type already available?
            var instance = instances.OfType<T>().FirstOrDefault();
            if (instance != null)
                return instance;

            // Create new instance and populate the properties.
            instance = new T();
            foreach (var memberName in members.Keys.ToArray())
            {
                var info = instance.GetType().GetProperty(memberName);
                if (info != null && info.CanWrite)
                {
                    var value = members[memberName];
                    if (info.PropertyType == value.GetType())
                        info.SetValue(instance, value, null);
                    else
                        info.SetValue(instance, Convert.ChangeType(value, info.PropertyType, CultureInfo.InvariantCulture), null);
                    members.Remove(memberName);
                }
            }

            this.With(instance);

            return instance;
        }

        /// <inheritdoc />
        /// <remarks>
        ///   This method is implemented by using deferred execution. The immediate return value is an object that 
        ///   stores all the information that is required to perform the action. The query represented by this method 
        ///   is not executed until the object is enumerated either by calling its <b>GetEnumerator</b> method directly 
        ///   or by using <c>foreach</c>.
        /// </remarks>
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
                    if (info.PropertyType == value.GetType())
                        info.SetValue(instance, value, null);
                    else
                        info.SetValue(instance, Convert.ChangeType(value, info.PropertyType, CultureInfo.InvariantCulture), null);
                    return true;
                }
            }

            members[binder.Name] = value;
            return true;
        }
    }
}
