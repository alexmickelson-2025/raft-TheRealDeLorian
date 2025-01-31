

namespace RaftLibrary;

public record VoteRequestData
{
    public int Term { get; set; }
    public bool Success { get; set; }
    public int SentFromNodeId { get; set; }
}
