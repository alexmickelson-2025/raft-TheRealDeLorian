using RaftLibrary;

namespace RaftTests
{
    public class FollowerTests
    {
        [Fact]
        public void TestFollowerTermIncrementOnElectionTimeout()
        {
            Node node = new Node(1);
            node.TimeoutElection();
            Assert.Equal(2, node.GetTerm());
        }
    }
}
