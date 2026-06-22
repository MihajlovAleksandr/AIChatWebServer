using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Themes;
using System.Text.Json;

namespace AIChatWebServer.Services.Interfaces.Users
{
    public interface IThemeService
    {
        Task<Theme> GetThemeByIdAsync(Guid id, CancellationToken ct = default);
        Task<Theme> GetThemeByNameAsync(Guid userId, string name, CancellationToken ct = default);
        Task<IReadOnlyList<Theme>> GetThemesByUserIdAsync(Guid? userId, CancellationToken ct = default);
        Task<IReadOnlyList<Theme>> GetThemesByTypeAsync(ThemeType type, CancellationToken ct = default);
        Task<IReadOnlyList<Theme>> GetAllThemesAsync(CancellationToken ct = default);
        Task<Guid> CreateThemeAsync(Guid userId, string name, ThemeType type, JsonDocument content, CancellationToken ct = default);
        Task<Theme> UpdateThemeAsync(Theme theme, CancellationToken ct = default);
        Task DeleteThemeAsync(Guid id, CancellationToken ct = default);
        Task<Theme> GetSelectedThemeByConnectionIdAsync(Guid connectionId, CancellationToken ct = default);
        Task SetSelectedThemeAsync(Guid connectionId, Guid themeId, CancellationToken ct = default);
        Task<bool> DeleteSelectedThemeAsync(Guid connectionId, CancellationToken ct = default);
        Task<long> GetThemeUsagesCountAsync(Guid themeId, CancellationToken ct = default);
    }
}