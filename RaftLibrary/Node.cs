using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

namespace RaftLibrary
{
    public class Node : INode
    {
        //Node info
        public int Id { get; set; }
        public int LeaderId { get; set; }
        public NodeState State { get; set; }
        public StateMachine StateMachine { get; set; } = new();
        public INode[] OtherNodes { get; set; }

        //Election
        public int CurrentTerm { get; set; } = 1;
        public int? VotedFor { get; set; }
        public int TimeLeft { get; set; }
        System.Timers.Timer ElectionTimer;
        Random r = new();

        //Logging
        public List<RPCData> Log { get; set; } = new();
        public int CommitIndex { get; set; }
        public int HeartbeatsReceived { get; set; }
        public static int NodeIntervalScalar { get; set; }
        public int NextIndex { get; set; } = 1;
        public List<(int nodeId, int nextIndex)> NextIndicesToSend { get; set; }
        public bool IsPaused { get; set; }
        System.Timers.Timer HeartbeatTimer;


        public Node(Node[] otherNodes, int nodeId)
        {
            Id = nodeId;
            OtherNodes = otherNodes;
            // Start(); //Make this like timer.Start() where you have to start up the node first every time you make it. or come up with some other way to make the timer not start int the ctor
        }

        //This method was largely inspired by official Microsoft C# documentation:
        // https://learn.microsoft.com/en-us/dotnet/api/system.timers.timer?view=net-9.0
        public async Task Start()
        {
            ElectionTimer = new System.Timers.Timer();
            ElectionTimer.Elapsed += OnTimerRunout;
            ElectionTimer.AutoReset = true;
            ElectionTimer.Enabled = true;
        }

        private void ResetTimer()
        {
            TimeLeft = r.Next(150, 301);
            ElectionTimer.Interval = TimeLeft;
            ElectionTimer.Start();
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

        public async Task StartHeartbeatTimer()
        {
            HeartbeatTimer = new System.Timers.Timer(50);
            HeartbeatTimer.Elapsed += OnHeartbeatTimerRunout;
            HeartbeatTimer.AutoReset = true;
            HeartbeatTimer.Enabled = true;
        }

        private async void OnHeartbeatTimerRunout(object? sender, ElapsedEventArgs e)
        {
            RPCData rpcData = new RPCData() { SentFrom = Id, Term = CurrentTerm, LeaderCommitIndex = CommitIndex };
            await AppendEntries(rpcData);
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
                otherNode.NextIndex = Log.Count + 1;
            }
        }

        public async Task<bool> AppendEntries(RPCData data)
        {
            if (IsPaused)
            {
                return false;
            }
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

            CommitIndex = data.LeaderCommitIndex;
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
            var rpcData = new RPCData() { Entry = command, SentFrom = Id, Term = CurrentTerm };
            Log.Add(rpcData);

            foreach (INode follower in OtherNodes)
            {
                await follower.AppendEntries(rpcData);
            }
        }

        public async Task SendCommand(ClientCommandData data)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public bool Set(string key, int value)
        {
            if (State != NodeState.Leader)
            {
                return false;
            }
            switch (key)
            {
                case "X":
                    StateMachine.AddToX(value);
                    break;
                case "Y":
                    StateMachine.AddToY(value);
                    break;
                case "Z":
                    StateMachine.AddToZ(value);
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
