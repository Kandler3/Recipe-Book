using Contracts;
using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI;

internal static class Utils
{
    public static IEnumerable<IRenderable> GetRowFromRecipe(Recipe recipe, bool isChosen) => isChosen
        ? [new Text(recipe.Title, new Style(Color.Green)), new Text(recipe.Category ?? "", new Style(Color.Green))]
        : [new Text(recipe.Title), new Text(recipe.Category ?? "")];

    public static string GetFileFormatPostfix(FileFormat fileFormat)
    {
        return fileFormat switch
        {
            FileFormat.Txt => ".txt",
            FileFormat.Json => ".json",
            FileFormat.Csv => ".csv",
            _ => throw new ArgumentException("Unsupported file format")
        };
    }

}