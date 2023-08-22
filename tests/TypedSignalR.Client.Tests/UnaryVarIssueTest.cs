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
        ISignalRConnection holder = new SignalRConnection();
        await holder.InitializeAsync();

        HubConnection conn = holder.Connection;

        var hub = conn.CreateHubProxy<IUnaryHubVarIssue>();

        var res = hub.Add(2,3);
        Assert.Equal(res,5);
        // case 1. holder's type is ConnectionHolder
        //var holder = new ConnectionHolder();

        //case 2. holder's type is IConnectionHolder
        //var holder = ConnectionHolder.Create();

        // ---------

        // case 1. use var
        //var connection = holder.HubConnection;
        //var hubProxy = connection.CreateHubProxy<IUnaryHub3>();

        // case 2. use property directly
        //var hubProxy = holder.HubConnection.CreateHubProxy<IUnaryHub3>();

        //await holder.HubConnection.StartAsync();

        //var x = Random.Shared.Next();
        //var y = Random.Shared.Next();

        //var added = await hubProxy.Add(x, y);

        //Assert.Equal(added, x + y);

        //await holder.HubConnection.StopAsync();
        //await holder.HubConnection.DisposeAsync();
    }
}

public interface ISignalRConnection : IAsyncDisposable
{
    HubConnection Connection { get; }
    Task InitializeAsync(string hubRoute);
}

public sealed class SignalRConnection() : ISignalRConnection
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

        var url = new Uri("http://localhost:5105", "Hubs/UnaryHubVarIssue");
        _hubConnection = new HubConnectionBuilder()
                         .WithUrl(url, options =>
                         {
                             options.AccessTokenProvider = async () =>
                             {
                                // get token provider from services...
                                // fake here

                                return "abc123";
                             };
                         })
                         .WithAutomaticReconnect()
                         .Build();
        await _hubConnection.StartAsync();
        _initialized = true;
    }
}