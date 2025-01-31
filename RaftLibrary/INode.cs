
namespace RaftLibrary
{
    public interface INode
    {
        int Id { get; set; }
        // int CurrentTerm { get; set; }
        // int HeartbeatsReceived { get; set; }
        // int LeaderId { get; set; }
        // INode[] OtherNodes { get; set; }
        // StateMachine StateMachine { get; set; }

        // NodeState State { get; set; }
        // int TimeLeft { get; set; }
        // int NextIndex { get; set; }
        // int CommitIndex { get; set; }
        // List<(int nodeId, int nextIndex)> NextIndicesToSend { get; set; }

        // int? VotedFor { get; set; }
        // List<RPCData> Log { get; set; }

        Task RequestAppendEntries(RPCData data);
        Task RespondAppendEntries(ResponseEntriesData data);
        Task<bool> RequestVote(int term, int candidateId);
        Task Start();
        Task Timeout();
    }
}