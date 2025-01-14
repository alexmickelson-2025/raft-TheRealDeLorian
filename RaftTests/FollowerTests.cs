using FluentAssertions;
using RaftLibrary;

namespace RaftTests
{
    public class FollowerTests
    {
        [Fact]
        public async void TestFollowerTermIncrementOnElectionTimeout()
        {
            Node node = new Node(1);
            node.TimeoutElection();
            int term = await node.GetTerm();
            term.Should().Be(2);
        }

        //Testing #3
        [Fact]
        public async void NewNodeShouldStartAsFollower()
        {
            Node node = new Node(1);
            NodeState state = await node.GetState();
            state.Should().Be(NodeState.Follower);
        }

        //Testing #7
        [Fact]
        public async void WhenEmptyAppendIsReceivedNoElectionIsStarted()
        {
            Node node = new Node(1);
            node.AppendEntries("");
            NodeState state = await node.GetState();
            state.Should().Be(NodeState.Follower);
        }
        //Testing #7
        //[Fact]
        //public async void WhenNoEmptyAppendIsReceivedAnElectionIsStarted()
        //{
        //    Node node = new Node(1);
        //    Task.Delay(300);
        //    NodeState state = await node.GetState();
        //    state.Should().Be(NodeState.Candidate);
        //}



        //Testing #10
        [Fact]
        public async void FollowerWhoHasNotVotedAndInEarlierTermThanCandidateVotesYes()
        {
            Node node = new Node(1);
            node.votedFor.Should().Be(null);
            bool votedForTestCandidate = await node.RequestVote(3, "test");
            votedForTestCandidate.Should().Be(true);
            node.votedFor.Should().Be("test");
        }
        //Testing #10
        [Fact]
        public async void FollowerWhoHasNotVotedAndInLaterTermThanCandidateVotesNo()
        {
            Node node = new Node(3);
            bool voteResult = await node.RequestVote(1, "test");
            voteResult.Should().Be(false);
        }

        [Fact]
        public async void FollowerWhoReceivesAppendRequestSendsResponse()
        {
            Node node = new Node(1);
            string response = await node.AppendEntries("");
            response.Length.Should().BeGreaterThan(0);
        }

    }
}
