/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

namespace Models;

/// <summary>
/// Представляет ингредиент рецепта.
/// </summary>
/// <param name="name">Название ингредиента.</param>
/// <param name="quantity">Количество ингредиента (опционально).</param>
/// <param name="measurement">Единица измерения ингредиента (опционально).</param>
public class Ingredient(string name, int? quantity = null, string? measurement = null)
{
    public string Name { get; } = name;
    public int? Quantity { get; set; } = quantity;
    public string? Measurement { get; } = measurement;
}