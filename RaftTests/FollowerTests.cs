using FluentAssertions;
using NSubstitute;
using RaftLibrary;

namespace RaftTests.Election;

public class FollowerTests
{
    [Fact]
    public async Task NewNodesInClusterAreFollowersOnStartup()
    {
       Node node = new Node(1);
       node.State.Should().Be(NodeState.Follower);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(35, 36)]
    [InlineData(9000, 9001)]
    public async Task WhenElectionTimerRunsOutTermShouldIncrement(int beforeTerm, int afterTerm)
    {
        Node node = new Node(1);
        node.CurrentTerm = beforeTerm;
        await node.Timeout();
        int term = node.CurrentTerm;
        term.Should().Be(afterTerm);
    }

    [Fact]
    public async Task WhenElectionTimerRunsOutInAClusterNodeShouldBeCandidate()
    {
        Node node = new Node(1);
        INode node2 = Substitute.For<INode>();
        node2.Id = 2;
        INode node3 = Substitute.For<INode>();
        node3.Id = 3;
        node.AddOtherNodes(new List<INode> { node2, node3 });
        node2.AddOtherNodes(new List<INode> { node, node3 });
        node3.AddOtherNodes(new List<INode> { node, node2 });

        await node.Timeout();
        NodeState state = node.State;
        state.Should().Be(NodeState.Candidate);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(34)]
    [InlineData(69)]
    public async Task WhenElectionTimerRunsOutVotedForShouldBeOwn(int id)
    {
        Node node = new Node(id);
        await node.Timeout();
        int? votedFor = node.VotedFor;
        votedFor.Should().Be(id);
    }

    [Fact]
    public async Task WhenElectionTimerRunsOutNodeShouldSendVoteRequestsToOtherNodes()
    {
        Node node = new Node(1);
        INode node2 = Substitute.For<INode>();
        node2.Id = 2;
        INode node3 = Substitute.For<INode>();
        node3.Id = 3;
        node.AddOtherNodes(new List<INode> { node2, node3 });

        await node.Timeout();

        await node2.Received(1).RequestVote(Arg.Any<RequestVoteData>());
        await node3.Received(1).RequestVote(Arg.Any<RequestVoteData>());
    }


    //Testing #7
    [Fact]
    public async Task WhenEmptyAppendIsReceivedNoElectionIsStarted()
    {
        Node node = new Node(1);
        await node.RequestAppendEntries(new RequestAppendEntriesData() { SentFrom = 2 });
        NodeState state = node.State;
        state.Should().Be(NodeState.Follower);
    }

    //Testing #4
    [Fact]
    public async Task WhenNoEmptyAppendIsReceivedAnElectionIsStarted()
    {
        Node node = new Node(1);
        INode node2 = Substitute.For<INode>();
        node.OtherNodes.Add(node2.Id, node2);
        node.Start();
        Thread.Sleep(500);
        NodeState state = node.State;
        state.Should().Be(NodeState.Candidate);
    }

    //Testing 5
    [Fact]
    public async Task WhenElectionTimeResetsItIsBetween150msAnd300ms()
    {
        Node node = new(1);
        node.Start();
        Thread.Sleep(350);
        node.TimeLeft.Should().BeLessThan(301);
        node.TimeLeft.Should().BeGreaterThan(149);
    }
    //Testing 5
    //[Fact]
    //public async Task WhenElectionTimeResetsItIsRandom()
    //{
    //    //call the test like 100 times and make sure that each time is different
    //    List<Node> nodes = new();
    //    List<int> times = new();

    //    for (int i = 0; i < 100; i++)
    //    {
    //        Node node = new(1);
    //        node.Start();
    //        nodes.Add(node);
    //        times.Add(node.TimeLeft);
    //    }

    //    var uniqueTimes = times.Distinct().ToList(); //ChatGPT helped me discover the Distinct method, something I didn't know existed
    //    uniqueTimes.Count.Should().BeGreaterThan(30);
    //}

    //Testing #10
    //[Fact]
    //public async Task FollowerWhoHasNotVotedAndInEarlierTermThanCandidateVotesYes()
    //{
    //    Node node = new Node(1);
    //    node.VotedFor.Should().Be(null);
    //    await node.RequestVote(new RequestVoteData() { CandidateId=3, Term=2 });
    //    //votedForTestCandidate.Should().Be(true); //trhe leader should keep track of it
    //    node.VotedFor.Should().Be(2);
    //}
    //Testing #10
    //[Fact]
    //public async Task FollowerWhoHasNotVotedAndInLaterTermThanCandidateVotesNo()
    //{
    //    var node = new Node( 1);
    //    node.CurrentTerm = 3;
    //    bool voteResult = await node.RequestVote(1, 2);
    //    voteResult.Should().Be(false);
    //}

    //Testing #TODO CHANGE THIS
    //[Fact]
    //public async Task FollowerWhoHasVotedThisTermVotesNoToAnyoneElse()
    //{
    //    var node = new Node(1);
    //    node.VotedFor = 2;
    //    node.CurrentTerm = 2;
    //    bool IsVotingForNode = await node.RequestVote(2, 3);
    //    IsVotingForNode.Should().Be(false);
    //}
    ////Testing #14
    //[Fact]
    //public async Task SecondVoteRequestFromSameCandidateInSameTermVotesNo()
    //{
    //    var node = new Node(1);
    //    node.CurrentTerm = 2;
    //    bool IsVotingForNode = await node.RequestVote(2, 2);
    //    IsVotingForNode.Should().Be(true);
    //    bool IsVotingForNode2 = await node.RequestVote(2, 2);
    //    IsVotingForNode2.Should().Be(false);
    //}
    ////Testing #15
    //[Fact]
    //public async Task FollowerWhoReceivesSecondVoteRequestFromSameCandidateForFutureTermShouldVoteYes()
    //{
    //    var node = new Node(1);
    //    node.CurrentTerm = 2;
    //    bool IsVotingForNode = await node.RequestVote(2, 2);
    //    IsVotingForNode.Should().Be(true);
    //    bool IsVotingForNode2 = await node.RequestVote(3, 2);
    //    IsVotingForNode2.Should().Be(true);
    //}

    //[Fact]
    //public async Task FollowerWhoReceivesVoteRequestForFutureTermShouldVoteYes()
    //{
    //    var node = new Node(1);
    //    node.VotedFor = 2;
    //    node.CurrentTerm = 2;
    //    bool IsVotingForNode = await node.RequestVote(3, 2);
    //    IsVotingForNode.Should().Be(true);
    //}
    //Testing #17
    [Fact]
    public async Task FollowerWhoReceivesAppendRequestSendsResponse()
    {
        Node node = new Node(1);
        await node.RequestAppendEntries(new RequestAppendEntriesData() { Term = 1, SentFrom = 2 });

    }

    //Testing #2
    //[Fact]
    //public async Task ReceivingAppendEntriesMakesNodeRememberWhoIsTheLeader()
    //{
    //    Node node = new Node(1);
    //    await node.AppendEntries(new RequestAppendEntriesData() { SentFrom = 2});
    //    node.LeaderId.Should().Be(2);
    //    await node.AppendEntries(new RequestAppendEntriesData() { SentFrom = 3, Entry="Hello! I'm not sus" });
    //    node.LeaderId.Should().Be(2);
    //}









}
