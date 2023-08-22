namespace TypedSignalR.Client.Tests.Shared;

public interface IUnaryHubVarIssue
{
    Task<int> Add(int x, int y);
}