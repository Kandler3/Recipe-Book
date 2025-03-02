using System.Security.Authentication;
using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ExportPage(
    IAnsiConsole console,
    IRecipeService service,
    RecipesQuery query,
    IYandexDiskService diskService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private RecipesQuery Query { get; } = query;
    private IYandexDiskService DiskService { get; } = diskService;

    public void Show()
    {
        Console.Clear();
        var local = Console.Prompt(
            new SelectionPrompt<string>().Title("Сохранить на").AddChoices("Этот компьютер", "Яндекс Диск")
        ) == "Этот компьютер";

        var format = new SelectFormatPrompt(Console, "Выберите формат экспорта").Ask();
        var filepath = new FilepathPrompt(Console, "Введите путь до файла", false, format).Ask();

        if (filepath == null) return;

        if (
            local
            && File.Exists(filepath)
            && !new ConfirmPrompt(Console, "Файл уже существует. Перезаписать?").Ask()
        ) return;

        if (!local && DiskService.OAuthToken == null)
        {
            DiskService.OpenAuthorizationPage();
            DiskService.OAuthToken =
                Console.Prompt(new TextPrompt<string>("Авторизуйтесь в браузере и введите токен: "));
        }

        try
        {
            Service.Export(filepath, format, Query, local);
            new MessagePage(Console, new SuccessText("Файл сохранен\n")).Show();
        }
        catch (UnauthorizedAccessException)
        {
            new MessagePage(Console, new ErrorText("Нет доступа к файлу\n")).Show();
        }
        catch (IOException)
        {
            new MessagePage(Console, new ErrorText("Ошибка при чтении файла\n")).Show();
        }
        catch (AuthenticationException) when (!local)
        {
            DiskService.OAuthToken = null;
            new MessagePage(Console, new ErrorText("Ошибка авторизации я Яндекс Диск\n")).Show();
        }
        catch (InvalidOperationException) when (!local)
        {
            new MessagePage(Console, new ErrorText("Ошибка при работе с Яндекс Диском\n")).Show();
        }
    }
}