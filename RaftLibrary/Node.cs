using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

namespace RaftLibrary
{
    public class Node
    {
        int _term;
        public string votedFor;
        public string currentLeader;
        public NodeState state;
        System.Timers.Timer t;

        public Node()
        {
            _term = 1;
            StartElectionTimer();
        }

        //This method was largely inspired by official Microsoft C# documentation:
        // https://learn.microsoft.com/en-us/dotnet/api/system.timers.timer?view=net-9.0
        public async Task StartElectionTimer()
        {
            t = new System.Timers.Timer(300);
            t.Elapsed += OnTimerRunout;
            t.AutoReset = true;
            t.Enabled = true;
        }

        private async void OnTimerRunout(Object source, ElapsedEventArgs e)
        {
            await TimeoutElection();
        }


        public async Task TimeoutElection()
        {
            _term++;
            state = NodeState.Candidate;
        }

        public async Task<int> GetTerm()
        {
            return _term;
        }
        public async Task SetTerm(int term)
        {
            _term = term;
        }

        public async Task<string> AppendEntries(RPCData data)
        {
            if(data.Entry == null)
            {
                currentLeader = data.SentFrom;
            }
            return "Successfully appended entries";
        }

        public async Task<NodeState> GetState()
        {
            return state;
        }

        public async Task<bool> RequestVote(int term, string candidateName)
        {
            if (term < _term)
            {
                return false;
            }

            votedFor = candidateName;
            return true;
        }
    }
}
