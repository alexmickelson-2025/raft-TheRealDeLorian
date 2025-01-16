using FluentAssertions;
using Meziantou.Xunit;
using NSubstitute;
using RaftLibrary;

namespace RaftTests
{
    public class FollowerTests
    {
        [Fact]
        public async Task TestFollowerTermIncrementOnElectionTimeout()
        {
            Node node = new Node([], 1);
            await node.TimeoutElection();
            int term = node.CurrentTerm;
            term.Should().Be(2);
        }

        [Fact]
        public async Task FollowerInitializesWithOtherNodesArrayAndOwnId()
        {
            Node node1 = new Node([], 1);
            Node node2 = new Node([], 2);
            Node node3 = new Node([], 3);

            node1.OtherNodes = [node2, node3];
            node2.OtherNodes = [node1, node3];
            node3.OtherNodes = [node1, node2];

            node1.Id.Should().Be(1);
            node2.Id.Should().Be(2);
            node3.Id.Should().Be(3);
        }
        //Testing #3
        [Fact]
        public async Task NewNodeShouldStartAsFollower()
        {
            Node node = new Node([], 1);
            NodeState state = node.State;
            state.Should().Be(NodeState.Follower);
        }

        //Testing #7
        [Fact]
        public async Task WhenEmptyAppendIsReceivedNoElectionIsStarted()
        {
            Node node = new Node([], 1);
            await node.AppendEntries(new RPCData() { SentFrom = 2 });
            NodeState state = node.State;
            state.Should().Be(NodeState.Follower);
        }

        //Testing #4
        [Fact]
        public async Task WhenNoEmptyAppendIsReceivedAnElectionIsStarted()
        {
            Node node = new Node([], 1);
            Thread.Sleep(500);
            NodeState state = node.State;
            state.Should().Be(NodeState.Candidate);
        }

        //Testing 5
        [Fact]
        public async Task WhenElectionTimeResetsItIsBetween150msAnd300ms()
        {
            Node node = new([], 1);
            Thread.Sleep(350);
            node.TimeLeft.Should().BeLessThan(300);
            node.TimeLeft.Should().BeGreaterThan(150);
        }
        //Testing 5
        [Fact]
        public async Task WhenElectionTimeResetsItIsRandom()
        {
            //call the test like 100 times and make sure that each time is different
            List<Node> nodes = new();
            List<int> times = new();

            for (int i = 0; i < 100; i++)
            {
                Node node = new([], 1);
                nodes.Add(node);
                times.Add(node.TimeLeft);
            }

            var uniqueTimes = times.Distinct().ToList(); //ChatGPT helped me discover the Distinct method, something I didn't know existed
            uniqueTimes.Count.Should().BeGreaterThan(30);
        }

        //Testing #10
        [Fact]
        public async Task FollowerWhoHasNotVotedAndInEarlierTermThanCandidateVotesYes()
        {
            Node node = new Node([], 1);
            node.VotedFor.Should().Be(null);
            bool votedForTestCandidate = await node.RequestVote(3, 2);
            votedForTestCandidate.Should().Be(true);
            node.VotedFor.Should().Be(2);
        }
        //Testing #10
        [Fact]
        public async Task FollowerWhoHasNotVotedAndInLaterTermThanCandidateVotesNo()
        {
            var node = new Node([], 1);
            node.CurrentTerm = 3;
            bool voteResult = await node.RequestVote(1, 2);
            voteResult.Should().Be(false);
        }

        //Testing #TODO CHANGE THIS
        [Fact]
        public async Task FollowerWhoHasVotedThisTermVotesNoToAnyoneElse()
        {
            var node = new Node([], 1);
            node.VotedFor = 2;
            node.CurrentTerm = 2;
            bool IsVotingForNode = await node.RequestVote(2, 3);
            IsVotingForNode.Should().Be(false);
        }
        //Testing #14
        [Fact]
        public async Task SecondVoteRequestFromSameCandidateInSameTermVotesNo()
        {
            var node = new Node([], 1);
            node.CurrentTerm = 2;
            bool IsVotingForNode = await node.RequestVote(2, 2);
            IsVotingForNode.Should().Be(true);
            bool IsVotingForNode2 = await node.RequestVote(2, 2);
            IsVotingForNode2.Should().Be(false);
        }
        //Testing #15
          [Fact]
        public async Task FollowerWhoReceivesSecondVoteRequestFromSameCandidateForFutureTermShouldVoteYes()
        {
            var node = new Node([], 1);
            node.CurrentTerm = 2;
            bool IsVotingForNode = await node.RequestVote(2, 2);
            IsVotingForNode.Should().Be(true);
            bool IsVotingForNode2 = await node.RequestVote(3, 2);
            IsVotingForNode2.Should().Be(true);
        }

        [Fact]
        public async Task FollowerWhoReceivesVoteRequestForFutureTermShouldVoteYes()
        {
            var node = new Node([], 1);
            node.VotedFor = 2;
            node.CurrentTerm = 2;
            bool IsVotingForNode = await node.RequestVote(3, 2);
            IsVotingForNode.Should().Be(true);
        }
        //Testing #17
        [Fact]
        public async Task FollowerWhoReceivesAppendRequestSendsResponse()
        {
            Node node = new Node([], 1);
            bool response = await node.AppendEntries(new RPCData() { Term = 1, SentFrom = 2 });
            response.Should().BeTrue();
        }

        //Testing #2
        [Fact]
        public async Task ReceivingAppendEntriesMakesNodeRememberWhoIsTheLeader()
        {
            Node node = new Node([], 1);
            await node.AppendEntries(new RPCData() { SentFrom = 2});
            node.LeaderId.Should().Be(2);
            await node.AppendEntries(new RPCData() { SentFrom = 3, Entry="Hello! I'm not sus" });
            node.LeaderId.Should().Be(2);
        }






    }
}
