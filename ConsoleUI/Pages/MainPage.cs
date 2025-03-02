using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class MainPage(
    IAnsiConsole console, 
    IRecipeService recipeService, 
    IShoppingListService shoppingListService, 
    IYandexDiskService yandexDiskService,
    IGigaChatService gigaChatService,
    Action onReturn
)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private RecipesQuery RecipesQuery { get; } = new();
    private IShoppingListService ShoppingListService { get; } = shoppingListService;
    private IYandexDiskService YandexDiskService { get; } = yandexDiskService;
    private IGigaChatService GigaChatService { get; } = gigaChatService;
    private Action OnReturn { get; } = onReturn;

    private SelectionPrompt<MenuOption> MenuSelectionPrompt => new SelectionPrompt<MenuOption>()
        .PageSize(10)
        .AddChoices(
            new MenuOption("Импортировать рецепты", OnImport),
            new MenuOption("Экспортировать рецепты", OnExport),
            new MenuOption("Показать рецепты", OnShowRecipes),
            new MenuOption("Фильтр и сортировка", OnFilterAndSorting),
            new MenuOption("Добавить рецепт", OnAddRecipe),
            new MenuOption("Сгенерировать список покупок", OnGenerateShoppingList),
            new MenuOption("Рецепт дня", OnShowRandomRecipe),
            new MenuOption("Сгенерировать рецепт", OnGenerateRecipe),
            new MenuOption("Выйти", OnExit)
        );

    private void OnShowRecipes()
    {
        new RecipesListPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnImport()
    {
        new ImportPage(Console, RecipeService, YandexDiskService).Show();
    }

    private void OnExport()
    {
        new ExportPage(Console, RecipeService, RecipesQuery, YandexDiskService).Show();
    }

    private void OnFilterAndSorting()
    {
        new QueryPage(Console, RecipeService, RecipesQuery).Show();
    }

    private void OnAddRecipe()
    {
        new AddRecipePage(Console, RecipeService).Show();
    }

    private void OnGenerateShoppingList()
    {
        new GenerateShoppingListPage(Console, RecipeService, RecipesQuery, ShoppingListService).Show();
    }

    private void OnShowRandomRecipe()
    {
        new RandomRecipePage(Console, RecipeService).Show();
    }

    private void OnGenerateRecipe()
    {
        new GenerateRecipePage(Console, RecipeService, GigaChatService).Show();
    }

    private void OnExit()
    {
        YandexDiskService.SaveOAuthToken();
        
        if (new ConfirmPrompt(Console, "Сохранить данные перед выходом?").Ask())
            new ExportPage(Console, RecipeService, RecipesQuery, YandexDiskService).Show();
        
        OnReturn();
    }

    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}