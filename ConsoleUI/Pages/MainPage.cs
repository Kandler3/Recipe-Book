/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Главная страница меню приложения, предоставляющая доступ к основным функциям.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="recipeService">Сервис для работы с рецептами.</param>
/// <param name="shoppingListService">Сервис для генерации списка покупок.</param>
/// <param name="yandexDiskService">Сервис для работы с Яндекс.Диском.</param>
/// <param name="gigaChatService">Сервис для генерации рецептов с помощью GigaChat.</param>
/// <param name="onReturn">Действие, выполняемое при выходе из приложения.</param>
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

    /// <summary>
    /// Промпт основного меню.
    /// </summary>
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

    /// <summary>
    /// Отображает страницу списка рецептов.
    /// </summary>
    private void OnShowRecipes()
    {
        new RecipesListPage(Console, RecipeService, RecipesQuery).Show();
    }

    /// <summary>
    /// Отображает страницу импорта рецептов.
    /// </summary>
    private void OnImport()
    {
        new ImportPage(Console, RecipeService, YandexDiskService).Show();
    }

    /// <summary>
    /// Отображает страницу экспорта рецептов.
    /// </summary>
    private void OnExport()
    {
        new ExportPage(Console, RecipeService, RecipesQuery, YandexDiskService).Show();
    }

    /// <summary>
    /// Отображает страницу фильтрации и сортировки рецептов.
    /// </summary>
    private void OnFilterAndSorting()
    {
        new QueryPage(Console, RecipeService, RecipesQuery).Show();
    }

    /// <summary>
    /// Отображает страницу добавления нового рецепта.
    /// </summary>
    private void OnAddRecipe()
    {
        new AddRecipePage(Console, RecipeService).Show();
    }

    /// <summary>
    /// Отображает страницу генерации списка покупок.
    /// </summary>
    private void OnGenerateShoppingList()
    {
        new GenerateShoppingListPage(Console, RecipeService, RecipesQuery, ShoppingListService).Show();
    }

    /// <summary>
    /// Отображает случайный рецепт (рецепт дня).
    /// </summary>
    private void OnShowRandomRecipe()
    {
        new RandomRecipePage(Console, RecipeService).Show();
    }

    /// <summary>
    /// Отображает страницу генерации рецепта с помощью GigaChat.
    /// </summary>
    private void OnGenerateRecipe()
    {
        new GenerateRecipePage(Console, RecipeService, GigaChatService).Show();
    }

    /// <summary>
    /// Выполняет выход из приложения, предлагая сохранить данные перед закрытием.
    /// </summary>
    private void OnExit()
    {
        YandexDiskService.SaveOAuthToken();

        if (new ConfirmPrompt(Console, "Сохранить данные перед выходом?").Ask())
            new ExportPage(Console, RecipeService, RecipesQuery, YandexDiskService).Show();

        OnReturn();
    }

    /// <summary>
    /// Отображает главное меню и обрабатывает пользовательский ввод.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        Console.Prompt(MenuSelectionPrompt).InvokeSelect();
    }
}
