using FluentAssertions;
using RaftLibrary;

namespace RaftTests.Election;

public class CandidateTests
{

    //Testing #11
    // [Theory]
    // [InlineData(1)]
    // [InlineData(456)]
    // [InlineData(230)]
    // public async Task NewCandidateVotesForItself(int id)
    // {
    //     Node node = new(id);
    //     await node.Timeout();
    //     node.VotedFor.Should().Be(id);
    // }

    //Testing #18
    [Fact]
    public async Task CandidateRejectsAppendEntriesFromPreviousTerm()
    {
        Node node = new(1);
        node.CurrentTerm = 2;
        await node.RequestAppendEntries(new RequestAppendEntriesData { LeaderId = 2, Term = 1 });
    }

    //Testing 6
    // [Fact]
    // public async Task NewElectionIncrementsTerm()
    // {
    //     Node node = new(1);
    //     int startingTerm = node.CurrentTerm;
    //     await node.Timeout();
    //     node.CurrentTerm.Should().BeGreaterThan(startingTerm);

    // }

    ////Testing #16
    //[Fact]
    //public async Task WhenNodeIsCandidateAndTimerExpiresNewElectionStarts()
    //{
    //    Node node = new([new Node(2)], 1);
    //    await node.Timeout();
    //    node.State.Should().Be(NodeState.Candidate);
    //    node.CurrentTerm.Should().Be(2);
    //    node.VotedFor.Should().Be(1);

    //    await node.Timeout();
    //    node.State.Should().Be(NodeState.Candidate);
    //    node.CurrentTerm.Should().Be(3);
    //    node.VotedFor.Should().Be(1);
    //}


    //Testing #8
    //Single cluster
    // [Fact]
    // public async Task SingleClusterCandidateBecomesLeaderWhenMajorityOfVotes()
    // {
    //     Node node = new(1);
    //     await node.Timeout();
    //     node.Status.Should().Be(NodeStatus.Leader);
    // }

    //Testing #8    
    //3-node cluster
    //[Fact]
    //public async Task ThreeNodeClusterCandidateBecomesLeaderWhenReceivesMajorityOfVotes()
    //{
    //    var node1 = new Node(1);
    //    var node2 = new Node(2);
    //    var node3 = new Node(3);

    //    //node1.OtherNodes = [node2, node3];
    //    //node2.OtherNodes = [node1, node3];
    //    //node3.OtherNodes = [node1, node2];

    //    await node1.Timeout();
    //    await node2.RequestVote(2, 1);
    //    await node3.RequestVote(2, 1);

    //    node1.State.Should().Be(NodeState.Leader);
    //}


    //Testing #12
    // [Fact]
    // public async Task WhenCandidateReceivesAppendEntriesFromLaterTermItBecomesFollower()
    // {
    //     Node node = new(1);
    //     Node node2 = new(2);
    //     node.OtherNodes = new INode[2];
    //     node2.OtherNodes = new INode[2];
    //     node.OtherNodes[0] = node2;
    //     node2.OtherNodes[0] = node;
    //     node2.CurrentTerm = 5;

    //     await node.Start();
    //     await node.Timeout();
    //     node.Status.Should().Be(NodeStatus.Candidate);

    //     await node.RequestAppendEntries(new RequestAppendEntriesData { LeaderId = 2, Term = 5 });
    //     node.Status.Should().Be(NodeStatus.Follower);
    // }

    //Testing #13
    //[Fact]
    //public async Task WhenCandidateReceivesAppendEntriesFromEqualTermItBecomesFollower()
    //{
    //    Node node = new(1);
    //    Node node2 = new(2);
    //    node.OtherNodes = [node2];
    //    node2.OtherNodes = [node];
    //    node2.CurrentTerm = 1;

    //    await node.Timeout();
    //    node.State.Should().Be(NodeState.Candidate);

    //    await node.AppendEntries(new RequestAppendEntriesData { SentFrom = 2, Term = 1 });
    //    node.State.Should().Be(NodeState.Follower);
    //}

    //Testing #19
    //[Fact]
    //public async Task WhenCandidateWinsElectionItSendsHeartbeat()
    //{
    //    Node node = new(1);
    //    INode node2 = Substitute.For<INode>();
    //    node.OtherNodes = [node2];
    //    node2.OtherNodes = [node];

    //    await node.WinElection();
    //    node.State.Should().Be(NodeState.Leader);
    //    node2.State.Should().Be(NodeState.Follower);
    //    node2.HeartbeatsReceived.Should().BeGreaterThanOrEqualTo(1);
    //}


     // [Fact]
    // public async Task GivenNodeIsLeaderWithElectionLoopWhenTheyGetPausedOtherNodesDontGetHeartbeat400ms()
    // {
    //     Node leader = new Node(1);
    //     leader.Status = NodeStatus.Leader;
    //     INode node1 = Substitute.For<INode>();
    //     leader.OtherNodes.Add(node1.Id, node1);
    //     leader.Start();

    //     int nodeHeartbeatsBeforePause = node1.HeartbeatsReceived;

    //     leader.Pause();
    //     Thread.Sleep(400);
    //     node1.HeartbeatsReceived.Should().Be(nodeHeartbeatsBeforePause);
    // }

    // [Fact]
    // public async Task GivenNodeIsLeaderWithElectionLoopWhenPauseGetsNoHeartbeatsThenResume()
    // {
    //     Node leader = new Node(1);
    //     leader.Status = NodeStatus.Leader;
    //     INode node1 = Substitute.For<INode>();
    //     leader.OtherNodes.Add(node1.Id, node1);
    //     int nodeHeartbeatsBeforePause = node1.HeartbeatsReceived;

    //     leader.Start(); //TODO: I don't think the start is working the way I think it does
    //     Thread.Sleep(400);

    //     leader.Pause();
    //     Thread.Sleep(400);
    //     node1.HeartbeatsReceived.Should().Be(nodeHeartbeatsBeforePause);

    //     leader.Resume();
    //     Thread.Sleep(1000);
    //     node1.HeartbeatsReceived.Should().BeGreaterThan(nodeHeartbeatsBeforePause);
    // }
}
