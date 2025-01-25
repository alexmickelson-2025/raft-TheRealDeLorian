﻿using NSubstitute;
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

    // Testing #1
    [Fact]
    public async Task WhenLeaderReceivesClientCommandSendsLogEntryInNextAppendEntriesToAllNodes()
    {
        Node leader = new Node([], 1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = [node1, node2];

        await leader.RequestFromClient("Add 2");
        node1.Log.Should().NotBeNull();
        node2.Log.Should().NotBeNull();
    }

    // Testing #2
    [Fact]
    public async Task WhenLeaderReceivesClientCommandItIsAppendedToLeadersLogs()
    {
        Node leader = new Node([], 1);
        leader.State = NodeState.Leader;

        await leader.RequestFromClient("Add 2");

        leader.Log.Count.Should().Be(1);
    }

    // Testing #3
    [Fact]
    public async Task WhenNodeIsNewLogIsEmpty()
    {
        Node node = new Node([], 1);

        node.Log.Count.Should().Be(0);
    }

    // Testing #4 
    [Fact]
    public async Task WhenLeaderWinsElectionAllFollowersNextIndexEqualsTheLeadersLastIndexPlusOne()
    {
        Node leader = new Node([], 1);

        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = [node1, node2];

        await leader.WinElection();
        leader.State.Should().Be(NodeState.Leader);

        node1.NextIndex.Should().Be(leader.Log.Count + 1);
        node2.NextIndex.Should().Be(leader.Log.Count + 1);
    }

    // # Testing #5
    [Fact]
    public async Task LeadersMaintainNextIndexToSendForEachFollower()
    {
        Node leader = new Node([], 1);
        leader.State = NodeState.Leader;

        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = [node1, node2];
        leader.NextIndicesToSend = new List<(int nodeId, int nextIndex)>();
    }

    // # Testing 6
    [Fact]
    public async Task HighestCommittedIndexFromLeaderIsIncludedInAllAppendEntries()
    {
        Node leader = new([], 1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        node1.CurrentTerm = 1;
        node1.Log = new();
        INode node2 = Substitute.For<INode>();
        node2.CurrentTerm = 1;
        node2.Log = new();

        leader.OtherNodes = [node1, node2];

        RPCData data = new() { SentFrom = 2, Entry = "New log", Term = 1, LeaderCommitIndex = leader.CommitIndex };
        await node1.AppendEntries(data);

    }

    // # Testing 7
    [Fact]
    public async Task FollowerUpdatesStateMachineUponLearningOfNewCommittedEntry()
    {
        Node follower = new([], 1);
        follower.CommitIndex.Should().Be(0);

        RPCData data = new() { SentFrom = 2, Entry = "New log", Term = 1, LeaderCommitIndex = 4 };
        await follower.AppendEntries(data);

        follower.CommitIndex.Should().Be(4);


    }        



}
