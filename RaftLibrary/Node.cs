﻿using System.Diagnostics;
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
        public static int NodeIntervalScalar { get; set; }


        //Election
        public int CurrentTerm { get; set; } = 1;
        public int? VotedFor { get; set; }
        public int TimeLeft { get; set; }
        System.Timers.Timer ElectionTimer;
        public Stopwatch ElectionStopwatch;
        public double LastInterval;
        Random r = new();
        List<INode> supporters {get; set;}

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
            ResetElectionTimer();
        }

        public void SetTimeLeft()
        {
            double elapsed = ElectionStopwatch.ElapsedMilliseconds;
            TimeLeft = (int)(LastInterval - elapsed);
        }

        private void ResetElectionTimer()
        {
            LastInterval = r.Next(NodeIntervalScalar * 150, NodeIntervalScalar * 301);
            ElectionTimer.Interval = LastInterval;
            ElectionTimer.Start();
            ElectionStopwatch.Restart();
        }


        public async Task Timeout()
        {
            Status = NodeStatus.Candidate;
            IncrementTerm();
            VotedFor = Id;
            ResetElectionTimer();
            foreach (var otherNode in OtherNodes)
            {
                await otherNode.RequestVote(new RequestVoteData() { CandidateId = Id, Term = CurrentTerm }); //requestvote is sent, which then returns 
                Console.WriteLine("Vote request sent to node number " + otherNode.Id);
            }
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

        public async Task WinElection()
        {
            Status = NodeStatus.Leader;
            LeaderId = Id;
            foreach (INode otherNode in OtherNodes)
            {
                await otherNode.RequestAppendEntries(new RequestAppendEntriesData { SentFrom = Id, Term = CurrentTerm });
                // otherNode.NextIndex = Log.Count + 1;
            }
        }

        public async Task RequestAppendEntries(RequestAppendEntriesData data)
        {
           

            if (data.Term > CurrentTerm)
            {
               CurrentTerm = data.Term;
               Status = NodeStatus.Follower;
            }

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
            Console.WriteLine("Vote requested from node " + data.CandidateId);
            if (data.Term < CurrentTerm)
            {
                await OtherNodes.FirstOrDefault(node => node.Id == data.CandidateId)?.RespondVote(new RespondVoteData() { SentFromNodeId = Id, Success = false, Term = CurrentTerm });
                Console.WriteLine($"Node {Id} did not vote for node {data.CandidateId} because the node {Id} had a larger term");
            }
            else if (data.Term > CurrentTerm)
            {
                CurrentTerm = data.Term;
                VotedFor = null;
                Console.WriteLine($"Node {Id} has set its VotedFor to null because node {data.CandidateId} has requested a vote from a larger term");
            }
            if (VotedFor is not null)
            {
                await OtherNodes.FirstOrDefault(node => node.Id == data.CandidateId)?.RespondVote(new RespondVoteData() { SentFromNodeId = Id, Success = false, Term = CurrentTerm });
                Console.WriteLine($"Node {Id} did not vote for node {data.CandidateId} because node {Id} has already voted");
            }
            VotedFor = data.CandidateId;
                            await OtherNodes.FirstOrDefault(node => node.Id == data.CandidateId)?.RespondVote(new RespondVoteData() { SentFromNodeId = Id, Success = true, Term = CurrentTerm });

        }

        public Task RespondVote(RespondVoteData data)
        {
            if(data.Success)
            {
                supporters.Add(OtherNodes.Where(e => e.Id == data.SentFromNodeId-1).FirstOrDefault());
                Console.WriteLine($"Node {Id} has {supporters.Count} supporters");
            }
            if (supporters.Count > (OtherNodes.Length + 1) / 2)
            {
              WinElection();
            }
            return Task.CompletedTask;
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

       



      
    }
}
