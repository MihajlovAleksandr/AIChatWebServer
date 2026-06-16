using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Themes;
using System.Text.Json;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IThemeRepository : ITransactionalScope<IThemeRepository>
    {
        Task<Theme?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<Theme?> GetByNameAsync(
                    Guid userId,
                    string name,
                    CancellationToken ct = default);

        Task<List<Theme>> GetByUserIdAsync(
            Guid? userId,
            CancellationToken ct = default);

        Task<List<Theme>> GetByTypeAsync(
            ThemeType type,
            CancellationToken ct = default);

        Task<List<Theme>> GetAllAsync(
            CancellationToken ct = default);

        Task<Guid> CreateAsync(
                    Guid? userId,
                    string name,
                    ThemeType type,
                    JsonDocument content,
                    CancellationToken ct = default);

        Task<Theme> UpdateAsync(
            Theme theme,
            CancellationToken ct = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken ct = default);
        Task<Theme?> GetSelectedThemeByConnectionIdAsync(
            Guid connectionId,
            CancellationToken ct = default);

        Task UpsertSelectedThemeAsync(
            Guid connectionId,
            Guid themeId,
            CancellationToken ct = default);

        Task<bool> DeleteSelectedThemeAsync(
            Guid connectionId,
            CancellationToken ct = default);

        Task<long> GetUsagesCountAsync(
            Guid themeId,
            CancellationToken ct = default);
    }
}