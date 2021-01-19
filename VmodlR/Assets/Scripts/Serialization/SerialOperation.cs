using System.Xml.Serialization;

[XmlRoot("Operation")]
public class SerialOperation
{
    [XmlAttribute("Content")]
    public string content;
}
