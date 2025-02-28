using ConsoleUI.Prompts;
using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class RecipeSortingPrompt(IAnsiConsole console, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private RecipesQuery Query { get; } = query;

    public void Show()
    {
        Console.Clear();
        
        string field = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для сортировки")
                .AddChoices("Название", "Категория")
        );
        
        Query.SortingParameter = field switch
        {
            "Название" => RecipeSortingParameter.Title,
            "Категория" => RecipeSortingParameter.Category,
            _ => throw new ArgumentException(),
        };

        Query.AscendingSorting = new ConfirmPrompt(Console, "Сортировать по возрастанию?").Ask();
    }
}