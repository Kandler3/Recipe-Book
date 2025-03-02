using Models;
using SixLabors.ImageSharp;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class RecipePanel : IRenderable
{
    public RecipePanel(Recipe recipe)
    {
        RecipeLayout = new Layout().SplitColumns(new Layout("Tree"), new Layout("Images"));
        RecipeLayout["Tree"].Update(GenerateTree(recipe));
        RecipeLayout["Images"].Update(GenerateImagesColumn(recipe));
    }

    private Layout RecipeLayout { get; }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RecipeLayout).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RecipeLayout).Render(options, maxWidth);
    }

    private static Tree GenerateTree(Recipe recipe)
    {
        var tree = new Tree(new Text(recipe.Title, new Style(decoration: Decoration.Bold)));
        if (recipe.Category != null)
            tree.AddNode(new Text($"Категория: {recipe.Category}"));

        if (recipe.Ingredients != null)
        {
            var ingredients = tree.AddNode("Ингридиенты");
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
            var instructions = tree.AddNode("Инструкция");
            foreach (var instruction in recipe.Instructions)
                instructions.AddNode(instruction);
        }

        return tree;
    }

    private static Columns GenerateImagesColumn(Recipe recipe)
    {
        if (recipe.Images == null)
            return new Columns().Collapse();

        List<IRenderable> images = [];
        foreach (var path in recipe.Images)
            try
            {
                images.Add(new CanvasImage(path).MaxWidth(15));
            }
            catch (Exception e) when (e is ImageFormatException or NotSupportedException or IOException)
            {
                images.Add(new Text($"Не удалось загрузить изображение {path}"));
            }

        return new Columns(images).Expand();
    }
}