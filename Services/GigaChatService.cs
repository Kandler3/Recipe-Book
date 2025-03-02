/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Contracts.Interfaces;
using Models;

namespace Services;

/// <summary>
/// Сервис для генерации рецептов с использованием GigaChat.
/// </summary>
/// <param name="authorizationKey">Ключ авторизации для получения токена доступа.</param>
/// <param name="recipeSerializer">Сериализатор для десериализации рецепта из JSON.</param>
public class GigaChatService(string authorizationKey, ISingleRecipeSerializer recipeSerializer) : IGigaChatService
{
    private const string GetAccessTokenUrl = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
    private const string PromptUrl = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
    private string AuthorizationKey { get; } = authorizationKey;
    private ISingleRecipeSerializer RecipeSerializer { get; } = recipeSerializer;

    private string? AccessToken { get; set; }

    // Позволяет игнорировать отсутствие у сбера ssl сертификатов
    private static HttpClientHandler IgnoreCertificateIssuesHandler => new()
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };
    
    /// <summary>
    /// Системный промпт.
    /// </summary>
    private static string SystemMessage =>
        "Сгенерируй рецепт, который соответствует следующей JSON-схеме. " +
        "Вывод должен содержать только валидный JSON-объект, без каких-либо дополнительных комментариев или пояснений. " +
        "Объект должен содержать только поля, указанные в схеме, с правильными именами (с заглавной буквы), " +
        "и все данные должны быть валидны согласно описанным типам.\n\n" +
        "{\n" +
        "  \"$schema\": \"http://json-schema.org/draft-07/schema#\",\n" +
        "  \"title\": \"Recipe\",\n" +
        "  \"type\": \"object\",\n" +
        "  \"properties\": {\n" +
        "    \"Title\": { \"type\": \"string\" },\n" +
        "    \"Category\": { \"type\": [\"string\", \"null\"] },\n" +
        "    \"Ingredients\": {\n" +
        "      \"type\": \"array\",\n" +
        "      \"items\": {\n" +
        "        \"type\": \"object\",\n" +
        "        \"properties\": {\n" +
        "          \"Name\": { \"type\": \"string\" },\n" +
        "          \"Quantity\": { \"type\": [\"integer\", \"null\"] },\n" +
        "          \"Measurement\": { \"type\": [\"string\", \"null\"] }\n" +
        "        },\n" +
        "        \"required\": [\"Name\"],\n" +
        "        \"additionalProperties\": false\n" +
        "      }\n" +
        "    },\n" +
        "    \"Instructions\": {\n" +
        "      \"type\": \"array\",\n" +
        "      \"items\": { \"type\": \"string\" }\n" +
        "    }\n" +
        "  },\n" +
        "  \"required\": [\"Title\"],\n" +
        "  \"additionalProperties\": false\n" +
        "}\n\n" +
        "Пожалуйста, сгенерируй рецепт в виде JSON, строго соответствующий указанной схеме.";

    /// <summary>
    /// Генерирует рецепт на основе заданного запроса.
    /// </summary>
    /// <param name="prompt">Текст запроса для генерации рецепта.</param>
    /// <returns>Сгенерированный рецепт.</returns>
    public Recipe GenerateRecipe(string prompt)
    {
        var recipeString = GenerateRecipeString(prompt);
        try
        {
            return RecipeSerializer.DeserializeRecipe(recipeString);
        }
        catch (FormatException)
        {
            recipeString = GenerateRecipeString(prompt, recipeString);
            return RecipeSerializer.DeserializeRecipe(recipeString);
        }
    }

    /// <summary>
    /// Обновляет токен доступа, отправляя запрос на получение OAuth токена.
    /// </summary>
    /// <exception cref="AuthenticationException">
    /// Выбрасывается, если в ответе отсутствует токен доступа.
    /// </exception>
    private void RefreshAccessToken()
    {
        using var httpClient = new HttpClient(IgnoreCertificateIssuesHandler);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {AuthorizationKey}");
        httpClient.DefaultRequestHeaders.Add("RqUID", new Guid().ToString());

        var request = new HttpRequestMessage(HttpMethod.Post, GetAccessTokenUrl)
        {
            Content = new FormUrlEncodedContent([new KeyValuePair<string, string?>("scope", "GIGACHAT_API_PERS")])
        };

        var response = httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        var content = response.Content.ReadAsStringAsync().Result;

        using var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        if (!root.TryGetProperty("access_token", out var accessTokenElement))
            throw new AuthenticationException("No access token in response");

        var accessToken = accessTokenElement.GetString();

        AccessToken = accessToken ?? throw new AuthenticationException("No access token in response");
    }

    /// <summary>
    /// Формирует тело запроса для первого запроса к API GigaChat.
    /// </summary>
    /// <param name="prompt">Текст запроса от пользователя.</param>
    /// <returns>Контент запроса в формате JSON.</returns>
    private static StringContent GetFirstRequestBody(string prompt)
    {
        var jsonObject = new JsonObject
        {
            { "model", "GigaChat" },
            {
                "messages", new JsonArray
                {
                    new JsonObject
                    {
                        { "role", "system" },
                        { "content", SystemMessage }
                    },
                    new JsonObject
                    {
                        { "role", "user" },
                        { "content", prompt }
                    }
                }
            }
        };

        var json = jsonObject.ToString();

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Формирует тело запроса для второго запроса к API GigaChat с исправлением формата.
    /// </summary>
    /// <param name="prompt">Текст запроса от пользователя.</param>
    /// <param name="firstReqContent">Содержимое ответа первого запроса.</param>
    /// <returns>Контент запроса в формате JSON.</returns>
    private static StringContent GetSecondRequestBody(string prompt, string firstReqContent)
    {
        var jsonObject = new JsonObject
        {
            { "model", "GigaChat" },
            {
                "messages", new JsonArray
                {
                    new JsonObject
                    {
                        { "role", "system" },
                        { "content", SystemMessage }
                    },
                    new JsonObject
                    {
                        { "role", "user" },
                        { "content", prompt }
                    },
                    new JsonObject
                    {
                        { "role", "assistant" },
                        { "content", firstReqContent }
                    },
                    new JsonObject
                    {
                        { "role", "user" },
                        {
                            "content",
                            "проверь свой ответ на строгое!!! соответствие формату, исправь ошибки и отправь рецепт заново"
                        }
                    }
                }
            }
        };

        // Преобразуем JSON-объект в строку
        var json = jsonObject.ToString();

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Отправляет запрос к API GigaChat с заданным запросом и, при необходимости, содержимым первого запроса.
    /// </summary>
    /// <param name="prompt">Текст запроса от пользователя.</param>
    /// <param name="firstReqContent">Содержимое ответа первого запроса (опционально).</param>
    /// <returns>Ответ HTTP-сервера.</returns>
    private HttpResponseMessage MakePromptRequest(string prompt, string? firstReqContent = null)
    {
        using var client = new HttpClient(IgnoreCertificateIssuesHandler);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

        var content = firstReqContent == null
            ? GetFirstRequestBody(prompt)
            : GetSecondRequestBody(prompt, firstReqContent);
        var request = new HttpRequestMessage(HttpMethod.Post, PromptUrl) { Content = content };
        return client.SendAsync(request).Result;
    }

    /// <summary>
    /// Генерирует строку рецепта, отправляя запросы к API GigaChat.
    /// </summary>
    /// <param name="prompt">Текст запроса от пользователя.</param>
    /// <param name="firstReqContent">Содержимое первого запроса (опционально).</param>
    /// <returns>Строка, содержащая JSON-представление рецепта.</returns>
    /// <exception cref="HttpRequestException">
    /// Выбрасывается, если в ответе отсутствует корректная строка рецепта.
    /// </exception>
    private string GenerateRecipeString(string prompt, string? firstReqContent = null)
    {
        var response = MakePromptRequest(prompt, firstReqContent);
        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            RefreshAccessToken();
            response = MakePromptRequest(prompt);
        }

        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().Result;
        using var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        try
        {
            var choices = root.GetProperty("choices").EnumerateArray();
            var choice = choices.First();
            var message = choice.GetProperty("message").GetProperty("content").GetString();
            return message ?? throw new KeyNotFoundException("No recipe string in response");
        }
        catch (Exception e) when (e is KeyNotFoundException or ArgumentException or FormatException)
        {
            throw new HttpRequestException($"No recipe string in response: {e.Message}");
        }
    }
}
