using System.Text.Json;
using RaftLibrary;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");


var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? throw new Exception("NODE_ID environment variable not set");
var otherNodesRaw = Environment.GetEnvironmentVariable("OTHER_NODES") ?? throw new Exception("OTHER_NODES environment variable not set");
var nodeIntervalScalarRaw = Environment.GetEnvironmentVariable("NODE_INTERVAL_SCALAR") ?? throw new Exception("NODE_INTERVAL_SCALAR environment variable not set");

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();
logger.LogInformation("Node ID {name}", nodeId);
logger.LogInformation("Other nodes environment config: {}", otherNodesRaw);


INode[] otherNodes = otherNodesRaw
  .Split(";")
  .Select(s => new HttpRpcOtherNode(int.Parse(s.Split(",")[0]), s.Split(",")[1]))
  .ToArray();


logger.LogInformation("other nodes {nodes}", JsonSerializer.Serialize(otherNodes));


var node = new Node(int.Parse(nodeId))
{

};

Node.NodeIntervalScalar = int.Parse(nodeIntervalScalarRaw);

node.Start(); //this is what gets the node going. If data isnt being updated it's because the Node class isn't configured to update it. 

app.MapGet("/health", () => "healthy");

app.MapGet("/nodeData", () =>
{
    return new NodeData()
  {
    Id = node.Id,
    State = node.State,
    ElectionTimeout = (int)node.LastInterval - (int)node.ElectionStopwatch.ElapsedMilliseconds,
    Term = node.CurrentTerm,
    CurrentTermLeader = node.LeaderId,
    CommittedEntryIndex = node.CommitIndex,
    Log = node.Log,
    StateMachineState = node.StateMachine.GetState()
  };
});

app.MapPost("/request/appendEntries", async (RequestAppendEntriesData request) =>
{
  logger.LogInformation("received append entries request {request}", request);
  await node.RequestAppendEntries(request);
});

app.MapPost("/request/vote", async (RequestVoteData request) =>
{
    logger.LogInformation("received vote request {request}", request);
    await node.RequestVote(request);
});

app.MapPost("/response/appendEntries", async (ResponseEntriesData response) =>
{
  logger.LogInformation("received append entries response {response}", response);
  await node.RespondAppendEntries(response);
});

app.MapPost("/response/vote", async (RespondVoteData response) =>
{
    logger.LogInformation("received vote response {response}", response);
    await node.RespondVote(response);
});

app.MapPost("/request/command", async (ClientCommandData data) =>
{
  await node.SendCommand(data);
});

app.Run();