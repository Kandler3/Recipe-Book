using System.Text.Json;
using ConsoleUI;
using Contracts;
using Serializers;
using Services;
using Spectre.Console;

namespace App;

public struct Recipe
{
    public string Name { get; set; }
    public string Category { get; set; }
}

public static class Program
{

    public static void Main()
    {
        IRecipeSerializer txtSerializer = new TxtRecipeSerializer();
        IRecipeSerializer jsonSerializer = new JsonRecipeSerializer(false);
        IRecipeSerializer csvSerializer = new CsvRecipeSerializer();

        IRecipeService service = new RecipeService(
            txtSerializer,
            jsonSerializer,
            csvSerializer
        );
        var app = new ConsoleApp(AnsiConsole.Console, service);
        app.Run();
    }
    public static void Test()
    {
        AnsiConsole.MarkupLineInterpolated($"[yellow]Hello World![/]");
        string name = AnsiConsole.Ask<string>("What's your name?");
        int age = AnsiConsole.Ask<int>("What's your age?", 0);
        AnsiConsole.MarkupLineInterpolated($"[green]name: {name}[/] [red]age: {age}[/]");
        // Ask for the user's favorite fruit
        var fruit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your [green]favorite fruit[/]?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .AddChoices(new[] {
                    "Apple", "Apricot", "Avocado", 
                    "Banana", "Blackcurrant", "Blueberry",
                    "Cherry", "Cloudberry", "Cocunut",
                }));

// Echo the fruit back to the terminal
        AnsiConsole.WriteLine($"I agree. {fruit} is tasty!");
        
        int index = 0;
        ConsoleKey key;


        List<Recipe> recipes =
        [
            new() { Name = "Apple", Category = "Fruits" },
            new() { Name = "Apple Pie", Category = "Fruits" },
            new() { Name = "Banana Pie", Category = "Fruits" },
            new() { Name = "Classic Burger", Category = "Dinner" },
        ];

        AnsiConsole.Live(Text.Empty).Start(ctx =>
        {
            do
            {
                var table = new Table();
                table.AddColumn("[bold]Название[/]");
                table.AddColumn("[bold]Категория[/]");

                for (int i = 0; i < recipes.Count; i++)
                {
                    if (i == index)
                        table.AddRow($"[green]{recipes[i].Name}[/]", $"[blue]{recipes[i].Category}[/]");
                    else
                        table.AddRow(recipes[i].Name, recipes[i].Category);
                }

                ctx.UpdateTarget(table);
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && index > 0) index--;
                if (key == ConsoleKey.DownArrow && index < recipes.Count - 1) index++;

            } while (key != ConsoleKey.Enter);
        });

    }
}