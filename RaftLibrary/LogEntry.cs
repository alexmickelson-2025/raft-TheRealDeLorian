namespace RaftLibrary;

public class LogEntry
{
  public int Index {get; set;}
  public int Term { get; set; }
  public ClientCommandData Command { get; set; }
}