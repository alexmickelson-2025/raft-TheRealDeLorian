using FluentAssertions;
using NSubstitute;
using RaftLibrary;

namespace RaftTests
{
    public class FollowerTests
    {
        [Fact]
        public async void TestFollowerTermIncrementOnElectionTimeout()
        {
            Node node = new Node();
            node.TimeoutElection();
            int term = await node.GetTerm();
            term.Should().Be(2);
        }

        //Testing #3
        [Fact]
        public async void NewNodeShouldStartAsFollower()
        {
            Node node = new Node();
            NodeState state = await node.GetState();
            state.Should().Be(NodeState.Follower);
        }

        //Testing #7
        [Fact]
        public async void WhenEmptyAppendIsReceivedNoElectionIsStarted()
        {
            Node node = new Node();
            node.AppendEntries(new RPCData() { SentFrom = "Leader" });
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
            Node node = new Node();
            node.votedFor.Should().Be(null);
            bool votedForTestCandidate = await node.RequestVote(3, "test");
            votedForTestCandidate.Should().Be(true);
            node.votedFor.Should().Be("test");
        }
        //Testing #10
        [Fact]
        public async void FollowerWhoHasNotVotedAndInLaterTermThanCandidateVotesNo()
        {
            var node = new Node();
            await node.SetTerm(3);
            bool voteResult = await node.RequestVote(1, "test");
            voteResult.Should().Be(false);
        }
        //Testing #17
        [Fact]
        public async void FollowerWhoReceivesAppendRequestSendsResponse()
        {
            Node node = new Node();
            string response = await node.AppendEntries(new RPCData() { SentFrom = "Leader" });
            response.Length.Should().BeGreaterThan(0);
        }

        //Testing #2
        [Fact]
        public async void ReceivingAppendEntriesMakesNodeRememberWhoIsTheLeader()
        {
            Node node = new Node();
            await node.AppendEntries(new RPCData() { SentFrom = "Leader"});
            node.votedFor.Should().Be("Leader");
        }

    }
}
