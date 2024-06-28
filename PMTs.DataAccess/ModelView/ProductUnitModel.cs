using System.Collections.Generic;
using System.Xml.Serialization;

namespace PMTs.DataAccess.ModelView
{
    [XmlRoot("productionUnit")]
    public class ProductUnitModel
    {
        [XmlElement("production")]
        public Production Production { get; set; }

        [XmlArrayItem("name")]
        public List<Name> Name { get; set; }
    }

    public class Production
    {
        [XmlAttribute]
        public string Unit { get; set; }
    }

    public class Name
    {
        [XmlAttribute]
        public string Display { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}
