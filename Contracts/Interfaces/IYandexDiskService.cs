/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс для работы с сервисом Яндекс.Диск.
/// </summary>
public interface IYandexDiskService
{
    /// <summary>
    /// OAuth токен для доступа к Яндекс.Диск.
    /// </summary>
    public string? OAuthToken { get; set; }

    /// <summary>
    /// Открывает страницу авторизации для получения OAuth токена.
    /// </summary>
    public void OpenAuthorizationPage();

    /// <summary>
    /// Сохраняет OAuth токен.
    /// </summary>
    public void SaveOAuthToken();
}