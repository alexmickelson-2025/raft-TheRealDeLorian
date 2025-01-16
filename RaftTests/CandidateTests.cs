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
    }
}
