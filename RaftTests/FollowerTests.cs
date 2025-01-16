using FluentAssertions;
using Meziantou.Xunit;
using NSubstitute;
using RaftLibrary;

namespace RaftTests
{
    [DisableParallelization]
    public class FollowerTests
    {
        [Fact]
        public async void TestFollowerTermIncrementOnElectionTimeout()
        {
            Node node = new Node();
            await node.TimeoutElection();
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
            await node.AppendEntries(new RPCData() { SentFrom = "Leader" });
            NodeState state = await node.GetState();
            state.Should().Be(NodeState.Follower);
        }

        //Testing #4
        [Fact]
        public async void WhenNoEmptyAppendIsReceivedAnElectionIsStarted()
        {
            Node node = new Node();
            Thread.Sleep(350);
            NodeState state = await node.GetState();
            state.Should().Be(NodeState.Candidate);
        }

        //Testing 5
        [Fact]
        public async void WhenElectionTimeResetsItIsBetween150msAnd300ms()
        {
            Node node = new();
            Thread.Sleep(350);
            node.TimeLeft.Should().BeLessThan(300);
            node.TimeLeft.Should().BeGreaterThan(150);
        }
        //Testing 5
        [Fact]
        public async void WhenElectionTimeResetsItIsRandom()
        {
            //call the test like 100 times and make sure that each time is different
            List<Node> nodes = new();
            List<int> times = new();

            for (int i = 0; i < 100; i++)
            {
                Node node = new();
                nodes.Add(node);
                times.Add(node.TimeLeft);
            }

            var uniqueTimes = times.Distinct().ToList(); //ChatGPT helped me discover the Distinct method, something I didn't know existed
            uniqueTimes.Count.Should().BeGreaterThan(30);
        }

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
            node.currentLeader.Should().Be("Leader");
            await node.AppendEntries(new RPCData() { SentFrom = "Impostor", Entry="Hello! I'm not sus" });
            node.currentLeader.Should().Be("Leader");
        }






    }
}
