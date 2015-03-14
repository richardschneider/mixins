using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Sepia.Mixins
{
    /// <summary>
    ///   XML serialisation/deserialisation test for <see cref="Mixin"/>.
    /// </summary>
    [TestClass]
    public class XmlTest
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

        public static string Serialize<T>(T value)
        {

            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {

            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        [TestMethod]
        public void Mixin_XML_Same_As_Object()
        {
            var me = new Contact { Name = "me", MailTo = "me@somewhere.org" };
            dynamic mixin = new Mixin().With(me);

            var xml1 = Serialize(me);
            var xml2 = Serialize(mixin);
            Assert.AreEqual(xml1, xml2);
        }

    }
}
