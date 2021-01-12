using System.Xml.Serialization;

[XmlRoot("Attribute")]
public class SerialAttribute
{
    [XmlAttribute("Content")]
    public string content;
}
