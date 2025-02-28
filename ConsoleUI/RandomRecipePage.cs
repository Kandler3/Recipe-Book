using ConsoleUI.Widgets;
using Contracts;
using Models;
using Spectre.Console;

namespace ConsoleUI;

public class RandomRecipePage(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    private Text Header { get; } = new Text(
        "Рецепт дня!\n",
        new Style(
            foreground: ConsoleColor.Magenta,
            background: ConsoleColor.Black,
            decoration: Decoration.SlowBlink
        )
    );

    private Text NoRecipesText { get; } = new Text(
        "Нет доступных рецептов((",
        new Style(foreground: ConsoleColor.DarkGray)
    );

    private Text ReturnText { get; } = new Text(
        "\n\nЧтобы вернуться нажмите Backspace",
        style: new Style(foreground: ConsoleColor.DarkGray)
    );

    public void Show()
    {
        Console.Clear();
        Console.Write(Header);

        IList<Recipe> recipes = Service.GetRecipes(new RecipesQuery());

        if (recipes.Count == 0)
            Console.Write(NoRecipesText);

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