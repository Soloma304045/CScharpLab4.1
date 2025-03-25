using Library;
class Program()
{
    static void Main(string[] args)
    {
        DomToolkid toolkid = new DomToolkid(@"C:\Users\Иван\Desktop\OOP\4 Семестр\Lab-1\data.xml");
        Transistor trn = toolkid.nodeShow(4);
        trn._id = 31;
        //trn.Info();
        toolkid.nodeCreate(trn);
        trn._name = "FKLFKLKLFKLKF";
        trn._voltage = 15;
        trn._amperage = 16;
        trn._price = 90023;
        trn._country = "dsdksod";
        trn._types = [TransistorType.MOSFET, TransistorType.IGBT];
        //toolkid.nodeRemove(trn._id);
        toolkid.nodeUpdate(31, trn);
    }
}
