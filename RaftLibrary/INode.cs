
namespace RaftLibrary
{
    public interface INode
    {
        int CurrentTerm { get; set; }
        int HeartbeatsReceived { get; set; }
        int Id { get; set; }
        int LeaderId { get; set; }
        INode[] OtherNodes { get; set; }
        NodeState State { get; set; }
        int TimeLeft { get; set; }
        int? VotedFor { get; set; }
        List<RPCData> Log {get; set;}

        Task<bool> AppendEntries(RPCData data);
        Task<bool> RequestVote(int term, int candidateId);
        Task Start();
        Task Timeout();
    }
}