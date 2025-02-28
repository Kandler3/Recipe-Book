using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class RecipesListPage
{
    private IAnsiConsole Console { get; }
    private IRecipeService RecipeService { get; }
    private RecipesQuery RecipesQuery { get; }
    private IList<Recipe> Recipes { get; set; }

    private int PageSize { get; } = 3;
    private int CurrentPage { get; set; } = 0;
    private int SelectedIndex { get; set; }
    private int MaxPage => Recipes.Count / PageSize + (Recipes.Count % PageSize > 0 ? 1 : 0);
    private int MinIndex => CurrentPage * PageSize;
    private int MaxIndex => int.Min((CurrentPage + 1) * PageSize, Recipes.Count);

    public RecipesListPage(IAnsiConsole console, IRecipeService recipeService, RecipesQuery query)
    {
        Console = console;
        RecipeService = recipeService;
        RecipesQuery = query;
        Recipes = RecipeService.GetRecipes(RecipesQuery);
    }

    public void Show()
    {
        Console.Clear();
        bool run = true;
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