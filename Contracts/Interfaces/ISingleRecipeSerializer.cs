/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс для десериализации одного рецепта.
/// </summary>
public interface ISingleRecipeSerializer
{
    /// <summary>
    /// Десериализует рецепт из строки.
    /// </summary>
    /// <param name="recipe">Строковое представление рецепта.</param>
    /// <returns>Экземпляр рецепта.</returns>
    public Recipe DeserializeRecipe(string recipe);
}