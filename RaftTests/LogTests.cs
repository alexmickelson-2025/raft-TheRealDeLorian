using NSubstitute;
using RaftLibrary;
using FluentAssertions;

namespace RaftTests;

public class LogTests
{
    [Fact]
    public async Task WhenNodeIsLeaderItReceivesAppendEntriesRequestsFromFollowers()
    {
        Node leader = new Node([], 1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        leader.OtherNodes = [node1];
        RPCData data = new() { SentFrom = 2, Entry = "New log", Term = 1 };
        await leader.AppendEntries(data);
        leader.Log.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenLeaderReceivesClientCommandSendsLogEntryInNextAppendEntriesToAllNodes()
    {
        Node leader = new Node([], 1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        INode node2 = Substitute.For<INode>();
        leader.OtherNodes = [node1, node2];

        await leader.RequestFromClient("Add 2");
        node1.Log.Should().NotBeNull();
        node2.Log.Should().NotBeNull();
    }

}
