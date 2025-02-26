using Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI;

public class MainMenu(IAnsiConsole console, IRecipeService recipeService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IRecipesQuery RecipesQuery { get; } = new RecipesQuery();
    private SelectionPrompt<MenuOption> MenuSelectionPrompt  => new SelectionPrompt<MenuOption>().AddChoices(
        new MenuOption("Показать рецепты", OnShowRecipes)
        );

    private void OnShowRecipes()
    {
        IList<Recipe> recipes = RecipeService.GetRecipes(RecipesQuery);
        new RecipesTable(Console, recipes, Show).Show();
    }

    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}