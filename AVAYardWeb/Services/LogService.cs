using AVAYardWeb.Models.Entities;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace AVAYardWeb.Services;

public class LogService : ILogService
{
    private readonly DbavayardContext db;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // ✅ ไม่ escape ภาษาไทย
        WriteIndented = false
    };

    public LogService(DbavayardContext _db)
    {
        db = _db;
    }

    public void AddLog(string action, string module, string refCode, object? before, object? after, string createBy)
    {
        var log = new LogAction
        {
            Action = action,
            Module = module,
            RefCode = refCode,
            BeforeData = before == null ? null : JsonSerializer.Serialize(before, _jsonOptions),
            AfterData = after == null ? null : JsonSerializer.Serialize(after, _jsonOptions),
            CreateBy = createBy,
            CreateDate = DateTime.Now
        };

        db.LogActions.Add(log);
    }

    public Task SaveAsync() => db.SaveChangesAsync();
}
