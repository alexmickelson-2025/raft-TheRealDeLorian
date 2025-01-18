


//READ THIS BEFORE GRADING
//THIS CODE IN THIS FILE IS NOT IN USE AT ALL ANYWHERE IN THE PROJECT
//It is only here in a .cs file so I can more easily see and format it. 
//The teams message cuts off the window so it's hard to see the code

namespace NoNamespace;

private class RaftSimulationNode : INode
{
    public string Message { get; private set; } = "";
    public static int NetworkRequestDelay { get; set; } = 1000;
    public static int NetworkResponseDelay { get; set; } = 0;
    public bool SimulationRunning { get; private set; } = false;
    public NodeStatus Status => InnerNode.Status;
    public int CurrentTerm => InnerNode.CurrentTerm;

    public int Id => InnerNode.Id;

    public RaftNode InnerNode { get; }

    public int CurrentTermLeader => InnerNode.CurrentTermLeader;

    public Task RequestAppendEntries(AppendEntriesData request)
    {
        if (!SimulationRunning)
            return Task.CompletedTask;
        Task.Delay(NetworkRequestDelay).ContinueWith(async (_previousTask) =>
        {
            Message = $"Received Append Entries from {request.LeaderId}";
            await InnerNode.RequestAppendEntries(request);
        });
        return Task.CompletedTask;
    }

    public Task RequestVote(VoteRequestData request)
    {
        if (!SimulationRunning)
            return Task.CompletedTask;
        Task.Delay(NetworkRequestDelay).ContinueWith(async (_previousTask) =>
        {
            Message = $"Received request to vote for {request.CandidateId} for term {request.Term}";
            await InnerNode.RequestVote(request);
        });
        return Task.CompletedTask;
    }

    public Task ResponseVote(VoteResponseData response)
    {
        if (!SimulationRunning)
            return Task.CompletedTask;
        Task.Delay(NetworkResponseDelay).ContinueWith(async (_previousTask) =>
        {
            Message = $"Sending vote for term {response.TermId}";
            await InnerNode.ResponseVote(response);
        });
        return Task.CompletedTask;
    }

    public Task RespondAppendEntries(RespondEntriesData response)
    {
        if (!SimulationRunning)
            return Task.CompletedTask;
        Task.Delay(NetworkResponseDelay).ContinueWith(async (_previousTask) =>
        {
            Message = $"";
            await InnerNode.RespondAppendEntries(response);
        });
        return Task.CompletedTask;
    }

    public RaftSimulationNode(RaftNode node)
    {
        InnerNode = node;
    }

    public void StopSimulationLoop()
    {
        Console.WriteLine("canceling election");
        InnerNode.CancellationTokenSource.Cancel();
        SimulationRunning = false;
    }
    public void StartSimulationLoop()
    {
        InnerNode.CancellationTokenSource = new CancellationTokenSource();
        InnerNode.RunElectionLoop();
        SimulationRunning = true;
    }

    public async Task SendCommand(ClientCommandData data)
    {
        if (!SimulationRunning)
            return;
        await InnerNode.SendCommand(data);
    }
}
