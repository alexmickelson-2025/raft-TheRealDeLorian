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
        public NodeStatus Status { get; set; }
        public StateMachine StateMachine { get; set; } = new();
        public INode[] OtherNodes { get; set; }
        public static int NodeIntervalScalar { get; set; } = 1;


        //Election
        public int CurrentTerm { get; set; } = 1;
        public int? VotedFor { get; set; }
        public int TimeLeft { get; set; }
        System.Timers.Timer ElectionTimer;
        public Stopwatch ElectionStopwatch;
        public double LastInterval;
        Random r = new();

        //Logging
        public List<RequestAppendEntriesData> Log { get; set; } = new();
        public int CommitIndex { get; set; }
        public int HeartbeatsReceived { get; set; }
        public int NextIndex { get; set; } = 1;
        public List<(int nodeId, int nextIndex)> NextIndicesToSend { get; set; }
        public bool IsPaused { get; set; }
        System.Timers.Timer HeartbeatTimer;


        public Node(int nodeId)
        {
            Id = nodeId;
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
            ElectionStopwatch = new Stopwatch();
            ElectionStopwatch.Start();
        }

        private async void OnTimerRunout(Object source, ElapsedEventArgs e)
        {
            // Console.WriteLine("The election timer has run out");
            await Timeout();
            ResetTimer();
        }

        public void SetTimeLeft()
        {
            double elapsed = ElectionStopwatch.ElapsedMilliseconds;
            TimeLeft = (int)(LastInterval - elapsed);
        }

        private void ResetTimer()
        {
            LastInterval = r.Next(150, 301);
            ElectionTimer.Interval = LastInterval;
            ElectionTimer.Start();
            ElectionStopwatch.Restart();
        }


        public async Task Timeout()
        {
            IncrementTerm();
            Console.WriteLine("the term is now " + CurrentTerm);
            Status = NodeStatus.Candidate;
            VotedFor = Id;
            await ConductElection();
        }

        private void IncrementTerm()
        {
            CurrentTerm++;
            // Console.WriteLine("Term for node " + Id + " is now " + CurrentTerm);
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
            Console.WriteLine("heartbeat...");
            RequestAppendEntriesData requestAppendEntriesData = new RequestAppendEntriesData() { SentFrom = Id, Term = CurrentTerm, LeaderCommitIndex = CommitIndex };
            await RequestAppendEntries(requestAppendEntriesData);
        }

        private async Task ConductElection()
        {
            if (OtherNodes.Length == 0)
            {
                await WinElection();
            }
            
            List<INode> supporters = new();
            foreach (var otherNode in OtherNodes)
            {
                await otherNode.RequestVote(new RequestVoteData() { CandidateId = Id, Term = CurrentTerm }); //requestvote is sent, which then returns 
                //if (isSupporter)
                //{
                //    supporters.Add(otherNode);
                //}
            }
            if (supporters.Count > OtherNodes.Length * 0.5)
            {
                await WinElection();
            }
        }

        public async Task WinElection()
        {
            Status = NodeStatus.Leader;
            foreach (INode otherNode in OtherNodes)
            {
                await otherNode.RequestAppendEntries(new RequestAppendEntriesData { SentFrom = Id, Term = CurrentTerm });
                // otherNode.NextIndex = Log.Count + 1;
            }
        }

        public async Task RequestAppendEntries(RequestAppendEntriesData data)
        {
            //if (IsPaused)
            //{
            //    return;
            //}
            //if (data.Term < CurrentTerm)
            //{
            //    return;
            //}

            //if (data.Term > CurrentTerm)
            //{
            //    CurrentTerm = data.Term;
            //    State = NodeState.Follower;
            //}

            //if (data.Entry == null || data.Entry == "")
            //{
            //    LeaderId = data.SentFrom;
            //    HeartbeatsReceived++;
            //}

            //CommitIndex = data.LeaderCommitIndex;
            Console.WriteLine("Received append entries from " + data.SentFrom);
            Log.Add(data);
            Console.WriteLine("Log length is " + Log.Count);

            //if (OtherNodes.ContainsKey(data.SentFrom) && OtherNodes.Count > 0)
            //{
            //    await OtherNodes[data.SentFrom].RespondAppendEntries(new ResponseEntriesData());
            //}
        }

        public async Task RespondAppendEntries(ResponseEntriesData data)
        {

        }

        public async Task RequestVote(RequestVoteData data)
        {
            if (data.Term < CurrentTerm)
            {
                //return false; //send a response that says no
            }
            else if (data.Term > CurrentTerm)
            {
                CurrentTerm = data.Term;
                VotedFor = null;
            }
            if (VotedFor is not null)
            {
                //return false; respond no
            }
            VotedFor = data.CandidateId;
            //return true; //respond yes
        }

        public async Task RequestFromClient(string command)
        {
            var requestAppendEntriesData = new RequestAppendEntriesData() { Entry = command, SentFrom = Id, Term = CurrentTerm };
            Log.Add(requestAppendEntriesData);

            foreach (INode follower in OtherNodes)
            {
                await follower.RequestAppendEntries(requestAppendEntriesData);
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
            if (Status != NodeStatus.Leader)
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

       



        public Task RespondVote(RespondVoteData data)
        {
            throw new NotImplementedException();
        }
    }
}
