using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI;

public static class Utils
{
    public static IEnumerable<IRenderable> GetRowFromRecipe(Recipe recipe, bool isChosen) => isChosen
        ? [new Text(recipe.Title, new Style(Color.Green)), new Text(recipe.Category, new Style(Color.Green))]
        : [new Text(recipe.Title), new Text(recipe.Category)];

}