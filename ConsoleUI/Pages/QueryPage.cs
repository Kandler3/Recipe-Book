/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница управления параметрами запроса (фильтрации и сортировки) для списка рецептов.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для работы с рецептами.</param>
/// <param name="query">Объект запроса, содержащий параметры фильтрации и сортировки.</param>
public class QueryPage(IAnsiConsole console, IRecipeService service, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private RecipesQuery Query { get; } = query;

    /// <summary>
    /// Отображает страницу управления параметрами запроса и обрабатывает пользовательский ввод.
    /// </summary>
    public void Show()
    {
        var run = true;
        while (run)
        {
            Console.Clear();
            Console.Write(new QueryPanel(Query));

            var option = Console.Prompt(
                new SelectionPrompt<MenuOption>()
                    .AddChoices(
                        new MenuOption("Выбрать параметры для удаления",
                            () => new EditQueryPage(Console, Query).Show()),
                        new MenuOption("Добавить/редактировать фильтр",
                            () => new RecipeFilterPage(Console, Service, Query).Show()),
                        new MenuOption("Добавить сортировку", () => new RecipeSortingPage(Console, Query).Show()),
                        new MenuOption("Назад", () => run = false)
                    )
            );
            option.InvokeSelect();
        }
    }
}