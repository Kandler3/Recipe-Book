using System.Security.Authentication;
using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts;
using Contracts.Enums;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ImportPage(IAnsiConsole console, IRecipeService service, IYandexDiskService diskService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private IYandexDiskService DiskService { get; } = diskService;

    public void Show()
    {
        Console.Clear();
        bool local = Console.Prompt(
            new SelectionPrompt<string>().Title("Выберите источник").AddChoices("Этот компьютер", "Яндекс Диск")
        ) == "Этот компьютер";
        
        FileFormat format = new SelectFormatPrompt(Console, "Выберите формат импортируемого файла").Ask();
        string filepath = new FilepathPrompt(Console, "Введите путь до файла", local, format).Ask();

        if (!local && DiskService.OAuthToken == null)
        {
            DiskService.OpenAuthorizationPage();
            DiskService.OAuthToken =
                Console.Prompt(new TextPrompt<string>("Авторизуйтесь в браузере и введите токен: "));
        }

        try
        {
            Service.Import(filepath, format, local);
            new MessagePage(Console, new SuccessText("Рецепты загружены\n")).Show();
        }
        catch (FormatException)
        {
            new MessagePage(Console, new ErrorText("Не удалось десериализовать данные из файла\n")).Show();
        }
        catch (IOException)
        {
            new MessagePage(Console, new ErrorText("Ошибка при записи в файл\n")).Show();
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