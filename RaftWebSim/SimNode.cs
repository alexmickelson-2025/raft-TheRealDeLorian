using RaftLibrary;

namespace RaftWebSim
{
    public class SimNode : INode // question: why did alex choose to implement interface through innernode rather than implementing it normally?
    {

        public Node InnerNode { get; }
        public SimNode(Node node)
        {
            InnerNode = node;
        }

        public bool SimulationRunning { get; private set; } = false;
        public int CurrentTerm { get => ((INode)InnerNode).CurrentTerm; set => ((INode)InnerNode).CurrentTerm = value; }
        public int HeartbeatsReceived { get => ((INode)InnerNode).HeartbeatsReceived; set => ((INode)InnerNode).HeartbeatsReceived = value; }
        public int Id { get => ((INode)InnerNode).Id; set => ((INode)InnerNode).Id = value; }
        public int LeaderId { get => ((INode)InnerNode).LeaderId; set => ((INode)InnerNode).LeaderId = value; }
        public INode[] OtherNodes { get => ((INode)InnerNode).OtherNodes; set => ((INode)InnerNode).OtherNodes = value; }
        public NodeState State { get => ((INode)InnerNode).State; set => ((INode)InnerNode).State = value; }
        public int TimeLeft { get => ((INode)InnerNode).TimeLeft; set => ((INode)InnerNode).TimeLeft = value; }
        public int? VotedFor { get => ((INode)InnerNode).VotedFor; set => ((INode)InnerNode).VotedFor = value; }
        public static int NetworkRequestDelay { get; set; } = 1000;
        public List<RPCData> Log { get => ((INode)InnerNode).Log; set => ((INode)InnerNode).Log = value; }

        public Task<bool> AppendEntries(RPCData data)
        {
            return ((INode)InnerNode).AppendEntries(data);
        }

        public Task<bool> RequestVote(int term, int candidateId)
        {
            return ((INode)InnerNode).RequestVote(term, candidateId);
        }

        public Task Start()
        {
            return ((INode)InnerNode).Start();
        }

        public Task Timeout()
        {
            return ((INode)InnerNode).Timeout();
        }

        public async Task SendCommand(ClientCommandData data)
        {
            if (!SimulationRunning)
                return;
            await InnerNode.SendCommand(data);
        }


    }
}
