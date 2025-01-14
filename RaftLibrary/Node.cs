using System.Diagnostics;

namespace RaftLibrary
{
    public class Node
    {
        int _term;
        public string votedFor;

        public Node(int term)
        {
            _term = term;
        }

        public async Task TimeoutElection()
        {
            _term++;
        }

        public async Task<int> GetTerm()
        {
            return _term;
        }

        public async Task<string> AppendEntries(string entry)
        {
            return "";
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
