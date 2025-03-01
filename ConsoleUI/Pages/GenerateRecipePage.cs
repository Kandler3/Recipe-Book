using System.Security.Authentication;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class GenerateRecipePage(IAnsiConsole console, IRecipeService recipeService, IGigaChatService gigaChatService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IGigaChatService GigaChatService { get; } = gigaChatService;
    private Recipe? GeneratedRecipe { get; set; }

    public void Show()
    {
        Console.Clear();
        string prompt = Console.Prompt(new TextPrompt<string>("Опишите рецепт: "));
        GenerateRecipe(prompt);
    }

    private void GenerateRecipe(string prompt)
    {
        string option;
        try
        {
            Console.Clear();
            GeneratedRecipe = GigaChatService.GenerateRecipe(prompt);
            Console.Write(new RecipePanel(GeneratedRecipe));
            option = Console.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Повторить генерацию", "Сохранить рецепт", "Назад")
            );
        }
        catch (Exception e) when (e is FormatException or AuthenticationException or HttpRequestException)
        {
            if (e is FormatException)
                Console.Write(new ErrorText("Не удалось распарсить ответ GigaChat в рецепт\n"));
            
            else if (e is AuthenticationException)
                Console.Write(new ErrorText("Ошибка при авторизации в GigaChat. Проверьте .env\n"));

            else
                Console.Write(new ErrorText("Ошибка при работе с GigaChat\n"));
            
            option = Console.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices("Повторить генерацию", "Назад")
            );
        }
        
        switch (option)
        {
            case "Повторить генерацию": GenerateRecipe(prompt); break;
            case "Сохранить рецепт": RecipeService.AddRecipe(GeneratedRecipe!); break;
            case "Назад": return;
        }
    }
}