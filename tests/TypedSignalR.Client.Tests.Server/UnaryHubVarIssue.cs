using Microsoft.AspNetCore.SignalR;
using TypedSignalR.Client.Tests.Shared;

namespace TypedSignalR.Client.Tests.Server.Hubs;

public class UnaryHubVarIssue : Hub, IUnaryHubVarIssue
{
    private readonly ILogger<UnaryHubVarIssue> _logger;

    public UnaryHubVarIssue(ILogger<UnaryHubVarIssue> logger)
    {
        _logger = logger;
    }

    public Task<int> Add(int x, int y)
    {
        _logger.Log(LogLevel.Information, "UnaryHub.Add");

        return Task.FromResult(x + y);
    }
}