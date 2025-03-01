using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class ShoppingListPanel : IRenderable
{
    private Table IngredientTable { get; }

    public ShoppingListPanel(IEnumerable<Ingredient> shoppingList)
    {
        IngredientTable = new Table().Title("Список покупок").AddColumns("Название", "Количество", "Ед. измерения");
        foreach (Ingredient ingredient in shoppingList)
            IngredientTable.AddRow(ingredient.Name, ingredient.Quantity?.ToString() ?? "", ingredient.Measurement ?? "");
    }

    public Measurement Measure(RenderOptions options, int maxWidth) =>
        ((IRenderable)IngredientTable).Measure(options, maxWidth);

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) =>
        ((IRenderable)IngredientTable).Render(options, maxWidth);
}