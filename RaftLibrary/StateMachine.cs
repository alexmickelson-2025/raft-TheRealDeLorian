using System.Security.Cryptography.X509Certificates;

public class StateMachine
{
    public Dictionary<string, string> Store { get; set; }


    public void Apply(string key, string value)
    {
        Console.WriteLine($"Applying command to StateMachine: {key} = {value}");
        Store[key] = value;
    }
}