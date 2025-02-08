using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLibrary;

public enum NodeStatus
{
    Follower,
    Candidate,
    Leader
}
