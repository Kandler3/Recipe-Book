/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Prompts;
using Contracts.Enums;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница настройки сортировки рецептов.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="query">Объект запроса рецептов, содержащий параметры сортировки.</param>
public class RecipeSortingPage(IAnsiConsole console, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private RecipesQuery Query { get; } = query;

    /// <summary>
    /// Отображает страницу выбора параметра сортировки и направления.
    /// </summary>
    public void Show()
    {
        Console.Clear();

        var choice = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для сортировки")
                .AddChoices("Название", "Категория", "Сбросить", "Назад")
        );

        switch (choice)
        {
            case "Название":
                Query.SortingParameter = RecipeSortingParameter.Title;
                break;
            case "Категория":
                Query.SortingParameter = RecipeSortingParameter.Category;
                break;
            case "Сбросить":
                ShowResetPrompt();
                return;
            case "Назад":
                return;
            default:
                return;
        }

        Query.AscendingSorting = new ConfirmPrompt(Console, "Сортировать по возрастанию?").Ask();
    }

    /// <summary>
    /// Отображает подтверждение сброса параметров сортировки.
    /// </summary>
    private void ShowResetPrompt()
    {
        Console.Clear();
        if (new ConfirmPrompt(Console, "Сбросить настройки сортировки?").Ask())
            Query.ResetSorting();
    }
}