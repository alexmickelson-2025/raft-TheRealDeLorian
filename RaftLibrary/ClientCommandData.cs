namespace RaftLibrary;
public class ClientCommandData
{
    public ClientCommandType Type { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public ClientCommandData(ClientCommandType type, string key, string value)
    {
        Type = type;
        Key = key;
        Value = value;
    }
}