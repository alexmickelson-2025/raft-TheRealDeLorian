using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLibrary;

public record RequestAppendEntriesData
{
    public int Term { get; set; }
    public int LeaderId { get; set; }
    public int PrevLogIndex { get; set; }
    public int PrevLogTerm { get; set; }
    
    public List<LogEntry>? Entries { get; set; }
    public int LeaderCommitIndex { get; set; }
}
