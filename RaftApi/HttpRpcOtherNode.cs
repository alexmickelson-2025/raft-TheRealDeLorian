using RaftLibrary;

public class HttpRpcOtherNode : INode //this class is acting as another node to make it easier for me to make a node communicate with the others.
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
 
  public async Task RespondVote(RespondVoteData response)
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

    public Task Start()
    {
        throw new NotImplementedException();
    }

    public Task Timeout()
    {
        throw new NotImplementedException();
    }

    public void AddOtherNodes(List<INode> nodes)
    {
        throw new NotImplementedException();
    }
}