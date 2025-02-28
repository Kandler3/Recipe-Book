using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class MainPage(IAnsiConsole console, IRecipeService recipeService, Action onReturn)
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
            new MenuOption("Выйти", OnExit)
        );

    private void OnShowRecipes()
    {
        new RecipesListPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnImport()
    {
        new ImportPage(Console, RecipeService).Show();
    }

    private void OnExport()
    {
        new ExportPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnAddFilter()
    {
        new RecipeFilterPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnAddSort()
    {
        new RecipeSortingPage(Console, RecipesQuery).Show();
    }

    private void OnAddRecipe()
    {
        new AddRecipePage(Console, RecipeService).Show();
    }

    private void OnShowRandomRecipe()
    {
        new RandomRecipePage(Console, RecipeService).Show();
    }

    private void OnExit()
    {
        if (new ConfirmPrompt(Console, "Сохранить данные перед выходом?").Ask())
            new ExportPage(Console, RecipeService, RecipesQuery).Show();
        
        OnReturn();
    }

    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}