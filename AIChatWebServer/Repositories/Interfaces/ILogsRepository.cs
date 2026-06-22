using AIChatWebServer.Models.LogModel;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface ILogsRepository
    {
        Task<IEnumerable<LogModel>> GetAll(
            CancellationToken cancellationToken = default);

        Task<LogModel?> GetById(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<LogModel>> GetByLevel(
            string level,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<LogModel>> GetBySource(
            string source,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<LogModel>> GetByDateRange(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        Task<LogModel> Add(
            string level,
            string message,
            string? source = null,
            CancellationToken cancellationToken = default);

        Task<int> DeleteOldLogs(
            DateTime cutoffDate,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteById(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}