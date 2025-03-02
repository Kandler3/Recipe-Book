/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;
using Contracts.Interfaces;

namespace ConsoleUI;

/// <summary>
/// Класс для формирования запроса фильтрации и сортировки рецептов.
/// </summary>
public class RecipesQuery : IRecipesQuery
{
    /// <summary>
    /// Список выбранных категорий.
    /// </summary>
    public List<string> CategoriesList { get; } = [];

    /// <summary>
    /// Список выбранных ингредиентов.
    /// </summary>
    public List<string> IngredientsList { get; } = [];

    /// <summary>
    /// Поисковый запрос по названию рецепта.
    /// </summary>
    public string? TitleSearchQuery { get; set; }

    /// <summary>
    /// Коллекция категорий, если они выбраны, иначе null.
    /// </summary>
    IEnumerable<string>? IRecipesQuery.Categories => CategoriesList.Count > 0 ? CategoriesList : null;

    /// <summary>
    /// Коллекция ингредиентов, если они выбраны, иначе null.
    /// </summary>
    IEnumerable<string>? IRecipesQuery.Ingredients => IngredientsList.Count > 0 ? IngredientsList : null;

    /// <summary>
    /// Параметр сортировки рецептов.
    /// </summary>
    public RecipeSortingParameter? SortingParameter { get; set; }

    /// <summary>
    /// Флаг направления сортировки (true - по возрастанию).
    /// </summary>
    public bool? AscendingSorting { get; set; }

    /// <summary>
    /// Сбрасывает параметры фильтрации рецептов.
    /// </summary>
    public void ResetFilter()
    {
        TitleSearchQuery = null;
        CategoriesList.Clear();
        IngredientsList.Clear();
    }

    /// <summary>
    /// Сбрасывает параметр сортировки рецептов.
    /// </summary>
    public void ResetSorting()
    {
        SortingParameter = null;
    }
}