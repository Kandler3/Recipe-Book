using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class RecipePanel: IRenderable
{
    private Tree Tree { get; set; }
    public RecipePanel(Recipe recipe)
    {
        Tree = new Tree(new Text(recipe.Title, style: new Style(decoration: Decoration.Bold)));
        if (recipe.Category != null)
            Tree.AddNode(new Text($"Категория: {recipe.Category}"));

        if (recipe.Ingredients != null)
        {
            var ingredients = Tree.AddNode("Ингридиенты");
            foreach (var ingredient in recipe.Ingredients)
            {
                if (ingredient.Quantity == null)
                {
                    ingredients.AddNode(ingredient.Name);
                    continue;
                }

                ingredients.AddNode($"{ingredient.Name} - {ingredient.Quantity} {ingredient.Measurement}");
            }
        }

        if (recipe.Instructions != null)
        {
            var instructions = Tree.AddNode("Инструкция");
            foreach (var instruction in recipe.Instructions)
                instructions.AddNode(instruction);
        }
    }

    public Measurement Measure(RenderOptions options, int maxWidth) =>
        ((IRenderable)Tree).Measure(options, maxWidth);

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) => 
        ((IRenderable)Tree).Render(options, maxWidth);
}