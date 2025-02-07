namespace RaftLibrary
{
  public class NodeData
  {
    public int Id { get; set; }
    public NodeState State { get; set; }
    public int ElectionTimeout { get; set; }
    public int Term { get; set; }
    public int CurrentTermLeader { get; set; }
    public int CommittedEntryIndex { get; set; }
    public List<RequestAppendEntriesData> Log { get; set; }
    public string StateMachineState { get; set; }

    public NodeData(int id, int electionTimeout, string stateMachineState, int term, int currentTermLeader, int committedEntryIndex, List<RequestAppendEntriesData> log, NodeState state, int nodeIntervalScalar)
    {
      Id = id;
      State = state;
      ElectionTimeout = electionTimeout;
      Term = term;
      CurrentTermLeader = currentTermLeader;
      CommittedEntryIndex = committedEntryIndex;
      Log = log;
      StateMachineState = stateMachineState;
    }
  }
}