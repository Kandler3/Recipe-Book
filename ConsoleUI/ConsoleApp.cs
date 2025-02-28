using ConsoleUI.Pages;
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
        MainPage mainPage = new(Console, RecipeService, () => run = false);
        while (run)
        {
            mainPage.Show();
        }
    }
}