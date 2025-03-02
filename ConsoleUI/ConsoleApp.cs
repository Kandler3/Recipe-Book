using ConsoleUI.Pages;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI;

public class ConsoleApp(
    IAnsiConsole console,
    IRecipeService recipeService,
    IShoppingListService shoppingListService,
    IYandexDiskService yandexDiskService,
    IGigaChatService gigaChatService
)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IShoppingListService ShoppingListService { get; } = shoppingListService;
    private IYandexDiskService YandexDiskService { get; } = yandexDiskService;
    private IGigaChatService GigaChatService { get; } = gigaChatService;

    public void Run()
    {
        var run = true;
        MainPage mainPage = new(Console, RecipeService, ShoppingListService, YandexDiskService, GigaChatService,
            () => run = false);
        while (run) mainPage.Show();
    }
}