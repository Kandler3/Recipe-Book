/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница настройки фильтрации рецептов.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для работы с рецептами.</param>
/// <param name="query">Объект запроса, содержащий параметры фильтрации.</param>
public class RecipeFilterPage(IAnsiConsole console, IRecipeService service, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private RecipesQuery Query { get; } = query;

    /// <summary>
    /// Отображает страницу выбора параметров фильтрации.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        var field = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для фильтрации")
                .AddChoices(
                    "Название",
                    "Категория",
                    "Ингредиенты",
                    "Назад"
                )
        );

        switch (field)
        {
            case "Название":
                ShowTitleFilter();
                break;
            case "Категория":
                ShowCategoryFilter();
                break;
            case "Ингредиенты":
                ShowIngredientFilter();
                break;
            case "Назад": return;
        }

        Show();
    }

    /// <summary>
    /// Отображает запрос на ввод фильтра по названию.
    /// </summary>
    private void ShowTitleFilter()
    {
        Console.Clear();
        var query = Console.Prompt(
            new TextPrompt<string>("Фильтр по названию: ").AllowEmpty()
        );
        Query.TitleSearchQuery = query;
    }

    /// <summary>
    /// Отображает выбор категорий для фильтрации.
    /// </summary>
    private void ShowCategoryFilter()
    {
        var categories = Service.GetCategories().ToList();
        if (categories.Count == 0)
        {
            new MessagePage(Console, new Text("Нет доступных категорий\n")).Show();
            return;
        }

        var prompt =
            new MultiSelectionPrompt<string>()
                .NotRequired()
                .Title("Выберите категории")
                .AddChoices(categories);

        foreach (var category in Query.CategoriesList) 
            prompt.Select(category);

        Console.Clear();
        Query.CategoriesList.Clear();
        Query.CategoriesList.AddRange(Console.Prompt(prompt));
    }

    /// <summary>
    /// Отображает выбор ингредиентов для фильтрации.
    /// </summary>
    private void ShowIngredientFilter()
    {
        var ingredients = Service.GetIngredients().ToList();
        if (ingredients.Count == 0)
        {
            new MessagePage(Console, new Text("Нет доступных ингредиентов\n")).Show();
            return;
        }

        var prompt =
            new MultiSelectionPrompt<string>()
                .NotRequired()
                .Title("Выберите ингредиенты")
                .AddChoices(ingredients);

        foreach (var ingredient in Query.IngredientsList) 
            prompt.Select(ingredient);

        Console.Clear();
        Query.IngredientsList.Clear();
        Query.IngredientsList.AddRange(Console.Prompt(prompt));
    }
}
