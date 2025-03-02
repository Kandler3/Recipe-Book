/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

/// <summary>
/// Страница отображения информации о рецепте.
/// </summary>
/// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
/// <param name="service">Сервис для работы с рецептами.</param>
/// <param name="recipe">Объект рецепта, информация о котором будет отображаться.</param>
public class RecipePage(IAnsiConsole console, IRecipeService service, Recipe recipe)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;
    private Recipe PanelRecipe { get; } = recipe;

    /// <summary>
    /// Отображает страницу с информацией о рецепте и позволяет добавить изображение.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        Console.Write(new RecipePanel(PanelRecipe));

        var option = Console.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(
                    "Добавить изображение",
                    "Назад"
                )
        );

        if (option.Equals("Добавить изображение"))
        {
            var imagePath = new FilepathPrompt(Console, "Введите путь до изображения", true).Ask();
            if (imagePath == null) return;

            Service.AddRecipeImage(PanelRecipe, imagePath);

            Show();
        }
    }
}