/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс запроса для фильтрации рецептов.
/// </summary>
public interface IRecipesQuery
{
    /// <summary>
    /// Поисковый запрос по названию рецепта.
    /// </summary>
    public string? TitleSearchQuery { get; }

    /// <summary>
    /// Фильтр по категориям рецептов.
    /// </summary>
    public IEnumerable<string>? Categories { get; }

    /// <summary>
    /// Фильтр по ингредиентам рецептов.
    /// </summary>
    public IEnumerable<string>? Ingredients { get; }

    /// <summary>
    /// Параметр сортировки рецептов.
    /// </summary>
    public RecipeSortingParameter? SortingParameter { get; }

    /// <summary>
    /// Флаг направления сортировки (true - по возрастанию).
    /// </summary>
    public bool? AscendingSorting { get; }
}