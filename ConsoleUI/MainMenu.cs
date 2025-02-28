using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class MainMenu(IAnsiConsole console, IRecipeService recipeService, Action onReturn)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private RecipesQuery RecipesQuery { get; } = new RecipesQuery();
    private Action OnReturn { get; } = onReturn;

    private SelectionPrompt<MenuOption> MenuSelectionPrompt => new SelectionPrompt<MenuOption>()
        .PageSize(10)
        .AddChoices(
            new MenuOption("Импортировать рецепты", OnImport),
            new MenuOption("Экспортировать рецепты", OnExport),
            new MenuOption("Показать рецепты", OnShowRecipes),
            new MenuOption("Фильтр", OnAddFilter),
            new MenuOption("Сортировка", OnAddSort),
            new MenuOption("Добавить рецепт", OnAddRecipe),
            new MenuOption("Рецепт дня", OnShowRandomRecipe),
            new MenuOption("Выйти", OnReturn)
        );

    private void OnShowRecipes()
    {
        new RecipesListPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnImport()
    {
        new ImportMenu(Console, RecipeService).Show();
    }

    private void OnExport()
    {
        new ExportMenu(Console, RecipeService).Show();
    }

    private void OnAddFilter()
    {
        new RecipeFilterPrompt(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnAddSort()
    {
        new RecipeSortingPrompt(Console, RecipesQuery).Show();
    }

    private void OnAddRecipe()
    {
        
    }

    private void OnShowRandomRecipe()
    {
        new RandomRecipePage(Console, RecipeService).Show();
    }

    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}