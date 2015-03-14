using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using Newtonsoft.Json;

namespace Sepia.Mixins
{
    /// <summary>
    ///   JSON serialisation/deserialisation test for <see cref="Sepia.Mixins.ExpandoExtensions"/>.
    /// </summary>
    [TestClass]
    public class JsonTest
    {
        public class Contact
        {
            public string Name { get; set; }
            public string MailTo { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string Country { get; set; }
        }

        [TestMethod]
        public void Mixin_JSON_Same_As_Object()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic mixin = new ExpandoObject().With(me);

            var json1 = JsonConvert.SerializeObject(me);
            var json2 = JsonConvert.SerializeObject(mixin);
            Assert.AreEqual(json1, json2);
        }

    }
}
