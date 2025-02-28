using System.Data.SqlTypes;
using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class ConsoleApp(IAnsiConsole console, IRecipeService recipeService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;

    public void Run()
    {
        bool run = true;
        MainMenu mainMenu = new(Console, RecipeService, () => run = false);
        while (run)
        {
            mainMenu.Show();
        }
    }
}