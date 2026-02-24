using Microsoft.Extensions.Configuration;

public class TerminalTokenStore
{
    public string TerminalId { get; }
    private readonly string _token;

    public TerminalTokenStore(IConfiguration cfg)
    {
        TerminalId = cfg["Terminal:Id"] ?? "POS-01";
        _token = cfg["Terminal:Token"] ?? "";
    }

    public bool Validate(string terminalId, string token)
        => terminalId == TerminalId && !string.IsNullOrWhiteSpace(_token) && token == _token;
}