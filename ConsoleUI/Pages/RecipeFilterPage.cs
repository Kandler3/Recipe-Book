using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class RecipeFilterPage(IAnsiConsole console, IRecipeService service, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private RecipesQuery Query { get; } = query;

    public void Show()
    {
        Console.Clear();
        var field = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для фильтрации")
                .AddChoices(
                    "Название",
                    "Категория",
                    "Ингридиенты",
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
            case "Ингридиенты":
                ShowIngredientFilter();
                break;
            case "Назад": return;
        }

        Show();
    }

    private void ShowTitleFilter()
    {
        Console.Clear();
        var query = Console.Prompt(
            new TextPrompt<string>("Фильтр по названию: ").AllowEmpty()
        );
        Query.TitleSearchQuery = query;
    }

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

        foreach (var category in Query.CategoriesList) prompt.Select(category);

        Console.Clear();
        Query.CategoriesList.Clear();
        Query.CategoriesList.AddRange(Console.Prompt(prompt));
    }

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

        foreach (var ingredient in Query.IngredientsList) prompt.Select(ingredient);

        Console.Clear();
        Query.IngredientsList.Clear();
        Query.IngredientsList.AddRange(Console.Prompt(prompt));
    }
}