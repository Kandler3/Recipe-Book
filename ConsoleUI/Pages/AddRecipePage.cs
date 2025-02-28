using ConsoleUI.Prompts;
using Contracts;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class AddRecipePage(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    public void Show()
    {
        var title = Console.Prompt(new TextPrompt<string>("Название рецепта: "));
        var category = Console.Prompt(new TextPrompt<string>("Категория: "));
        List<Ingredient> ingredients = new ListPrompt<Ingredient>(Console,
            "Ингредиенты (формат: \"[название] - [количество] [ед. измерения]\"): ", ParseIngredient).Ask();
        List<string> instructions = new ListPrompt<string>(Console, "Инструкция: ", str => str).Ask();

        service.AddRecipe(new Recipe(
            title,
            category != "" ? category : null,
            ingredients.Count != 0 ? ingredients : null,
            instructions.Count != 0 ? instructions : null
        ));
    }
    
    private Ingredient ParseIngredient(string input)
    {
        string[] args = input.Split(" - ", 2, StringSplitOptions.TrimEntries);
        string name = args[0];
        
        if (args.Length == 1)
            return new Ingredient(name);
        
        string[] quantityArgs = args[1].Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        
        if (!int.TryParse(quantityArgs[0], out int quantity) || quantity <= 0)
            throw new FormatException("Ingredient quantity must be an integer greater than 0");
        
        string? measurement = quantityArgs.Length > 1 ? quantityArgs[1] : null;
        return new Ingredient(name, quantity, measurement);
    }
}