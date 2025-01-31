

namespace RaftLibrary;

public record ResponseEntriesData
{
    public int Term { get; set; }
    public bool Success { get; set; }
    public int SentFromNodeId { get; set; }
}
