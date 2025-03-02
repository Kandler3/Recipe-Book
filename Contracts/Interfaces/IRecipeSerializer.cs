/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс для сериализации и десериализации рецептов.
/// </summary>
public interface IRecipeSerializer
{
    /// <summary>
    /// Десериализует рецепты из файла.
    /// </summary>
    /// <param name="inputFilepath">Путь к входному файлу.</param>
    /// <returns>Коллекция десериализованных рецептов.</returns>
    public IEnumerable<Recipe> FileDeserialize(string inputFilepath);

    /// <summary>
    /// Сериализует коллекцию рецептов и сохраняет их в файл.
    /// </summary>
    /// <param name="recipes">Коллекция рецептов для сериализации.</param>
    /// <param name="outputFilepath">Путь к выходному файлу.</param>
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath);
}