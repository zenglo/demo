using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlSerializerDemo
{
    [XmlRoot("MyFoo")]
    public class Foo
    {
        [XmlElement("ID")]
        public int Id { get; set; }
        [XmlAttribute("attrDate")]
        public DateTime Date { get; set; }

        [XmlIgnore]
        public string IgnoreValue { get; set; }
        [XmlArray("MyBars")]
        [XmlArrayItem("Bar")]
        public List<BaseBar> Bars { get; set; }
    }

    public class BaseBar
    {
        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlIgnore]
        public string IgnoreValue { get; set; }

        [XmlElement("attrDate")]
        public DateTime Date { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }

    public class ABar : BaseBar
    {
        [XmlElement("AProEle")]
        public string APro { get; set; }
        [XmlIgnore]
        public string AIgnoreValue { get; set; }
    }

    public class BBar : BaseBar
    {
        [XmlAttribute("AProAttr")]
        public string BPro { get; set; }

        [XmlIgnore]
        public string BIgnoreValue { get; set; }
    }
}
