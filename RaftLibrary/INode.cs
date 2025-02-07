
namespace RaftLibrary
{
    public interface INode
    {
        int Id { get; }

        Task RequestAppendEntries(RequestAppendEntriesData data);
        Task RespondAppendEntries(ResponseEntriesData data);
        Task RequestVote(RequestVoteData data);
        Task RespondVote(RespondVoteData data);
        Task Start();
        Task Timeout();
        void AddOtherNodes(List<INode> nodes);
    }
}