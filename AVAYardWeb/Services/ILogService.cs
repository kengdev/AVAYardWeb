namespace AVAYardWeb.Services
{
    public interface ILogService
    {
        void AddLog(string action, string module, string refCode,object? before, object? after, string createBy);
        Task SaveAsync();
    }
}
