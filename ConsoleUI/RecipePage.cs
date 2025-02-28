using ConsoleUI.Widgets;
using Models;
using Spectre.Console;

namespace ConsoleUI;

public class RecipePage(IAnsiConsole console, Recipe recipe)
{
    private IAnsiConsole Console { get; } = console;
    private Recipe PanelRecipe { get; } = recipe;

    public void Show()
    {
        
        Console.Clear();
        Console.Write(new RecipePanel(PanelRecipe));
        Console.Write(new Text("\n\nЧтобы вернуться нажмите Backspace", style: new Style(foreground: ConsoleColor.Gray)));
        Console.Cursor.Show(false);

        ConsoleKey key;
        do
        {
            key = System.Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Backspace);
    }
}