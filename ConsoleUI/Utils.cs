using Contracts.Enums;
using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI;

internal static class Utils
{
    public static IEnumerable<IRenderable> GetRowFromRecipe(Recipe recipe, bool isChosen)
    {
        return isChosen
            ? [new Text(recipe.Title, new Style(Color.Green)), new Text(recipe.Category ?? "", new Style(Color.Green))]
            : [new Text(recipe.Title), new Text(recipe.Category ?? "")];
    }

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

    public static List<string> GenerateQueryRows(RecipesQuery query)
    {
        List<string> rows = [];

        if (query.TitleSearchQuery != null)
            rows.Add($"Фильтр по названию: {query.TitleSearchQuery}");

        if (query.CategoriesList.Count != 0)
            rows.Add($"Фильтр по категориям: {string.Join(", ", query.CategoriesList)}");

        if (query.IngredientsList.Count != 0)
            rows.Add($"Фильтр по ингредиентам: {string.Join(", ", query.IngredientsList)}");

        if (query.SortingParameter != null)
            rows.Add(
                $"Сортировка по {(query.AscendingSorting!.Value ? "возрастанию" : "убыванию")}: {ToLocaleSortingParameter(query.SortingParameter.Value)}");

        return rows;
    }

    public static string ToLocaleSortingParameter(RecipeSortingParameter sortingParameter)
    {
        return sortingParameter switch
        {
            RecipeSortingParameter.Title => "Название",
            RecipeSortingParameter.Category => "Категория",
            _ => sortingParameter.ToString()
        };
    }
}