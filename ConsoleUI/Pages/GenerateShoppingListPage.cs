/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница генерации списка покупок на основе выбранных рецептов.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="recipeService">Сервис для работы с рецептами.</param>
/// <param name="query">Запрос, содержащий параметры фильтрации рецептов.</param>
/// <param name="shoppingListService">Сервис для генерации списка покупок.</param>
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

    /// <summary>
    /// Отображает страницу генерации списка покупок и позволяет выбрать рецепты.
    /// </summary>
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