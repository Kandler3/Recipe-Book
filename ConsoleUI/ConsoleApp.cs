using ConsoleUI.Pages;
using Contracts;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI;

public class ConsoleApp(
    IAnsiConsole console, 
    IRecipeService recipeService, 
    IShoppingListService shoppingListService,
    IYandexDiskService yandexDiskService
)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IShoppingListService ShoppingListService { get; } = shoppingListService;
    private IYandexDiskService YandexDiskService { get; } = yandexDiskService;

    public void Run()
    {
        bool run = true;
        MainPage mainPage = new(Console, RecipeService, ShoppingListService, YandexDiskService, () => run = false);
        while (run)
        {
            mainPage.Show();
        }
    }
}