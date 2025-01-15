
namespace RaftLibrary
{
    public interface INode
    {
        int LeaderId { get; set; }
        Task<string> AppendEntries(RPCData data);
        Task<NodeState> GetState();
        Task<int> GetTerm();
        Task<bool> RequestVote(int term, string candidateName);
        Task SetTerm(int term);
        Task StartElectionTimer();
        Task TimeoutElection();
    }
}