/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Security.Authentication;
using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница импорта рецептов из локального файла или с Яндекс.Диска.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для управления рецептами.</param>
/// <param name="diskService">Сервис для работы с Яндекс.Диском.</param>
public class ImportPage(IAnsiConsole console, IRecipeService service, IYandexDiskService diskService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private IYandexDiskService DiskService { get; } = diskService;

    /// <summary>
    /// Отображает страницу импорта и выполняет процесс загрузки рецептов.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        var local = Console.Prompt(
            new SelectionPrompt<string>().Title("Выберите источник").AddChoices("Этот компьютер", "Яндекс Диск")
        ) == "Этот компьютер";

        var format = new SelectFormatPrompt(Console, "Выберите формат импортируемого файла").Ask();
        var filepath = new FilepathPrompt(Console, "Введите путь до файла", local, format).Ask();

        if (filepath == null) return;

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
        catch (UnauthorizedAccessException)
        {
            new MessagePage(Console, new ErrorText("Нет доступа к файлу\n")).Show();
        }
        catch (IOException)
        {
            new MessagePage(Console, new ErrorText("Ошибка при записи в файл\n")).Show();
        }
        catch (AuthenticationException) when (!local)
        {
            DiskService.OAuthToken = null;
            new MessagePage(Console, new ErrorText("Ошибка авторизации на Яндекс Диск\n")).Show();
        }
        catch (InvalidOperationException) when (!local)
        {
            new MessagePage(Console, new ErrorText("Ошибка при работе с Яндекс Диском\n")).Show();
        }
    }
}
