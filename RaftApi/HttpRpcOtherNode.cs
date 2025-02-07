using RaftLibrary;

public class HttpRpcOtherNode : INode
{
  public int Id { get; }
  public string Url { get; }
    public int CurrentTerm { get ; set; }
  

    private HttpClient client = new();
 
  public HttpRpcOtherNode(int id, string url)
  {
    Id = id;
    Url = url;
  }
 
  public async Task RequestAppendEntries(RequestAppendEntriesData request)
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
 
  public async Task RequestVote(RequestVoteData request)
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
 
  public async Task ResponseVote(RespondVoteData response)
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

    public Task RespondVote(RespondVoteData data)
    {
        throw new NotImplementedException();
    }

    public void AddOtherNodes(List<INode> nodes)
    {
        throw new NotImplementedException();
    }
}