/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

namespace Models;

/// <summary>
/// Представляет рецепт.
/// </summary>
/// <param name="title">Название рецепта.</param>
/// <param name="category">Категория рецепта (опционально).</param>
/// <param name="ingredients">Список ингредиентов (опционально).</param>
/// <param name="instructions">Список инструкций (опционально).</param>
/// <param name="images">Список путей к изображениям (опционально).</param>
public class Recipe(
    string title,
    string? category = null,
    List<Ingredient>? ingredients = null,
    List<string>? instructions = null,
    List<string>? images = null
)
{
    /// <summary>
    /// Инициализирует новый экземпляр рецепта с названием "no-title".
    /// </summary>
    public Recipe() : this("no-title")
    {
    }

    public string Title { get; set; } = title;
    public string? Category { get; set; } = category;
    public List<Ingredient>? Ingredients { get; set; } = ingredients;
    public List<string>? Instructions { get; set; } = instructions;
    public List<string>? Images { get; set; } = images;

    /// <summary>
    /// Возвращает название рецепта.
    /// </summary>
    /// <returns>Название рецепта.</returns>
    public override string ToString()
    {
        return Title;
    }
}