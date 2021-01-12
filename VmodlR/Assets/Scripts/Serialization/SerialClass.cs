using System.Xml.Serialization;

[XmlRoot("UML:Class")]
public class SerialClass 
{
    [XmlAttribute("name")]
    public string className;

    [XmlArray("Attributes"), XmlArrayItem("Attribute")]
    public SerialAttribute[] attributes;
    [XmlArray("Operations"), XmlArrayItem("Operations")]
    public SerialOperation[] operations;
}
