namespace RaftLibrary
{
  public record NodeData
  {
    public int Id { get; set; }
    public NodeStatus Status { get; set; }
    public int ElectionTimeout { get; set; }
    public int Term { get; set; }
    public int CurrentTermLeader { get; set; }
    public int CommittedEntryIndex { get; set; }
    public List<RequestAppendEntriesData> Log { get; set; }
    public string StateMachineState { get; set; }
  }
}