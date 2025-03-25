namespace Library;
using System.Xml;
using System.Globalization;

public class DomToolkid
{
    public XmlDocument _doc { get; set; }
    public string _path { get; set; }
    public DomToolkid(string xmlPath)
    {
        _doc = new XmlDocument();
        _path = xmlPath;
        if (File.Exists(_path))
        {
            _doc.Load(_path);
        }
    }
    public void saveDoc()
    {
        _doc.Save(_path);
    }

    public Transistor nodeShow(int id)
    {
        List<TransistorType> typeList = new List<TransistorType>();
        string transistorId = id.ToString("D3");
        XmlNode node = _doc.SelectSingleNode($"/root/transistor[@id='{transistorId}']");
        if (node != null)
        {
            XmlNode typeNodes = node.SelectSingleNode("types");
            foreach(XmlNode typeNode in typeNodes.ChildNodes)
            {
                if(!Enum.TryParse(typeNode.InnerText, out TransistorType type))
                {
                    Console.WriteLine("type reading error");
                }
                typeList.Add(type);
            }

            string nodeName = node.SelectSingleNode("name").InnerText;

            if(float.TryParse(node.SelectSingleNode("voltage").InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out float nodeVoltage))
            {
                Console.WriteLine("voltage readed");
            }
            else
            {
                Console.WriteLine("voltage reading failed");
            }

            if(float.TryParse(node.SelectSingleNode("amperage").InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out float nodeAmperage))
            {
                Console.WriteLine("amperage readed");
            }
            else
            {
                Console.WriteLine("amperage reading failed");
            }

            if(float.TryParse(node.SelectSingleNode("price").InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out float nodePrice))
            {
                Console.WriteLine("price readed");
            }
            else
            {
                Console.WriteLine("voltage reading failed");
            }
            string nodeCountry = node.SelectSingleNode("country").InnerText;
            Transistor trn = new Transistor(id, nodeName, typeList, nodeVoltage, nodeAmperage, nodePrice, nodeCountry);
            Console.WriteLine("transistor readed\n");
            return trn;
        }
        else 
        {
            Console.WriteLine("nodeShow error.");
            return null;
        }
    }

    public void nodeCreate(Transistor trn)
    {
        XmlElement transistor = _doc.CreateElement("transistor");
        string transistorId = trn._id.ToString("D3");
        transistor.SetAttribute("id", transistorId);

        XmlElement name = _doc.CreateElement("name");
        name.InnerText = trn._name;
        transistor.AppendChild(name);

        XmlElement types = _doc.CreateElement("types");
        foreach(TransistorType type in trn._types)
        {
            XmlElement xtype = _doc.CreateElement("type");
            string stType = type.ToString();
            xtype.InnerText = stType;
            types.AppendChild(xtype);
        }
        transistor.AppendChild(types);

        transistor.AppendChild(CreateMeasurementElement(_doc, "voltage", trn._voltage, "V"));
        transistor.AppendChild(CreateMeasurementElement(_doc, "amperage", trn._amperage, "A"));
        transistor.AppendChild(CreateMeasurementElement(_doc, "price", trn._price, "USD"));

        XmlElement country = _doc.CreateElement("country");
        country.InnerText = trn._name;
        transistor.AppendChild(country);

        _doc.DocumentElement.AppendChild(transistor);
        saveDoc();
        Console.WriteLine("new element created.");
    }

    public void nodeRemove(int id)
    {
        string transistorId = id.ToString("D3");
        XmlNode node = _doc.SelectSingleNode($"/root/transistor[@id='{transistorId}']");
        if(node != null)
        {
            XmlNode parent = node.ParentNode;
            parent.RemoveChild(node);
            saveDoc();
            Console.WriteLine("element removed");
        }
        else
        {
            Console.WriteLine("Element not found. Remove data error");
        }
    }

    public void nodeUpdate(int id, Transistor trn)
    {
        string transistorId = id.ToString("D3");
        XmlNode node = _doc.SelectSingleNode($"/root/transistor[@id='{transistorId}']");
        if(node != null)
        {
            node.Attributes["id"].Value = trn._id.ToString("D3");
            XmlNode nameNode = node.SelectSingleNode("name");
            if(nameNode != null) nameNode.InnerText = trn._name;
            XmlNode typesNode = node.SelectSingleNode("types");
            typesNode.RemoveAll();
            foreach(TransistorType type in trn._types)
            {
                XmlElement xtype = _doc.CreateElement("type");
                xtype.InnerText = type.ToString();
                typesNode.AppendChild(xtype);
            }
            XmlNode voltageNode = node.SelectSingleNode("voltage");
            if(voltageNode != null) voltageNode.InnerText = trn._voltage.ToString();
            XmlNode amperageNode = node.SelectSingleNode("amperage");
            if(amperageNode != null) amperageNode.InnerText = trn._amperage.ToString(); 
            XmlNode priceNode = node.SelectSingleNode("price");
            if(priceNode != null) priceNode.InnerText = trn._price.ToString();
            XmlNode countryNode = node.SelectSingleNode("country");
            if(countryNode != null) countryNode.InnerText = trn._country;

            saveDoc();
            Console.WriteLine("element updated");
        }
        else
        {
            Console.WriteLine("Update error. transistor not found");
        }
    }

    public XmlElement CreateMeasurementElement(XmlDocument doc, string tagName, float value, string currency)
    {
        XmlElement element = doc.CreateElement(tagName);
        element.SetAttribute("currency", currency);
        element.InnerText = value.ToString();
        return element;
    }
}