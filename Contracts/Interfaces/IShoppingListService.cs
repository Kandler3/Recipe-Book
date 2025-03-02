/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы со списком покупок.
/// </summary>
public interface IShoppingListService
{
    /// <summary>
    /// Генерирует список покупок на основе переданных рецептов.
    /// </summary>
    /// <param name="recipes">Коллекция рецептов, по которым формируется список покупок.</param>
    /// <returns>Список ингредиентов для покупок.</returns>
    public IEnumerable<Ingredient> GenerateShoppingList(IEnumerable<Recipe> recipes);

    /// <summary>
    /// Экспортирует список ингредиентов в файл.
    /// </summary>
    /// <param name="ingredients">Коллекция ингредиентов для экспорта.</param>
    /// <param name="fileName">Путь к файлу для сохранения списка.</param>
    public void Export(IEnumerable<Ingredient> ingredients, string fileName);
}