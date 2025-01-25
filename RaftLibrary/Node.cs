using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

namespace RaftLibrary
{
    public class Node : INode
    {
        public int Id { get; set; }
        public int LeaderId { get; set; }
        public NodeState State { get; set; }
        public int CurrentTerm { get; set; } = 1;
        public int? VotedFor { get; set; }
        public int TimeLeft { get; set; }
        public INode[] OtherNodes { get; set; }
        public int HeartbeatsReceived { get; set; }
        public static int NodeIntervalScalar {get; set;}
        public List<RPCData> Log { get; set; } = new();
        System.Timers.Timer t;
        Random r = new();

        public Node(Node[] otherNodes, int nodeId)
        {
            Id = nodeId;
            OtherNodes = otherNodes;
            // StartElectionTimer(); //Make this like timer.Start() where you have to start up the node first every time you make it. or come up with some other way to make the timer not start int the ctor
        }

        //This method was largely inspired by official Microsoft C# documentation:
        // https://learn.microsoft.com/en-us/dotnet/api/system.timers.timer?view=net-9.0
        public async Task Start()
        {
            t = new System.Timers.Timer();
            t.Elapsed += OnTimerRunout;
            t.AutoReset = true;
            t.Enabled = true;
            ResetTimer();
        }

        private void ResetTimer()
        {
            TimeLeft = r.Next(150, 301);
            t.Interval = TimeLeft;
            t.Start();
        }

        private async void OnTimerRunout(Object source, ElapsedEventArgs e)
        {
            await Timeout();
            ResetTimer();
        }


        public async Task Timeout()
        {
            CurrentTerm++;
            State = NodeState.Candidate;
            VotedFor = Id;
            await ConductElection();
        }

        private async Task ConductElection()
        {
            if (OtherNodes.Length == 0)
            {
                await WinElection();
            }
            List<INode> supporters = new();
            foreach (INode otherNode in OtherNodes)
            {
                bool isSupporter = await otherNode.RequestVote(CurrentTerm, Id);
                if (isSupporter)
                {
                    supporters.Add(otherNode);
                }
            }
            if (supporters.Count > OtherNodes.Length * 0.5)
            {
                await WinElection();
            }
        }

        public async Task WinElection()
        {
            State = NodeState.Leader; 
            foreach (INode otherNode in OtherNodes)
            {
                await otherNode.AppendEntries(new RPCData { SentFrom = Id, Term = CurrentTerm });
            }
        }

        public async Task<bool> AppendEntries(RPCData data)
        {
            if (data.Term < CurrentTerm)
            {
                return false;
            }
            
            if (data.Term > CurrentTerm)
            {
                CurrentTerm = data.Term;
                State = NodeState.Follower;
            }

            if (data.Entry == null || data.Entry == "")
            {
                LeaderId = data.SentFrom;
                HeartbeatsReceived++;
            }

            Log.Add(data);

            return true;
        }

        public async Task<bool> RequestVote(int term, int candidateId)
        {
            if (term < CurrentTerm)
            {
                return false;
            }
            else if (term > CurrentTerm)
            {
                CurrentTerm = term;
                VotedFor = null;
            }
            if (VotedFor is not null)
            {
                return false;
            }
            VotedFor = candidateId;
            return true;
        }

        public async Task RequestFromClient(string command)
        {
            foreach(INode follower in OtherNodes)
            {
                var rpcData = new RPCData() { Entry = command, SentFrom = Id, Term = CurrentTerm };
                await follower.AppendEntries(rpcData);
            }
        }

        public async Task SendCommand(ClientCommandData data)
        {
            throw new NotImplementedException();
        }
    }
}
