using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class DisplayHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> Map = new();
    private readonly TerminalTokenStore _store;

    public DisplayHub(TerminalTokenStore store) => _store = store;

    public Task RegisterTerminal(string terminalId)
    {
        var auth = Context.GetHttpContext()?.Request.Headers["Authorization"].ToString() ?? "";
        var token = auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? auth["Bearer ".Length..].Trim()
            : "";

        if (!_store.Validate(terminalId, token))
            throw new HubException("Invalid terminal token");

        Map[terminalId] = Context.ConnectionId;
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? ex)
    {
        foreach (var kv in Map)
            if (kv.Value == Context.ConnectionId)
                Map.TryRemove(kv.Key, out _);

        return base.OnDisconnectedAsync(ex);
    }

    public static bool TryGetConn(string terminalId, out string connId)
        => Map.TryGetValue(terminalId, out connId);
}