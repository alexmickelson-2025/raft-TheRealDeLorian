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

        public Task RequestAppendEntries(RPCData data)
        {
            return ((INode)InnerNode).RequestAppendEntries(data);
        }

        public Task RespondAppendEntries(ResponseEntriesData data)
        {
            return ((INode)InnerNode).RespondAppendEntries(data);
        }
    }
}
