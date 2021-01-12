using System.Xml.Serialization;

[XmlRoot("Model")]
public class SerialModel 
{
    [XmlArray("Classes"), XmlArrayItem("Class")]
    public SerialClass[] classes;

    [XmlArray("Connections"), XmlArrayItem("Connection")]
    public SerialConnection[] connections;
}
