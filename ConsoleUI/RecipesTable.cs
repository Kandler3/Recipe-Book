using Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI;

public class RecipesTable
{
    private IAnsiConsole Console { get; }
    private IRecipeService RecipeService { get; }
    private IRecipesQuery RecipesQuery { get; }
    private IList<Recipe> Recipes { get; set; }
    private event Action OnReturn;

    private int PageSize { get; } = 3;
    private int CurrentPage { get; set; } = 0;
    private int SelectedIndex { get; set; }
    private int MaxPage => Recipes.Count / PageSize + (Recipes.Count % PageSize > 0 ? 1 : 0);
    private int MinIndex => CurrentPage * PageSize;
    private int MaxIndex => int.Min((CurrentPage + 1) * PageSize, Recipes.Count);

    public RecipesTable(IAnsiConsole console, IRecipeService recipeService, IRecipesQuery query,Action onReturn)
    {
        Console = console;
        RecipeService = recipeService;
        RecipesQuery = query;
        Recipes = RecipeService.GetRecipes(RecipesQuery);
        OnReturn += onReturn;
    }

    public void Show()
    {
        Console.Clear();
        int index = 0;
        bool run = true;
        ConsoleKey key;
        do
        {
            Table table = BuildTable();
            Console.Clear();
            Console.Write(table);
            Console.Cursor.Hide();
            
            key = System.Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Enter: 
                    run = false;
                    break;
                
                case ConsoleKey.Backspace:
                    run = false;
                    OnReturn?.Invoke();
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

    private Table BuildTable()
    {
        Table table = new();
        
        table.AddColumn("[bold]Название[/]");
        table.AddColumn("[bold]Категория[/]");
        
        table.Caption =
            new TableTitle(
                $"Страница {CurrentPage + 1}/{MaxPage}\n"
                + "\u2191/\u2193: выбрать рецепт\n"
                + "\u2190/\u2192: перелистнуть страницу\n"
                + "Enter: посмотреть рецепт\n"
                + "Del: удалить рецепт\n"
                + "Backspace: назад"
            );

        for (int i = MinIndex; i < MaxIndex; i++)
        {
            table.AddRow(Utils.GetRowFromRecipe(Recipes[i], i == SelectedIndex));
        }
        
        return table;
    }

    public void OnDeleteRecipe()
    {
        if (!new ConfirmPrompt(
                Console, $"Вы уверены, что хотите удалить рецепт \"{Recipes[SelectedIndex].Title}\"?").Ask()
           )
            return;
        RecipeService.DeleteRecipe(Recipes[SelectedIndex]);
        Recipes = RecipeService.GetRecipes(RecipesQuery);
        SelectedIndex = MinIndex;
    }
}