using ConsoleUI.Prompts;
using Contracts;
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
        string field = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для фильтрации")
                .AddChoices(
                    "Название",
                    "Категория",
                    "Ингридиенты",
                    "Сбросить",
                    "Назад"
                )
        );

        switch (field)
        {
            case "Название": ShowTitleFilter();
                break;
            case "Категория": ShowCategoryFilter(); break;
            case "Ингридиенты": ShowIngredientFilter(); break;
            case "Сбросить": ShowResetPrompt(); break;
            case "Назад": return;
        }

        Show();
    }

    private void ShowTitleFilter()
    {
        Console.Clear();
        string query = Console.Prompt(
            new TextPrompt<string>("Фильтр по названию: ")
        );
        Query.TitleSearchQuery = query;
    }

    private void ShowCategoryFilter()
    {
        var prompt =
            new MultiSelectionPrompt<string>()
                .NotRequired()
                .Title("Выберите категории")
                .AddChoices(Service.GetCategories());

        foreach (string category in Query.CategoriesList)
        {
            prompt.Select(category);
        }
        
        Console.Clear();
        Query.CategoriesList.Clear();
        Query.CategoriesList.AddRange(Console.Prompt(prompt));
    }

    private void ShowIngredientFilter()
    {
        var prompt =
            new MultiSelectionPrompt<string>()
                .NotRequired()
                .Title("Выберите ингредиенты")
                .AddChoices(Service.GetIngredients());

        foreach (string ingredient in Query.IngredientsList)
        {
            prompt.Select(ingredient);
        }
        
        Console.Clear();
        Query.IngredientsList.Clear();
        Query.IngredientsList.AddRange(Console.Prompt(prompt));
    }

    private void ShowResetPrompt()
    {
        if (new ConfirmPrompt(Console, "Вы уверены, что хотите сбросить фильтрацию?").Ask())
            Query.ResetFilter();
    }
}