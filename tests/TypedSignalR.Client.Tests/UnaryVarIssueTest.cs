using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client.Tests.Shared;
using Xunit;

namespace TypedSignalR.Client.Tests;

// Lunch TypedSignalR.Client.Tests.Server.csproj before test!
public class UnaryHubVarIssueTest
{
    [Fact]
    public async Task Add()
    {
        await using ISignalRConnection holder = new SignalRConnection();
        await holder.InitializeAsync("Hubs/UnaryHubVarIssue");

        var conn = holder.Connection;

        var hub = conn.CreateHubProxy<IUnaryHubVarIssue>();

        var res = await hub.Add(2,3);
        Assert.Equal(5, res);
    }
}

public interface ISignalRConnection : IAsyncDisposable
{
    HubConnection Connection { get; }
    Task InitializeAsync(string hubRoute);
}

public sealed class SignalRConnection : ISignalRConnection
{
    private HubConnection? _hubConnection;
    private bool _initialized;

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is null)
        {
            return;
        }

        await Connection.StopAsync();
        await Connection.DisposeAsync();
    }

    public HubConnection Connection
    {
        get
        {
            if (!_initialized || _hubConnection is null)
            {
                throw new
                    InvalidOperationException($"Hub connection has to be initialized before using it - call {nameof(InitializeAsync)}");
            }

            return _hubConnection;
        }
    }

    public async Task InitializeAsync(string hubRoute)
    {
        if (_initialized)
        {
            throw new
                InvalidOperationException("Hub connection is already initialized");
        }

        _hubConnection = new HubConnectionBuilder()
                         .WithUrl($"http://localhost:5105/{hubRoute}", options =>
                         {
                             options.AccessTokenProvider = async () =>
                             {
                                // get token provider from services...
                                // fake here
                                var token = await Task.FromResult("abc123");

                                return token;
                             };
                         })
                         .WithAutomaticReconnect()
                         .Build();
        await _hubConnection.StartAsync();
        _initialized = true;
    }
}