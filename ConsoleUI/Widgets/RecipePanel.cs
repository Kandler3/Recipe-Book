/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;
using SixLabors.ImageSharp;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

/// <summary>
/// Панель для отображения рецепта с деревом деталей и изображениями.
/// </summary>
public class RecipePanel : IRenderable
{
    /// <summary>
    /// Создает панель рецепта с заданным рецептом.
    /// </summary>
    /// <param name="recipe">Рецепт для отображения.</param>
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

    /// <summary>
    /// Генерирует древовидное представление рецепта с его деталями.
    /// </summary>
    /// <param name="recipe">Рецепт для отображения.</param>
    /// <returns>Дерево с информацией о рецепте.</returns>
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

    /// <summary>
    /// Генерирует колонку для отображения изображений рецепта.
    /// </summary>
    /// <param name="recipe">Рецепт.</param>
    /// <returns>Колонки с изображениями или текстовыми сообщениями об ошибке загрузки.</returns>
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
                // Заглушка в случае ошибки.
                images.Add(new Text($"Не удалось загрузить изображение {path}"));
            }

        return new Columns(images).Expand();
    }
}
