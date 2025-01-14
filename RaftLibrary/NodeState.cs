using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLibrary;

public enum NodeState
{
    Follower,
    Candidate,
    Leader
}
