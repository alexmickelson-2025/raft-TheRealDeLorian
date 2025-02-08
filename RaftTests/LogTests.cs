using NSubstitute;
using RaftLibrary;
using FluentAssertions;

namespace RaftTests.Log;

public class LogTests
{
    //testing something, not sure but i need to make sure this is working for #8 to work
    [Fact]
    public async Task HeartbeatTimerWorks()
    {
        Node leader = new Node(1);
        leader.Status = NodeStatus.Leader;
        INode node1 = Substitute.For<INode>();
        leader.OtherNodes.Add(node1.Id, node1);

        await leader.StartHeartbeatTimer();
    }

    [Fact]
    public async Task LeaderSendsHeartbeatEvery50ms()
    {
        Node leader = new Node(1);
        leader.Status = NodeStatus.Leader;
        INode node1 = Substitute.For<INode>();
        leader.OtherNodes.Add(node1.Id, node1);

        int heartbeatsReceivedBeforeStart = node1.HeartbeatsReceived;
        leader.StartHeartbeatTimer();
        Thread.Sleep(70);
        node1.HeartbeatsReceived.Should().BeGreaterThan(0);
        int heartbeatsreceivedafterfirstcheck = node1.HeartbeatsReceived;
        Thread.Sleep(70);
        node1.HeartbeatsReceived.Should().BeGreaterThan(heartbeatsreceivedafterfirstcheck);

    }




    // Testing #1
    [Fact]
    public async Task WhenLeaderReceivesClientCommandSendsLogEntryInNextAppendEntriesToAllNodes()
    {
        Node leader = new Node(1);
        leader.Status = NodeStatus.Leader;
        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes.Add(node1.Id, node1);
        leader.OtherNodes.Add(node2.Id, node2);

        await leader.RequestFromClient("Add 2");
        node1.Log.Should().NotBeNull();
        node2.Log.Should().NotBeNull();
    }

    // Testing #2
    [Fact]
    public async Task WhenLeaderReceivesClientCommandItIsAppendedToLeadersLogs()
    {
        Node leader = new Node(1);
        leader.Status = NodeStatus.Leader;

        await leader.RequestFromClient("Add 2");

        leader.Log.Count.Should().Be(1);
    }

    // Testing #3
    [Fact]
    public async Task WhenNodeIsNewLogIsEmpty()
    {
        Node node = new Node(1);

        node.Log.Count.Should().Be(0);
    }

    // Testing #4 
    [Fact]
    public async Task WhenLeaderWinsElectionAllFollowersNextIndexEqualsTheLeadersLastIndexPlusOne()
    {
        Node leader = new Node(1);

        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = new Dictionary<int, INode> { { node1.Id, node1 }, { node2.Id, node2 } };

        await leader.WinElection();
        leader.Status.Should().Be(NodeStatus.Leader);

        node1.NextIndex.Should().Be(leader.Log.Count + 1);
        node2.NextIndex.Should().Be(leader.Log.Count + 1);
    }

    // Testing #5
    [Fact]
    public async Task LeadersMaintainNextIndexToSendForEachFollower()
    {
        Node leader = new Node(1);
        leader.Status = NodeStatus.Leader;

        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = new Dictionary<int, INode> { { node1.Id, node1 }, { node2.Id, node2 } };
        leader.NextIndicesToSend = new List<(int nodeId, int nextIndex)>();
    }





    // Testing #8
    //[Fact]
    //public async Task LeaderCommitsLogsOnMajorityConfirmation()
    //{
    //    Node leader = new(1);
    //    leader.State = NodeState.Leader;
    //    INode node1 = Substitute.For<INode>();
    //    node1.CurrentTerm = 1;
    //    node1.Log = new();
    //    INode node2 = Substitute.For<INode>();
    //    node2.CurrentTerm = 1;
    //    node2.Log = new();
    //    INode node3 = Substitute.For<INode>();
    //    node3.CurrentTerm = 1;
    //    node3.Log = new();
    //    INode node4 = Substitute.For<INode>();
    //    node4.CurrentTerm = 1;
    //    node4.Log = new();

    //    leader.OtherNodes = [node1, node2, node3, node4];

    //    leader.CommitIndex.Should().Be(0);
    //    RequestAppendEntriesData data = new() { SentFrom = 2, Entry = "New log", Term = 1, LeaderCommitIndex = leader.CommitIndex };
    //    node1.RequestAppendEntries(data).Returns(true);
    //    node2.RequestAppendEntries(data).Returns(true);
    //    node3.RequestAppendEntries(data).Returns(true);
    //    node4.RequestAppendEntries(data).Returns(false);
    //    foreach (INode node in leader.OtherNodes)
    //    {
    //        await node.RequestAppendEntries(data);
    //    }
    //    //need some way of "counting" the votes
    //    leader.CommitIndex.Should().Be(1);
    //}


}
