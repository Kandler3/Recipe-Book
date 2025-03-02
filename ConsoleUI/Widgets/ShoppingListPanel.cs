using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class ShoppingListPanel : IRenderable
{
    public ShoppingListPanel(IEnumerable<Ingredient> shoppingList)
    {
        IngredientTable = new Table().Title("Список покупок").AddColumns("Название", "Количество", "Ед. измерения");
        foreach (var ingredient in shoppingList)
            IngredientTable.AddRow(ingredient.Name, ingredient.Quantity?.ToString() ?? "",
                ingredient.Measurement ?? "");
    }

    private Table IngredientTable { get; }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)IngredientTable).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)IngredientTable).Render(options, maxWidth);
    }
}