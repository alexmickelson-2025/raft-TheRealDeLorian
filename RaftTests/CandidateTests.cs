using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using RaftLibrary;

namespace RaftTests
{
    public class CandidateTests
    {

        //Testing #11
        [Theory]
        [InlineData(1)]
        [InlineData(456)]
        [InlineData(230)]
        public async Task NewCandidateVotesForItself(int id)
        {
            Node node = new([], id);
            await node.TimeoutElection();
            node.VotedFor.Should().Be(id);
        }

        //Testing #18
        [Fact]
        public async Task CandidateRejectsAppendEntriesFromPreviousTerm()
        {
            Node node = new([], 1);
            node.CurrentTerm = 2;
            bool response = await node.AppendEntries(new RPCData {SentFrom = 2, Term = 1});
            response.Should().Be(false);
        }

        //Testing 6
        [Fact]
        public async Task NewElectionIncrementsTerm()
        {
            Node node = new([], 1);
            int startingTerm = node.CurrentTerm;
            await node.TimeoutElection();
            node.CurrentTerm.Should().BeGreaterThan(startingTerm);
	
        }

        //Testing #16
        [Fact]
        public async Task WhenNodeIsCandidateAndTimerExpiresNewElectionStarts()
        {
            Node node = new([new Node([], 2)], 1);
            Thread.Sleep(500);
            node.State.Should().Be(NodeState.Candidate);
            node.CurrentTerm.Should().Be(2);
            node.VotedFor.Should().Be(1);

            Thread.Sleep(node.TimeLeft);
            node.State.Should().Be(NodeState.Candidate);
            node.CurrentTerm.Should().Be(3);
            node.VotedFor.Should().Be(1);
        }


        //Testing #8
        //Single cluster
        [Fact]
        public async Task SingleClusterCandidateBecomesLeaderWhenMajorityOfVotes()
        {
            Node node = new([], 1);
            await node.TimeoutElection();
            node.State.Should().Be(NodeState.Leader);
        }



        //Testing #8    
        //3-node cluster





        //Testing #8    
        //5-node cluster


        //Testing #8
        //11-node cluster



    }
}
