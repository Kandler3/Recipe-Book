/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;
using Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI;

/// <summary>
/// Класс с вспомогательными методами.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Генерирует строку для отображения рецепта в таблице.
    /// </summary>
    /// <param name="recipe">Рецепт для отображения.</param>
    /// <param name="isChosen">Флаг, указывающий, выбран ли данный рецепт.</param>
    /// <returns>Коллекция IRenderable элементов.</returns>
    public static IEnumerable<IRenderable> GetRowFromRecipe(Recipe recipe, bool isChosen)
    {
        return isChosen
            ? [new Text(recipe.Title, new Style(Color.Green)), new Text(recipe.Category ?? "", new Style(Color.Green))]
            : [new Text(recipe.Title), new Text(recipe.Category ?? "")];
    }

    /// <summary>
    /// Возвращает строку с расширением файла, соответствующего формату.
    /// </summary>
    /// <param name="fileFormat">Формат файла.</param>
    /// <returns>Строка с расширением файла.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если формат не поддерживается.</exception>
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

    /// <summary>
    /// Генерирует список строк, описывающих параметры запроса.
    /// </summary>
    /// <param name="query">Объект запроса рецептов.</param>
    /// <returns>Список строк с параметрами запроса.</returns>
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

    /// <summary>
    /// Преобразует параметр сортировки в человекочитаемый формат.
    /// </summary>
    /// <param name="sortingParameter">Параметр сортировки.</param>
    /// <returns>Локализованное название параметра сортировки.</returns>
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
