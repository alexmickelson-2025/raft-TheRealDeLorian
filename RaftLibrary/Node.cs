using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RaftLibrary
{
    public class Node
    {
        int _term;
        public string votedFor;
        public string currentLeader;

        public Node()
        {
            _term = 1;
        }

        public async Task TimeoutElection()
        {
            _term++;
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
            return NodeState.Follower;
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
