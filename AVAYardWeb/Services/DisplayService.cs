using Microsoft.AspNetCore.SignalR;

public interface IDisplayService
{
    Task ShowAsync(string url);
    Task HideAsync();
}

public class DisplayService : IDisplayService
{
    private readonly IHubContext<DisplayHub> _hub;
    private readonly TerminalTokenStore _terminal;

    public DisplayService(IHubContext<DisplayHub> hub, TerminalTokenStore terminal)
    {
        _hub = hub;
        _terminal = terminal;
    }

    public async Task ShowAsync(string url)
    {
        var terminalId = _terminal.TerminalId;

        if (DisplayHub.TryGetConn(terminalId, out var connId))
        {
            await _hub.Clients.Client(connId).SendAsync("show", url);
        }
        // ถ้า offline จะไม่ throw ให้ flow POS ไม่พัง
    }

    public async Task HideAsync()
    {
        var terminalId = _terminal.TerminalId;

        if (DisplayHub.TryGetConn(terminalId, out var connId))
        {
            await _hub.Clients.Client(connId).SendAsync("hide");
        }
    }
}