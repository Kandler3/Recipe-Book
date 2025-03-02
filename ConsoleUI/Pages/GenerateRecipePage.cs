/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using System.Security.Authentication;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница генерации рецепта с использованием GigaChat.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="recipeService">Сервис для управления рецептами.</param>
/// <param name="gigaChatService">Сервис GigaChat для генерации рецептов.</param>
public class GenerateRecipePage(IAnsiConsole console, IRecipeService recipeService, IGigaChatService gigaChatService)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService RecipeService { get; } = recipeService;
    private IGigaChatService GigaChatService { get; } = gigaChatService;
    private Recipe? GeneratedRecipe { get; set; }

    /// <summary>
    /// Отображает страницу генерации рецепта и запрашивает описание у пользователя.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        var prompt = Console.Prompt(new TextPrompt<string>("Опишите рецепт: "));
        GenerateRecipe(prompt);
    }

    /// <summary>
    /// Генерирует рецепт на основе пользовательского запроса и обрабатывает возможные ошибки.
    /// </summary>
    /// <param name="prompt">Текстовое описание рецепта, передаваемое в GigaChat.</param>
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
            case "Повторить генерацию":
                GenerateRecipe(prompt);
                break;
            case "Сохранить рецепт":
                OnSave();
                break;
            case "Назад": return;
        }
    }

    /// <summary>
    /// Сохраняет сгенерированный рецепт в сервис рецептов.
    /// </summary>
    private void OnSave()
    {
        if (GeneratedRecipe == null)
        {
            new MessagePage(Console, new ErrorText("Нет сгенерированного рецепта\n")).Show();
            return;
        }

        RecipeService.AddRecipe(GeneratedRecipe!);
        new MessagePage(Console, new SuccessText("Рецепт добавлен\n")).Show();
    }
}
