using dotenv.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using Contracts.Interfaces;

namespace Services
{
    public class YandexDiskService : IYandexDiskService
    {
        private const string AuthorizeUrl = "https://oauth.yandex.ru/authorize";
        private const string ApiUrl = "https://cloud-api.yandex.net/v1/disk/resources";
        private string ClientId { get; }
        public string? OAuthToken { get; set; }
        private string DotenvFilepath { get; }

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

        public void OpenAuthorizationPage()
        {
            string url = $"{AuthorizeUrl}?response_type=token&client_id={ClientId}";
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

            List<string> lines = [];
            
            if (File.Exists(DotenvFilepath))
            {
                lines.AddRange(File.ReadAllLines(DotenvFilepath));
                bool found = false;
                
                for (int i = 0; i < lines.Count; i++)
                    if (lines[i].StartsWith("YANDEX_DISK_OAUTH_TOKEN="))
                    {
                        lines[i] = $"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}";
                        found = true;
                        break;
                    }
                
                if (!found)
                {
                    lines.Add($"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}");
                }
            }
            else
            {
                lines.Add($"YANDEX_DISK_OAUTH_TOKEN={OAuthToken}");
            }
            File.WriteAllLines(DotenvFilepath, lines);
        }

        public void UploadFile(string localFilepath, string cloudFilepath)
        {
            if (OAuthToken == null)
                throw new AuthenticationException("No OAuth token");

            try
            {
                GetLoadUrl("upload", cloudFilepath, out string href, out string method);
                UploadFile(localFilepath, href, method);
            }
            catch (Exception ex) when (
                ex is HttpRequestException or JsonException or IOException or KeyNotFoundException or AggregateException
            )
            {
                throw new InvalidOperationException("Yandex Disk exception", ex);
            }
        }

        public void DownloadFile(string localFilepath, string cloudFilepath)
        {
            if (OAuthToken == null)
                throw new AuthenticationException("No OAuth token");

            try
            {
                GetLoadUrl("download",cloudFilepath, out string href, out string httpMethod);
                DownloadFile(localFilepath, href, httpMethod);
            }
            catch (Exception ex) when (
                ex is HttpRequestException or JsonException or IOException or KeyNotFoundException or AggregateException
                )
            {
                throw new InvalidOperationException("Yandex Disk exception", ex);
            }
        }

        private void GetLoadUrl(string action, string filepath, out string url, out string method)
        {
            if (action != "upload" && action != "download")
                throw new ArgumentException("Invalid action");
                
            string responseContent;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

                string requestUrl = $"{ApiUrl}/{action}?path={Uri.EscapeDataString(filepath)}";
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
            
            string? href = hrefElement.GetString();
            string? httpMethod = methodElement.GetString();
            
            if (href == null || httpMethod == null)
                throw new KeyNotFoundException("Invalid response from Yandex Disk API: href or method are null");
            
            url = href;
            method = httpMethod;
        }

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
}