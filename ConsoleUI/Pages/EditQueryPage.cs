using ConsoleUI.Widgets;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class EditQueryPage(IAnsiConsole console, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private RecipesQuery Query { get; } = query;

    public void Show()
    {
        Console.Clear();

        var rows = Utils.GenerateQueryRows(Query);
        if (rows.Count == 0)
        {
            new MessagePage(Console, new Text("Нет параметров\n")).Show();
            return;
        }
        
        List<string> selected = Console.Prompt(
            new MultiSelectionPrompt<string>()
                .AddChoices(rows)
                .NotRequired()
        );
        
        if (selected.Count == 0)
            return;

        foreach (string selectedRow in selected)
        {
            if (selectedRow.StartsWith("Фильтр по названию"))
                Query.TitleSearchQuery = null;
            
            else if (selectedRow.StartsWith("Фильтр по категориям"))
                Query.CategoriesList.Clear();
            
            else if (selectedRow.StartsWith("Фильтр по ингредиентам"))
                Query.IngredientsList.Clear();
            
            else if (selectedRow.StartsWith("Сортировка"))
            {
                Query.SortingParameter = null;
                Query.AscendingSorting = null;
            }
        }
        
        new MessagePage(Console, new SuccessText("Параметры удалены\n")).Show();
    }
}