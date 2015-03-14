using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Sepia.Dynamic
{
    public partial class Mixin : IXmlSerializable
    {
        static readonly Type[] simpleXmlTypes = new[] {
            typeof(string), typeof(DateTime), typeof(DateTimeOffset), typeof(Enum), 
            typeof(decimal), typeof(Guid)
        };

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer)
        {
            // Write the instances.
            foreach (var instance in instances)
            {
                new XmlSerializer(instance.GetType()).Serialize(writer, instance);
            }

            // Write the extra properties.
            foreach (var kvp in members)
            {
                if (kvp.Value == null)
                    continue;
                var type = kvp.Value.GetType();
                if (type.IsPrimitive || simpleXmlTypes.Contains(type))
                {
                    writer.WriteStartElement(kvp.Key);
                    writer.WriteValue(kvp.Value);
                    writer.WriteEndElement();
                }
                else
                {
                    var root = new XmlRootAttribute(kvp.Key);
                    new XmlSerializer(type, root).Serialize(writer, kvp.Value);
                }
            }
        }

    }
}
