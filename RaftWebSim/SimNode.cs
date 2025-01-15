using RaftLibrary;

namespace RaftWebSim
{
    public class SimNode : INode
    {

        public readonly Node InnerNode;
        public SimNode(Node node)
        {
            InnerNode = node;
        }
        //public int CurrentLeader => InnerNode.CurrentLeader;
        public int Id { get => InnerNode.Id; set => InnerNode.Id = value; }
        public int LeaderId { get => ((INode)InnerNode).LeaderId; set => ((INode)InnerNode).LeaderId = value; }
        public NodeState State { get; set; }
        public int Term { get; set; }




        public Task<string> AppendEntries(RPCData data)
        {
            throw new NotImplementedException();
        }

        public Task<NodeState> GetState()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTerm()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RequestVote(int term, string candidateName)
        {
            throw new NotImplementedException();
        }

        public Task SetTerm(int term)
        {
            throw new NotImplementedException();
        }

        public Task StartElectionTimer()
        {
            throw new NotImplementedException();
        }

        public Task TimeoutElection()
        {
            throw new NotImplementedException();
        }
    }
}
