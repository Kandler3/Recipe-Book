using Models;
using Spectre.Console;

namespace ConsoleUI;

public class RecipesTable(IAnsiConsole console, IList<Recipe> recipes, Action onReturn)
{
    private IAnsiConsole Console { get; } = console;
    private IList<Recipe> Recipes { get; } = recipes;
    private event Action OnReturn = onReturn;

    public void Show()
    {
        Console.Clear();
        int index = 0;
        ConsoleKey key;
        
        AnsiConsole.Live(Text.Empty).Start(ctx =>
        {
            do
            {
                var table = new Table();
                table.AddColumn("[bold]Название[/]");
                table.AddColumn("[bold]Категория[/]");

                for (int i = 0; i < Recipes.Count; i++)
                {
                    table.AddRow(Utils.GetRowFromRecipe(Recipes[i], i == index));
                }

                ctx.UpdateTarget(table);
                key = System.Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && index > 0) index--;
                if (key == ConsoleKey.DownArrow && index < Recipes.Count - 1) index++;

            } while (key != ConsoleKey.Enter);
        });
    }
}