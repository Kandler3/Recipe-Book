namespace Contracts.Interfaces;

public interface IYandexDiskService
{
    public string? OAuthToken { get; set; }
    public void OpenAuthorizationPage();
    public void SaveOAuthToken();
}