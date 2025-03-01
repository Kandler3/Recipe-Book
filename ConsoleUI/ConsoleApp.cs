using ConsoleUI.Pages;
using Contracts;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI;

public class ConsoleApp(IAnsiConsole console, IRecipeService recipeService, IShoppingListService shoppingListService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IShoppingListService ShoppingListService { get; } = shoppingListService;

    public void Run()
    {
        bool run = true;
        MainPage mainPage = new(Console, RecipeService, ShoppingListService, () => run = false);
        while (run)
        {
            mainPage.Show();
        }
    }
}