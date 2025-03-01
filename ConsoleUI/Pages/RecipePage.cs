using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class RecipePage(IAnsiConsole console, IRecipeService service,Recipe recipe)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private Recipe PanelRecipe { get; } = recipe;

    public void Show()
    {
        
        Console.Clear();
        Console.Write(new RecipePanel(PanelRecipe));

        string option = Console.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(
                    "Добавить изображение",
                    "Назад"
                )
        );
        if (option.Equals("Добавить изображение"))
        {
            Service.AddRecipeImage(
                PanelRecipe, 
                new FilepathPrompt(Console, "Введите путь до изображения", true).Ask()
            );

            Show();
        }
    }
}