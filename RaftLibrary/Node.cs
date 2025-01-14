namespace RaftLibrary
{
    public class Node
    {
        int _term;

        public Node(int term)
        {
            _term = term;
        }

        public void TimeoutElection()
        {
            _term++;
        }

        public int GetTerm()
        {
            return _term;
        }

        public NodeState GetState()
        {
            return NodeState.Follower;
        }

    }
}
