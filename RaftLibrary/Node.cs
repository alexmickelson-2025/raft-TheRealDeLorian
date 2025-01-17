﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

namespace RaftLibrary
{
    public class Node 
    {
        public int Id { get; set; }
        public int LeaderId { get; set; }
        public NodeState State { get; set; }
        public int CurrentTerm {get; set;}
        public int? VotedFor {get; set;}
        public int TimeLeft {get; set;}
        public Node[] OtherNodes {get; set;}
        public int HeartbeatsReceived { get; set; }
        System.Timers.Timer t;
        Random r = new();

        public Node(Node[] otherNodes, int nodeId)
        {
            Id = nodeId;
            OtherNodes = otherNodes;
            CurrentTerm = 1;
            StartElectionTimer();
        }

        //This method was largely inspired by official Microsoft C# documentation:
        // https://learn.microsoft.com/en-us/dotnet/api/system.timers.timer?view=net-9.0
        public async Task StartElectionTimer()
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
            await TimeoutElection();
            ResetTimer();
        }


        public async Task TimeoutElection()
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
                State = NodeState.Leader;
            }
            List<Node> supporters = new();
            foreach (Node otherNode in OtherNodes)
            {
                bool isSupporter = await otherNode.RequestVote(CurrentTerm, Id);
                if (isSupporter)
                {
                    supporters.Add(otherNode);
                }
            }
            if (supporters.Count > OtherNodes.Length * 0.5)
            {
                State = NodeState.Leader;
                foreach (Node otherNode in OtherNodes)
                {
                    await otherNode.AppendEntries(new RPCData { SentFrom = Id, Term=CurrentTerm });
                }
            }
        }

        public async Task<bool> AppendEntries(RPCData data)
        {
            if(data.Term > CurrentTerm)
            {
                CurrentTerm = data.Term;
                State = NodeState.Follower;
            }

            if (data.Entry == null || data.Entry == "")
            {
                LeaderId = data.SentFrom;
                HeartbeatsReceived++;
            }
            if(data.Term < CurrentTerm)
            {
                return false;
            }
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
            if(VotedFor is not null)
            {
                return false;
            }
            VotedFor = candidateId;
            return true;
        }
    }
}
