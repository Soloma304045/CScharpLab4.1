namespace Library;

public class Transistor
{
    public int _id { get; set; }
    public string _name { get; set; }
    public List<TransistorType> _types { get; set; }
    public float _voltage { get; set; }
    public float _amperage { get; set; }
    public float _price { get; set; }
    public string _country { get; set; }
    public Transistor(int id, string name, List<TransistorType> types, float voltage, float amperage, float price, string country)
    {
        _id = id;
        _name = name;
        _types = types;
        _voltage = voltage;
        _amperage = amperage;
        _price = price;
        _country = country;
    }

    public void Info()
    {
        string transistorId = _id.ToString("D3"); 
        Console.WriteLine($"id - {transistorId}");
        Console.WriteLine($"name - {_name}");
        Console.Write("Types -");
        foreach(TransistorType type in _types)
        {
            Console.Write($" {type}");
        }
        Console.WriteLine($"\nvoltage - {_voltage}");
        Console.WriteLine($"amperage - {_amperage}");
        Console.WriteLine($"price - {_price}");
        Console.WriteLine($"country - {_country}");
    }

    public string TypesString => string.Join(", ", _types.Select(t => t.ToString()));
}