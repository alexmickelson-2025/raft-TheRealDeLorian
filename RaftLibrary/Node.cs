using System.Diagnostics;
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
        }


        public async Task<string> AppendEntries(RPCData data)
        {
            if(data.Entry == null)
            {
                LeaderId = data.SentFrom;
            }
            return "Successfully appended entries";
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
