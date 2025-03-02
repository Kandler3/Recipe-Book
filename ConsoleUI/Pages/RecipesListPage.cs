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
/// Страница списка рецептов с возможностью просмотра, удаления и навигации по страницам.
/// </summary>
public class RecipesListPage
{
    /// <summary>
    /// Создает страницу списка рецептов.
    /// </summary>
    /// <param name="console">Консольный интерфейс для взаимодействия с пользователем.</param>
    /// <param name="recipeService">Сервис для работы с рецептами.</param>
    /// <param name="query">Запрос, содержащий параметры фильтрации и сортировки рецептов.</param>
    public RecipesListPage(IAnsiConsole console, IRecipeService recipeService, RecipesQuery query)
    {
        Console = console;
        RecipeService = recipeService;
        RecipesQuery = query;
        Recipes = RecipeService.GetRecipes(RecipesQuery);
    }

    private IAnsiConsole Console { get; }
    private IRecipeService RecipeService { get; }
    private RecipesQuery RecipesQuery { get; }
    private IList<Recipe> Recipes { get; set; }

    private int PageSize { get; } = 10;
    private int CurrentPage { get; set; }
    private int SelectedIndex { get; set; }
    private int MaxPage => Recipes.Count / PageSize + (Recipes.Count % PageSize > 0 ? 1 : 0);
    private int MinIndex => CurrentPage * PageSize;
    private int MaxIndex => int.Min((CurrentPage + 1) * PageSize, Recipes.Count);

    /// <summary>
    /// Отображает список рецептов и обрабатывает пользовательский ввод для навигации и управления.
    /// </summary>
    public void Show()
    {
        Console.Clear();
        if (Recipes.Count == 0)
        {
            new MessagePage(Console, new Text("Нет рецептов\n")).Show();
            return;
        }

        var run = true;
        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.Write(
                new RecipesTable(
                    Recipes.Skip(MinIndex).Take(PageSize).ToList(),
                    SelectedIndex - MinIndex,
                    CurrentPage,
                    MaxPage
                )
            );
            Console.Cursor.Hide();

            key = System.Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Enter:
                    if (Recipes.Count > 0)
                        new RecipePage(Console, RecipeService, Recipes[SelectedIndex]).Show();
                    break;

                case ConsoleKey.Backspace:
                    run = false;
                    break;

                case ConsoleKey.Delete:
                    OnDeleteRecipe();
                    break;

                case ConsoleKey.UpArrow when SelectedIndex > MinIndex:
                    SelectedIndex--;
                    break;

                case ConsoleKey.DownArrow when SelectedIndex < MaxIndex - 1:
                    SelectedIndex++;
                    break;

                case ConsoleKey.LeftArrow when CurrentPage > 0:
                    CurrentPage--;
                    SelectedIndex = CurrentPage * PageSize;
                    break;

                case ConsoleKey.RightArrow when CurrentPage < MaxPage - 1:
                    CurrentPage++;
                    SelectedIndex = CurrentPage * PageSize;
                    break;
            }
        } while (run);
    }

    /// <summary>
    /// Удаляет выбранный рецепт после подтверждения пользователя.
    /// </summary>
    public void OnDeleteRecipe()
    {
        if (Recipes.Count == 0)
            return;

        if (!new ConfirmPrompt(
                Console, $"Вы уверены, что хотите удалить рецепт \"{Recipes[SelectedIndex].Title}\"?").Ask()
           )
            return;
        RecipeService.DeleteRecipe(Recipes[SelectedIndex]);
        Recipes = RecipeService.GetRecipes(RecipesQuery);
        SelectedIndex = MinIndex;
    }
}
