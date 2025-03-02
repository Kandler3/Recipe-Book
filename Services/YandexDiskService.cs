/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using Contracts.Interfaces;
using dotenv.net;

namespace Services;

/// <summary>
/// Сервис для работы с Яндекс.Диск.
/// </summary>
public class YandexDiskService : IYandexDiskService
{
    private const string AuthorizeUrl = "https://oauth.yandex.ru/authorize";
    private const string ApiUrl = "https://cloud-api.yandex.net/v1/disk/resources";

    /// <summary>
    /// Конструктор сервиса.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента для авторизации.</param>
    /// <param name="dotenvFilepath">Путь к файлу с переменными окружения.</param>
    public YandexDiskService(string clientId, string dotenvFilepath)
    {
        ClientId = clientId;
        DotenvFilepath = dotenvFilepath;
        if (File.Exists(DotenvFilepath))
        {
            DotEnv.Load(new DotEnvOptions(envFilePaths: [DotenvFilepath]));
            OAuthToken = Environment.GetEnvironmentVariable("YANDEX_DISK_OAUTH_TOKEN");
        }
    }

    private string ClientId { get; }
    private string DotenvFilepath { get; }

    public string? OAuthToken { get; set; }

    /// <summary>
    /// Открывает страницу авторизации в браузере.
    /// </summary>
    public void OpenAuthorizationPage()
    {
        var url = $"{AuthorizeUrl}?response_type=token&client_id={ClientId}";
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    /// <summary>
    /// Сохраняет OAuth токен в файл окружения.
    /// </summary>
    public void SaveOAuthToken()
    {
        if (OAuthToken == null)
            return;

        List<string> lines = [];

        if (File.Exists(DotenvFilepath))
        {
            lines.AddRange(File.ReadAllLines(DotenvFilepath));
            var found = false;

            for (var i = 0; i < lines.Count; i++)
                if (lines[i].StartsWith("YANDEX_DISK_OAUTH_TOKEN="))
                {
                    lines[i] = $"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}";
                    found = true;
                    break;
                }

            if (!found) lines.Add($"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}");
        }
        else
        {
            lines.Add($"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}");
        }

        File.WriteAllLines(DotenvFilepath, lines);
    }

    /// <summary>
    /// Загружает файл на Яндекс.Диск.
    /// </summary>
    /// <param name="localFilepath">Локальный путь к файлу.</param>
    /// <param name="cloudFilepath">Путь на Яндекс.Диск для сохранения файла.</param>
    /// <exception cref="AuthenticationException">Выбрасывается, если OAuth токен отсутствует.</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при ошибке работы с API Яндекс.Диск.</exception>
    public void UploadFile(string localFilepath, string cloudFilepath)
    {
        if (OAuthToken == null)
            throw new AuthenticationException("No OAuth token");

        try
        {
            GetLoadUrl("upload", cloudFilepath, out var href, out var method);
            UploadFile(localFilepath, href, method);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or JsonException or IOException or KeyNotFoundException or AggregateException
        )
        {
            throw new InvalidOperationException("Yandex Disk exception", ex);
        }
    }

    /// <summary>
    /// Скачивает файл с Яндекс.Диск.
    /// </summary>
    /// <param name="localFilepath">Локальный путь для сохранения файла.</param>
    /// <param name="cloudFilepath">Путь к файлу на Яндекс.Диск.</param>
    /// <exception cref="AuthenticationException">Выбрасывается, если OAuth токен отсутствует.</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при ошибке работы с API Яндекс.Диск.</exception>
    public void DownloadFile(string localFilepath, string cloudFilepath)
    {
        if (OAuthToken == null)
            throw new AuthenticationException("No OAuth token");

        try
        {
            GetLoadUrl("download", cloudFilepath, out var href, out var httpMethod);
            DownloadFile(localFilepath, href, httpMethod);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or JsonException or IOException or KeyNotFoundException or AggregateException
        )
        {
            throw new InvalidOperationException("Yandex Disk exception", ex);
        }
    }

    /// <summary>
    /// Получает URL для загрузки или скачивания файла с Яндекс.Диск.
    /// </summary>
    /// <param name="action">Действие ("upload" или "download").</param>
    /// <param name="filepath">Путь к файлу на Яндекс.Диск.</param>
    /// <param name="url">Выходной URL для загрузки/скачивания.</param>
    /// <param name="method">HTTP метод для запроса.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если задано некорректное действие.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если в ответе отсутствуют необходимые данные.</exception>
    private void GetLoadUrl(string action, string filepath, out string url, out string method)
    {
        if (action != "upload" && action != "download")
            throw new ArgumentException("Invalid action");

        string responseContent;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

            var requestUrl = $"{ApiUrl}/{action}?path={Uri.EscapeDataString(filepath)}";
            var response = client.GetAsync(requestUrl).Result;

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new AuthenticationException("Invalid OAuth token");

            response.EnsureSuccessStatusCode();
            responseContent = response.Content.ReadAsStringAsync().Result;
        }

        using var jsonDoc = JsonDocument.Parse(responseContent);
        var root = jsonDoc.RootElement;

        if (
            !root.TryGetProperty("href", out var hrefElement)
            || !root.TryGetProperty("method", out var methodElement)
        )
            throw new KeyNotFoundException("Invalid response from Yandex Disk API: missing href or method.");

        var href = hrefElement.GetString();
        var httpMethod = methodElement.GetString();

        if (href == null || httpMethod == null)
            throw new KeyNotFoundException("Invalid response from Yandex Disk API: href or method are null");

        url = href;
        method = httpMethod;
    }

    /// <summary>
    /// Скачивает файл по заданному URL.
    /// </summary>
    /// <param name="localFilepath">Локальный путь для сохранения файла.</param>
    /// <param name="href">URL для скачивания файла.</param>
    /// <param name="httpMethod">HTTP метод для запроса.</param>
    private void DownloadFile(string localFilepath, string href, string httpMethod)
    {
        using var client = new HttpClient();

        var downloadRequest = new HttpRequestMessage(new HttpMethod(httpMethod), href);
        downloadRequest.Headers.Add("Authorization", $"OAuth {OAuthToken}");

        var downloadResponse = client.SendAsync(downloadRequest).Result;

        if (downloadResponse.StatusCode == HttpStatusCode.Unauthorized)
            throw new AuthenticationException("Invalid OAuth token");

        downloadResponse.EnsureSuccessStatusCode();

        using var responseStream = downloadResponse.Content.ReadAsStreamAsync().Result;
        using var fileStream = File.Create(localFilepath);
        responseStream.CopyTo(fileStream);
    }

    /// <summary>
    /// Загружает файл по заданному URL.
    /// </summary>
    /// <param name="localFilepath">Локальный путь к файлу для загрузки.</param>
    /// <param name="href">URL для загрузки файла.</param>
    /// <param name="httpMethod">HTTP метод для запроса.</param>
    private void UploadFile(string localFilepath, string href, string httpMethod)
    {
        using var client = new HttpClient();
        using var fileStream = File.OpenRead(localFilepath);
        var request = new HttpRequestMessage(new HttpMethod(httpMethod), href)
        {
            Content = new StreamContent(fileStream)
        };
        var uploadResponse = client.SendAsync(request).Result;
        if (uploadResponse.StatusCode == HttpStatusCode.Unauthorized)
            throw new AuthenticationException("Invalid OAuth token");

        uploadResponse.EnsureSuccessStatusCode();
    }
}
