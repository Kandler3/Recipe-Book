using System.Data.SqlTypes;
using Interfaces;
using Spectre.Console;

namespace ConsoleUI;

public class ConsoleApp(IAnsiConsole console, IRecipeService recipeService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;

    public void Run()
    {
        MainMenu mainMenu = new(Console, RecipeService);
        while (true)
        {
            mainMenu.Show();
        }
    }
}