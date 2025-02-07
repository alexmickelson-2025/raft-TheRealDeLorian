using FluentAssertions;
using NSubstitute;
using RaftLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftTests;

public class InClassTests
{
    [Fact]
    public async Task GivenNodeIsLeaderWithElectionLoopWhenTheyGetPausedOtherNodesDontGetHeartbeat400ms()
    {
        Node leader = new Node(1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        leader.OtherNodes.Add(node1.Id, node1);
        leader.Start();

        int nodeHeartbeatsBeforePause = node1.HeartbeatsReceived;

        leader.Pause();
        Thread.Sleep(400);
        node1.HeartbeatsReceived.Should().Be(nodeHeartbeatsBeforePause);
    }

    [Fact]
    public async Task GivenNodeIsLeaderWithElectionLoopWhenPauseGetsNoHeartbeatsThenResume()
    {
        Node leader = new Node(1);
        leader.State = NodeState.Leader;
        INode node1 = Substitute.For<INode>();
        leader.OtherNodes.Add(node1.Id, node1);
        int nodeHeartbeatsBeforePause = node1.HeartbeatsReceived;

        leader.Start(); //TODO: I don't think the start is working the way I think it does
        Thread.Sleep(400);

        leader.Pause();
        Thread.Sleep(400);
        node1.HeartbeatsReceived.Should().Be(nodeHeartbeatsBeforePause);

        leader.Resume();
        Thread.Sleep(1000);
        node1.HeartbeatsReceived.Should().BeGreaterThan(nodeHeartbeatsBeforePause);
    }


}
