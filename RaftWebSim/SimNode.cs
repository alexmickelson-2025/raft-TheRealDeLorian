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
        public int Id { get => ((INode)InnerNode).Id; set => ((INode)InnerNode).Id = value; }
        public static int NetworkRequestDelay { get; set; } = 1000;
        public int CurrentTerm { get => ((INode)InnerNode).CurrentTerm; set => ((INode)InnerNode).CurrentTerm = value; }
        public int HeartbeatsReceived { get => ((INode)InnerNode).HeartbeatsReceived; set => ((INode)InnerNode).HeartbeatsReceived = value; }
        public int LeaderId { get => ((INode)InnerNode).LeaderId; set => ((INode)InnerNode).LeaderId = value; }
        public StateMachine StateMachine { get => ((INode)InnerNode).StateMachine; set => ((INode)InnerNode).StateMachine = value; }
        public NodeStatus State { get => ((INode)InnerNode).State; set => ((INode)InnerNode).State = value; }
        public int TimeLeft { get => ((INode)InnerNode).TimeLeft; set => ((INode)InnerNode).TimeLeft = value; }
        public int NextIndex { get => ((INode)InnerNode).NextIndex; set => ((INode)InnerNode).NextIndex = value; }
        public int CommitIndex { get => ((INode)InnerNode).CommitIndex; set => ((INode)InnerNode).CommitIndex = value; }
        public List<(int nodeId, int nextIndex)> NextIndicesToSend { get => ((INode)InnerNode).NextIndicesToSend; set => ((INode)InnerNode).NextIndicesToSend = value; }
        public int? VotedFor { get => ((INode)InnerNode).VotedFor; set => ((INode)InnerNode).VotedFor = value; }
        public List<RequestAppendEntriesData> Log { get => ((INode)InnerNode).Log; set => ((INode)InnerNode).Log = value; }
        public Dictionary<int, INode> OtherNodes { get => ((INode)InnerNode).OtherNodes; set => ((INode)InnerNode).OtherNodes = value; }

        public Task RequestVote(int term, int candidateId)
        {
            return ((INode)InnerNode).RequestVote(new RequestVoteData() { Term = term, CandidateId = candidateId });
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

        public Task RequestAppendEntries(RequestAppendEntriesData data)
        {
            return ((INode)InnerNode).RequestAppendEntries(data);
        }

        public Task RespondAppendEntries(ResponseEntriesData data)
        {
            return ((INode)InnerNode).RespondAppendEntries(data);
        }

        public Task RequestVote(RequestVoteData data)
        {
            return ((INode)InnerNode).RequestVote(data);
        }

        public Task RespondVote(RespondVoteData data)
        {
            return ((INode)InnerNode).RespondVote(data);
        }

        public void AddOtherNodes(List<INode> nodes)
        {
            ((INode)InnerNode).AddOtherNodes(nodes);
        }
    }
}
