using Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI;

public class MainMenu(IAnsiConsole console, IRecipeService recipeService, Action onReturn)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IRecipesQuery RecipesQuery { get; } = new RecipesQuery();
    private Action OnReturn { get; } = onReturn;

    private SelectionPrompt<MenuOption> MenuSelectionPrompt => new SelectionPrompt<MenuOption>()
        .PageSize(10)
        .AddChoices(
            new MenuOption("Импортировать рецепты", () => { }),
            new MenuOption("Экспортировать рецепты", () => { }),
            new MenuOption("Показать рецепты", OnShowRecipes),
            new MenuOption("Добавить фильтр", () => { }),
            new MenuOption("Добавить сортировку", () => { }),
            new MenuOption("Добавить рецепт", () => { }),
            new MenuOption("Выйти", OnReturn)
        );

    private void OnShowRecipes()
    {
        new RecipesTable(Console, RecipeService, RecipesQuery, Show).Show();
    }

    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}