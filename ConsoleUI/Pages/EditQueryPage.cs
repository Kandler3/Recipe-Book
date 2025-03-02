/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница редактирования параметров запроса рецептов.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="query">Объект запроса рецептов, параметры которого редактируются.</param>
public class EditQueryPage(IAnsiConsole console, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private RecipesQuery Query { get; } = query;

    /// <summary>
    /// Отображает страницу редактирования параметров запроса и выполняет удаление выбранных параметров.
    /// </summary>
    public void Show()
    {
        Console.Clear();

        var rows = Utils.GenerateQueryRows(Query);
        if (rows.Count == 0)
        {
            new MessagePage(Console, new Text("Нет параметров\n")).Show();
            return;
        }

        var selected = Console.Prompt(
            new MultiSelectionPrompt<string>()
                .AddChoices(rows)
                .NotRequired()
        );

        if (selected.Count == 0)
            return;

        foreach (var selectedRow in selected)
            if (selectedRow.StartsWith("Фильтр по названию"))
            {
                Query.TitleSearchQuery = null;
            }
            else if (selectedRow.StartsWith("Фильтр по категориям"))
            {
                Query.CategoriesList.Clear();
            }
            else if (selectedRow.StartsWith("Фильтр по ингредиентам"))
            {
                Query.IngredientsList.Clear();
            }
            else if (selectedRow.StartsWith("Сортировка"))
            {
                Query.SortingParameter = null;
                Query.AscendingSorting = null;
            }

        new MessagePage(Console, new SuccessText("Параметры удалены\n")).Show();
    }
}