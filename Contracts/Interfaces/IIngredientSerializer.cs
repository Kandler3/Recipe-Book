/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс для сериализации ингредиентов в файл.
/// </summary>
public interface IIngredientSerializer
{
    /// <summary>
    /// Сериализует коллекцию ингредиентов и сохраняет их в файл.
    /// </summary>
    /// <param name="ingredients">Коллекция ингредиентов для сериализации.</param>
    /// <param name="filepath">Путь к файлу для сохранения.</param>
    public void FileSerialize(IEnumerable<Ingredient> ingredients, string filepath);
}