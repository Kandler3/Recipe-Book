using dotenv.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Contracts.Interfaces;

namespace Services
{
    public class YandexDiskService : IYandexDiskService
    {
        private string ClientId { get; }
        public string? OAuthToken { get; set; }
        private readonly string _dotenvFilepath;

        public YandexDiskService(string clientId, string dotenvFilepath)
        {
            ClientId = clientId;
            _dotenvFilepath = dotenvFilepath;
            if (File.Exists(dotenvFilepath))
            {
                DotEnv.Load(new DotEnvOptions(envFilePaths: [dotenvFilepath]));
                OAuthToken = Environment.GetEnvironmentVariable("OAUTH_TOKEN");
            }
        }

        public void OpenAuthorizationPage()
        {
            string url = $"https://oauth.yandex.ru/authorize?response_type=token&client_id={ClientId}";
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public void SaveOAuthToken()
        {
            if (OAuthToken == null)
                return;

            var lines = new List<string>();
            if (File.Exists(_dotenvFilepath))
            {
                lines.AddRange(File.ReadAllLines(_dotenvFilepath));
                bool found = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("OAUTH_TOKEN="))
                    {
                        lines[i] = $"OAUTH_TOKEN={OAuthToken}";
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    lines.Add($"OAUTH_TOKEN={OAuthToken}");
                }
            }
            else
            {
                lines.Add($"OAUTH_TOKEN={OAuthToken}");
            }
            File.WriteAllLines(_dotenvFilepath, lines);
        }

        public void UploadFile(string localFilepath, string cloudFilepath)
        {
            if (OAuthToken == null)
                throw new InvalidOperationException("OAuth token is not set.");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");
                string requestUrl = $"https://cloud-api.yandex.net/v1/disk/resources/upload?path={Uri.EscapeDataString(cloudFilepath)}";
                var response = client.GetAsync(requestUrl).Result;
                response.EnsureSuccessStatusCode();
                var responseContent = response.Content.ReadAsStringAsync().Result;

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                if (!root.TryGetProperty("href", out var hrefElement) || !root.TryGetProperty("method", out var methodElement))
                {
                    throw new Exception("Invalid response from Yandex Disk API: missing href or method.");
                }
                string href = hrefElement.GetString();
                string httpMethod = methodElement.GetString();

                using (var fileStream = File.OpenRead(localFilepath))
                {
                    var request = new HttpRequestMessage(new HttpMethod(httpMethod), href)
                    {
                        Content = new StreamContent(fileStream)
                    };
                    var uploadResponse = client.SendAsync(request).Result;
                    uploadResponse.EnsureSuccessStatusCode();
                }
            }
        }

        public void DownloadFile(string localFilepath, string cloudFilepath)
        {
            if (OAuthToken == null)
                throw new InvalidOperationException("OAuth token is not set.");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");
                string requestUrl = $"https://cloud-api.yandex.net/v1/disk/resources/download?path={Uri.EscapeDataString(cloudFilepath)}";
                var response = client.GetAsync(requestUrl).Result;
                response.EnsureSuccessStatusCode();
                var responseContent = response.Content.ReadAsStringAsync().Result;

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                if (!root.TryGetProperty("href", out var hrefElement) || !root.TryGetProperty("method", out var methodElement))
                {
                    throw new Exception("Invalid response from Yandex Disk API: missing href or method.");
                }
                string href = hrefElement.GetString();
                string httpMethod = methodElement.GetString();

                var downloadRequest = new HttpRequestMessage(new HttpMethod(httpMethod), href);
                downloadRequest.Headers.Add("Authorization", $"OAuth {OAuthToken}");
                var downloadResponse = client.SendAsync(downloadRequest).Result;
                downloadResponse.EnsureSuccessStatusCode();

                using (var responseStream = downloadResponse.Content.ReadAsStreamAsync().Result)
                using (var fileStream = File.Create(localFilepath))
                {
                    responseStream.CopyTo(fileStream);
                }
            }
        }
    }
}
