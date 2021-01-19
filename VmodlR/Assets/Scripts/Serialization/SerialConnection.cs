using System.Xml.Serialization;

[XmlRoot("Connection")]
public class SerialConnection
{
    [XmlAttribute("OriginClassName")]
    public string originClassName;
    [XmlAttribute("TargetClassName")]
    public string targetClassName;

    [XmlAttribute("Type")]
    public ConnectorTypes connectorType;
}
