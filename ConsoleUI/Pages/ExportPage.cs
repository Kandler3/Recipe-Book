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
/// Страница экспорта рецептов в локальный файл или на Яндекс.Диск.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для работы с рецептами.</param>
/// <param name="query">Параметры запроса для фильтрации экспортируемых рецептов.</param>
/// <param name="diskService">Сервис для работы с Яндекс.Диском.</param>
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

    /// <summary>
    /// Отображает страницу экспорта и выполняет процесс сохранения рецептов.
    /// </summary>
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
            new MessagePage(Console, new ErrorText("Ошибка авторизации на Яндекс Диске\n")).Show();
        }
        catch (InvalidOperationException) when (!local)
        {
            new MessagePage(Console, new ErrorText("Ошибка при работе с Яндекс Диском\n")).Show();
        }
    }
}
