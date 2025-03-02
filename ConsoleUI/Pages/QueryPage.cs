using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class QueryPage(IAnsiConsole console, IRecipeService service, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private RecipesQuery Query { get; } = query;

    public void Show()
    {
        var run = true;
        {
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
}