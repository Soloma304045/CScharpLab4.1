namespace Library;

public static class TransistorExtensions
{
    public static string TypesString(this Transistor transistor)
    {
        return string.Join(", ", transistor._types.Select(t => t.ToString()));
    }
}