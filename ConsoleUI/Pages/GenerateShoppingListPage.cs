using ConsoleUI.Widgets;
using Contracts;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class GenerateShoppingListPage(
    IAnsiConsole console, 
    IRecipeService recipeService, 
    RecipesQuery query, 
    IShoppingListService shoppingListService
)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private RecipesQuery Query { get; } = query;
    private IShoppingListService ShoppingListService { get; } = shoppingListService;

    public void Show()
    {
        Console.Clear();
        var recipes = RecipeService.GetRecipes(Query);
        if (recipes.Count == 0)
        {
            new MessagePage(Console, new ErrorText("Нет рецептов\n")).Show();
            return;
        }
        
        var selectedRecipes = Console.Prompt(
            new MultiSelectionPrompt<Recipe>().AddChoices(recipes)
        );
        var shoppingList = ShoppingListService.GenerateShoppingList(selectedRecipes);
        new ShoppingListPage(Console, ShoppingListService, shoppingList).Show();
    }
}