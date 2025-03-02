/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

/// <summary>
/// Таблица для отображения списка рецептов с постраничной навигацией.
/// </summary>
public class RecipesTable : IRenderable
{
    /// <summary>
    /// Создает таблицу с рецептами.
    /// </summary>
    /// <param name="recipes">Список рецептов для отображения.</param>
    /// <param name="selected">Индекс выбранного рецепта.</param>
    /// <param name="page">Номер текущей страницы (начиная с 0).</param>
    /// <param name="maxPage">Общее количество страниц.</param>
    public RecipesTable(IList<Recipe> recipes, int selected, int page, int maxPage)
    {
        Recipes = recipes;
        Selected = selected;
        Page = page;
        MaxPage = maxPage;
        Table = BuildTable();
    }

    private IList<Recipe> Recipes { get; }
    private int Selected { get; }
    private int Page { get; }
    private int MaxPage { get; }
    private Table Table { get; }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)Table).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)Table).Render(options, maxWidth);
    }

    /// <summary>
    /// Собирает таблицу с рецептами и навигационной информацией.
    /// </summary>
    /// <returns>Построенная таблица.</returns>
    private Table BuildTable()
    {
        Table table = new();

        table.AddColumn("[bold]Название[/]");
        table.AddColumn("[bold]Категория[/]");

        table.Caption =
            new TableTitle(
                $"Страница {Page + 1}/{MaxPage}\n"
                + "\u2191/\u2193: выбрать рецепт\n"
                + "\u2190/\u2192: перелистнуть страницу\n"
                + "Enter: посмотреть рецепт\n"
                + "Del: удалить рецепт\n"
                + "Backspace: назад"
            );

        for (var i = 0; i < Recipes.Count; i++) 
            table.AddRow(Utils.GetRowFromRecipe(Recipes[i], i == Selected));

        return table;
    }
}
