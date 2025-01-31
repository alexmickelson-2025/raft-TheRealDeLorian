using RaftLibrary;

public class HttpRpcOtherNode : INode
{
  public int Id { get; }
  public string Url { get; }
    public int CurrentTerm { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int HeartbeatsReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    int INode.Id { get => Id; set => throw new NotImplementedException(); }
    public int LeaderId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public INode[] OtherNodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public StateMachine StateMachine { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public NodeState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int TimeLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int NextIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int CommitIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public List<(int nodeId, int nextIndex)> NextIndicesToSend { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? VotedFor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public List<RPCData> Log { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private HttpClient client = new();
 
  public HttpRpcOtherNode(int id, string url)
  {
    Id = id;
    Url = url;
  }
 
  public async Task RequestAppendEntries(RPCData request)
  {
    try
    {
      await client.PostAsJsonAsync(Url + "/request/appendEntries", request);
    }
    catch (HttpRequestException)
    {
      Console.WriteLine($"node {Url} is down");
    }
  }
 
  public async Task RequestVote(VoteRequestData request)
  {
    try
    {
      await client.PostAsJsonAsync(Url + "/request/vote", request);
    }
    catch (HttpRequestException)
    {
      Console.WriteLine($"node {Url} is down");
    }
  }
 
  public async Task RespondAppendEntries(ResponseEntriesData response)
  {
    try
    {
      await client.PostAsJsonAsync(Url + "/response/appendEntries", response);
    }
    catch (HttpRequestException)
    {
      Console.WriteLine($"node {Url} is down");
    }
  }
 
  public async Task ResponseVote(VoteResponseData response)
  {
    try
    {
      await client.PostAsJsonAsync(Url + "/response/vote", response);
    }
    catch (HttpRequestException)
    {
      Console.WriteLine($"node {Url} is down");
    }
  }
 
  public async Task SendCommand(ClientCommandData data)
  {
    await client.PostAsJsonAsync(Url + "/request/command", data);
  }

    public Task<bool> RequestVote(int term, int candidateId)
    {
        throw new NotImplementedException();
    }

    public Task Start()
    {
        throw new NotImplementedException();
    }

    public Task Timeout()
    {
        throw new NotImplementedException();
    }
}