using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace Sepia.Mixins
{
    /// <summary>
    ///   Basic tests for <see cref="Sepia.Mixins.ExpandoExtensions"/>.
    /// </summary>
    [TestClass]
    public class WithTest
    {
        class Contact
        {
            public string Name { get; set; }
            public string MailTo { get; set; }
            protected string ProtectedProperty { get; set; }
            private string PrivateProperty { get; set; }
            public static string StaticProperty { get; set; }
            public string ReadOnly { get; private set; }
            public string WriteOnly { private get; set; }
        }

        class Contact2 : Contact
        {
            public string NickName { get; set; }
        }

        class Address
        {
            public string Street { get; set; }
            public string Country { get; set; }
        }

        [TestMethod]
        public void Property_Is_Added()
        {
            dynamic o = new ExpandoObject().With("X", 123);
            Assert.AreEqual(123, o.X);
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void Property_Is_Missing_Throws()
        {
            dynamic o = new ExpandoObject().With("X", 123);
            Assert.AreEqual(123, o.Y);
        }

        [TestMethod]
        public void Mixin_Property_Is_Fluent()
        {
            dynamic o = new ExpandoObject()
                .With("X", 123)
                .With("Z", "zeta");
            Assert.AreEqual(123, o.X);
            Assert.AreEqual("zeta", o.Z);
        }
        
        [TestMethod]
        public void Object_Properties_Are_Added()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            Assert.AreEqual("me", o.Name);
            Assert.AreEqual("me@somewhere.org", o.MailTo);
        }

        [TestMethod]
        public void Object_Properties_Must_Have_Public_Getter()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            var x = (IDictionary<string, object>)o;
            Assert.IsTrue(x.ContainsKey("Name"));
            Assert.IsTrue(x.ContainsKey("MailTo"));
            Assert.IsTrue(x.ContainsKey("ReadOnly"));
            Assert.IsFalse(x.ContainsKey("WriteOnly"));
        }

        [TestMethod]
        public void Inherited_Object_Properties_Are_Added()
        {
            var me = new Contact2 { Name = "me", MailTo = "me@somewhere.org", NickName = "baz" };
            dynamic o = new ExpandoObject().With(me);
            Assert.AreEqual("me", o.Name);
            Assert.AreEqual("me@somewhere.org", o.MailTo);
            Assert.AreEqual("baz", o.NickName);
        }

        [TestMethod]
        public void Anonymous_Classes_Are_Added()
        {
            var me = new { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            Assert.AreEqual("me", o.Name);
            Assert.AreEqual("me@somewhere.org", o.MailTo);
        }

        [TestMethod]
        public void Expandos_Are_Added()
        {
            dynamic me = new ExpandoObject();
            me.Name = "me";
            me.MailTo = "me@somewhere.org";

            dynamic o = ExpandoExtensions.With(new ExpandoObject(), me);
            Assert.AreEqual("me", o.Name);
            Assert.AreEqual("me@somewhere.org", o.MailTo);
        }

        [TestMethod]
        public void Protected_Object_Properties_Are_Not_Added()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            var x = (IDictionary<string, object>)o;
            Assert.IsTrue(x.ContainsKey("Name"));
            Assert.IsTrue(x.ContainsKey("MailTo"));
            Assert.IsFalse(x.ContainsKey("ProtectedProperty"));
        }

        [TestMethod]
        public void Private_Object_Properties_Are_Not_Added()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            var x = (IDictionary<string, object>)o;
            Assert.IsTrue(x.ContainsKey("Name"));
            Assert.IsTrue(x.ContainsKey("MailTo"));
            Assert.IsFalse(x.ContainsKey("PrivateProperty"));
        }

        [TestMethod]
        public void Static_Object_Properties_Are_Not_Added()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic o = new ExpandoObject().With(me);
            var x = (IDictionary<string, object>)o;
            Assert.IsTrue(x.ContainsKey("Name"));
            Assert.IsTrue(x.ContainsKey("MailTo"));
            Assert.IsFalse(x.ContainsKey("StaticProperty"));
        }

        [TestMethod]
        public void Object_Composition_Is_Fluent()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            var addr = new Address { Street = "1 Main St", Country = "NZL" };
            dynamic o = new ExpandoObject()
                .With(me)
                .With(addr);
            Assert.AreEqual("me", o.Name);
            Assert.AreEqual("me@somewhere.org", o.MailTo);
            Assert.AreEqual("1 Main St", o.Street);
            Assert.AreEqual("NZL", o.Country);
        }
    }
}
