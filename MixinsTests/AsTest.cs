using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;

namespace Sepia.Dynamic
{
    /// <summary>
    ///   Conversion test for <see cref="Mixin"/>.
    /// </summary>
    [TestClass]
    public class AsTest
    {
        public class Contact
        {
            public string Name { get; set; }
            public string MailTo { get; set; }
        }

        public class Contact2 : Contact
        {
            public string Phone { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string Country { get; set; }
        }

        [TestMethod]
        public void Same_Instance_Is_Returned()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic mixin = new Mixin().With(me);

            Assert.AreSame(me, mixin.As<Contact>());
        }

        [TestMethod]
        public void Same_Instance_Is_Returned_For_Subclass()
        {
            var me = new Contact2 { Name = "me", MailTo = "me@somewhere.org", Phone = "+64 4 ..." };
            var mixin = new Mixin().With(me);

            Assert.AreSame(me, mixin.As<Contact>());
        }

        [TestMethod]
        public void New_Instance_Is_Populated()
        {
            var mixin = new Mixin()
                .With("Name", "me")
                .With("MailTo", "me@somewhere.org")
                .With("Phone", "+64 4 ...");
            var me = mixin.As<Contact2>();
            Assert.AreEqual("me", me.Name);
            Assert.AreEqual("me@somewhere.org", me.MailTo);
            Assert.AreEqual("+64 4 ...", me.Phone);
        }

    }
}
