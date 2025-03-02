/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница отображения случайного рецепта.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для работы с рецептами.</param>
public class RandomRecipePage(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    private Text Header { get; } = new(
        "Рецепт дня!\n",
        new Style(
            ConsoleColor.Magenta,
            ConsoleColor.Black,
            Decoration.RapidBlink
        )
    );

    private HintText NoRecipesText { get; } = new("Нет доступных рецептов((\n");

    private HintText ReturnText { get; } = new("Чтобы вернуться нажмите Backspace\n");

    /// <summary>
    /// Отображает случайный рецепт из списка доступных рецептов.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        Console.Write(Header);

        var recipes = Service.GetRecipes(new RecipesQuery());

        if (recipes.Count == 0)
        {
            Console.Write(NoRecipesText);
        }
        else
        {
            var random = new Random();
            var recipe = recipes.ElementAt(random.Next(0, recipes.Count));
            Console.Write(new RecipePanel(recipe));
        }

        Console.Write(ReturnText);
        Console.Cursor.Show(false);

        ConsoleKey key;
        do
        {
            key = System.Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Backspace);
    }
}