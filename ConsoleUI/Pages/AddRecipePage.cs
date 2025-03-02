using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
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
        var category = Console.Prompt(new TextPrompt<string>("Категория: ").AllowEmpty());
        var ingredients = new ListPrompt<Ingredient>(Console,
            "Ингредиенты (формат: \"[название] - [количество] [ед. измерения]\"): ", ParseIngredient).Ask();
        var instructions = new ListPrompt<string>(Console, "Инструкция: ", str => str).Ask();

        Service.AddRecipe(new Recipe(
            title,
            category != "" ? category : null,
            ingredients.Count != 0 ? ingredients : null,
            instructions.Count != 0 ? instructions : null
        ));

        new MessagePage(Console, new SuccessText("Рецепт добавлен\n")).Show();
    }

    private static Ingredient ParseIngredient(string input)
    {
        var args = input.Split(" - ", 2, StringSplitOptions.TrimEntries);
        var name = args[0];

        if (args.Length == 1)
            return new Ingredient(name);

        var quantityArgs =
            args[1].Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (!int.TryParse(quantityArgs[0], out var quantity) || quantity <= 0)
            throw new FormatException("Ingredient quantity must be an integer greater than 0");

        var measurement = quantityArgs.Length > 1 ? quantityArgs[1] : null;
        return new Ingredient(name, quantity, measurement);
    }
}