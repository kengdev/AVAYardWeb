using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[Authorize] // ✅ ต้อง login cookie ก่อน
[ApiController]
[Route("api/display")]
public class DisplayController : ControllerBase
{
    private readonly IHubContext<DisplayHub> _hub;
    private readonly TerminalTokenStore _terminal;

    public DisplayController(IHubContext<DisplayHub> hub, TerminalTokenStore terminal)
    {
        _hub = hub;
        _terminal = terminal;
    }

    [HttpPost("show")]
    public async Task<IActionResult> Show([FromBody] ShowReq req)
    {
        // ✅ force ใช้ terminal เดียว (กันคนอื่นสั่ง terminal แปลกๆ)
        var terminalId = _terminal.TerminalId;

        var url = $"https://yardweb.avagloballogistics.com/orderpayment/displaypaymentpos/{req.Code}";

        if (DisplayHub.TryGetConn(terminalId, out var connId))
        {
            await _hub.Clients.Client(connId).SendAsync("show", url);
            return Ok(new { ok = true });
        }

        return Ok(new { ok = false, message = "POS display is offline" });
    }

    [HttpPost("hide")]
    public async Task<IActionResult> Hide()
    {
        var terminalId = _terminal.TerminalId;

        if (DisplayHub.TryGetConn(terminalId, out var connId))
        {
            await _hub.Clients.Client(connId).SendAsync("hide");
            return Ok(new { ok = true });
        }

        return Ok(new { ok = false, message = "POS display is offline" });
    }

    public record ShowReq(string Code);
}