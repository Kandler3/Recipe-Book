using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class RecipesTable : IRenderable
{
    private IList<Recipe> Recipes { get; }
    private int Selected { get; }
    private int Page { get; }
    private int MaxPage { get; }
    private Table Table { get; set; }

    public RecipesTable(IList<Recipe> recipes, int selected, int page, int maxPage)
    {
        Recipes = recipes;
        Selected = selected;
        Page = page;
        MaxPage = maxPage;
        Table = BuildTable();

    }
    
    private Table BuildTable()
    {
        Table table = new();
        
        table.AddColumn("[bold]Название[/]");
        table.AddColumn("[bold]Категория[/]");
        
        table.Caption =
            new TableTitle(
                $"Страница {Page + 1}/{MaxPage}\n"
                + "\u2191/\u2193: выбрать рецепт\n"
                + "\u2190/\u2192: перелистнуть страницу\n"
                + "Enter: посмотреть рецепт\n"
                + "Del: удалить рецепт\n"
                + "Backspace: назад"
            );

        for (int i = 0; i < Recipes.Count; i++)
        {
            table.AddRow(Utils.GetRowFromRecipe(Recipes[i], i == Selected));
        }
        
        return table;
    }

    public Measurement Measure(RenderOptions options, int maxWidth) => 
        ((IRenderable)Table).Measure(options, maxWidth);
    
    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) => 
        ((IRenderable)Table).Render(options, maxWidth);
}