using AIChatWebServer.Models.Exceptions.Implementations.File;
using AIChatWebServer.Models.Exceptions.Implementations.Themes;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Themes;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Users;
using System.Text.Json;

namespace AIChatWebServer.Services.Implementations.Users
{
    public sealed class ThemeService(
        IThemeRepository themeRepository,
        ILogger<ThemeService> logger) : IThemeService
    {
        private readonly IThemeRepository _themeRepository =
            themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
        private readonly ILogger<ThemeService> _logger =
            logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Theme> GetThemeByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetByIdAsync(id, ct)
                ?? throw new ThemeNotFoundException(id);

            _logger.LogInformation(
                "Retrieved theme {ThemeId} successfully.",
                id);

            return theme;
        }

        public async Task<Theme> GetThemeByNameAsync(
            Guid userId,
            string name,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetByNameAsync(userId, name, ct)
                ?? throw new ThemeNotFoundException(name, userId);

            _logger.LogInformation(
                "Retrieved theme '{ThemeName}' for user {UserId} successfully.",
                name,
                userId);

            return theme;
        }

        public async Task<IReadOnlyList<Theme>> GetThemesByUserIdAsync(
            Guid? userId,
            CancellationToken ct = default)
        {
            var themes = await _themeRepository.GetByUserIdAsync(userId, ct);

            _logger.LogInformation(
                "Retrieved {Count} themes for user {UserId}.",
                themes.Count,
                userId ?? Guid.Empty);

            return themes;
        }

        public async Task<IReadOnlyList<Theme>> GetThemesByTypeAsync(
            ThemeType type,
            CancellationToken ct = default)
        {
            var themes = await _themeRepository.GetByTypeAsync(type, ct);

            _logger.LogInformation(
                "Retrieved {Count} themes of type {ThemeType}.",
                themes.Count,
                type);

            return themes;
        }

        public async Task<IReadOnlyList<Theme>> GetAllThemesAsync(
            CancellationToken ct = default)
        {
            var themes = await _themeRepository.GetAllAsync(ct);

            _logger.LogInformation(
                "Retrieved {Count} total themes.",
                themes.Count);

            return themes;
        }

        public async Task<Guid> CreateThemeAsync(
            Guid userId,
            string name,
            ThemeType type,
            JsonDocument content,
            CancellationToken ct = default)
        {
            var existingTheme = await _themeRepository.GetByNameAsync(userId, name, ct);
            if (existingTheme != null)
            {
                throw new ThemeAlreadyExistsException(name, userId);
            }

            var themeId = await _themeRepository.CreateAsync(userId, name, type, content, ct);

            _logger.LogInformation(
                "Created theme {ThemeId} '{ThemeName}' of type {ThemeType} for user {UserId}.",
                themeId,
                name,
                type,
                userId);

            return themeId;
        }

        public async Task<Theme> UpdateThemeAsync(
            Theme theme,
            CancellationToken ct = default)
        {
            var existingTheme = await _themeRepository.GetByIdAsync(theme.Id, ct)
                ?? throw new ThemeNotFoundException(theme.Id);

            if (existingTheme.UserId.HasValue && theme.Name != existingTheme.Name)
            {
                var conflictingTheme = await _themeRepository.GetByNameAsync(
                    existingTheme.UserId.Value,
                    theme.Name,
                    ct);

                if (conflictingTheme != null && conflictingTheme.Id != theme.Id)
                {
                    throw new ThemeAlreadyExistsException(theme.Name, existingTheme.UserId.Value);
                }
            }

            theme.UpdatedAt = DateTime.UtcNow;

            var updatedTheme = await _themeRepository.UpdateAsync(theme, ct);

            _logger.LogInformation(
                "Updated theme {ThemeId} successfully.",
                theme.Id);

            return updatedTheme;
        }

        public async Task DeleteThemeAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetByIdAsync(id, ct)
                ?? throw new ThemeNotFoundException(id);

            var usagesCount = await _themeRepository.GetUsagesCountAsync(id, ct);
            if (usagesCount > 0)
            {
                throw new ThemeDeleteException(id, $"Theme is currently used by {usagesCount} connections");
            }

            var result = await _themeRepository.DeleteAsync(id, ct);

            if (!result)
            {
                throw new ThemeDeleteException(id);
            }

            _logger.LogInformation(
                "Deleted theme {ThemeId} successfully.",
                id);
        }

        public async Task<Theme> GetSelectedThemeByConnectionIdAsync(
            Guid connectionId,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetSelectedThemeByConnectionIdAsync(connectionId, ct)
                ?? throw new SelectedThemeNotFoundException(connectionId);

            _logger.LogInformation(
                "Retrieved selected theme {ThemeId} for connection {ConnectionId}.",
                theme.Id,
                connectionId);

            return theme;
        }

        public async Task SetSelectedThemeAsync(
            Guid connectionId,
            Guid themeId,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetByIdAsync(themeId, ct)
                ?? throw new ThemeNotFoundException(themeId);

            await _themeRepository.UpsertSelectedThemeAsync(connectionId, themeId, ct);

            _logger.LogInformation(
                "Set theme {ThemeId} as selected for connection {ConnectionId}.",
                themeId,
                connectionId);
        }

        public async Task<bool> DeleteSelectedThemeAsync(
            Guid connectionId,
            CancellationToken ct = default)
        {
            var result = await _themeRepository.DeleteSelectedThemeAsync(connectionId, ct);

            if (result)
            {
                _logger.LogInformation(
                    "Deleted selected theme for connection {ConnectionId}.",
                    connectionId);
            }
            else
            {
                _logger.LogWarning(
                    "No selected theme found for connection {ConnectionId} to delete.",
                    connectionId);
            }

            return result;
        }

        public async Task<long> GetThemeUsagesCountAsync(
            Guid themeId,
            CancellationToken ct = default)
        {
            var theme = await _themeRepository.GetByIdAsync(themeId, ct)
                ?? throw new ThemeNotFoundException(themeId);

            var usagesCount = await _themeRepository.GetUsagesCountAsync(themeId, ct);

            _logger.LogInformation(
                "Theme {ThemeId} is used by {UsagesCount} connections.",
                themeId,
                usagesCount);

            return usagesCount;
        }
    }
}