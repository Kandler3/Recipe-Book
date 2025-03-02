/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;
using Models;

namespace Contracts.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с рецептами.
/// </summary>
public interface IRecipeService
{
    /// <summary>
    /// Возвращает список рецептов по заданному запросу.
    /// </summary>
    /// <param name="query">Параметры запроса.</param>
    /// <returns>Список рецептов.</returns>
    public IList<Recipe> GetRecipes(IRecipesQuery query);

    /// <summary>
    /// Возвращает список категорий рецептов.
    /// </summary>
    /// <returns>Список категорий.</returns>
    public IEnumerable<string> GetCategories();

    /// <summary>
    /// Возвращает список ингредиентов, используемых в рецептах.
    /// </summary>
    /// <returns>Список ингредиентов.</returns>
    public IEnumerable<string> GetIngredients();

    /// <summary>
    /// Добавляет новый рецепт.
    /// </summary>
    /// <param name="recipe">Рецепт для добавления.</param>
    public void AddRecipe(Recipe recipe);

    /// <summary>
    /// Удаляет указанный рецепт.
    /// </summary>
    /// <param name="recipe">Рецепт для удаления.</param>
    public void DeleteRecipe(Recipe recipe);

    /// <summary>
    /// Добавляет изображение к рецепту.
    /// </summary>
    /// <param name="recipe">Рецепт, к которому добавляется изображение.</param>
    /// <param name="imagePath">Путь к изображению.</param>
    public void AddRecipeImage(Recipe recipe, string imagePath);

    /// <summary>
    /// Импортирует рецепты из файла.
    /// </summary>
    /// <param name="filepath">Путь к файлу для импорта.</param>
    /// <param name="format">Формат файла.</param>
    /// <param name="local">Флаг, указывающий на локальный импорт.</param>
    public void Import(string filepath, FileFormat format, bool local = true);

    /// <summary>
    /// Экспортирует рецепты в файл.
    /// </summary>
    /// <param name="filepath">Путь к файлу для экспорта.</param>
    /// <param name="format">Формат файла.</param>
    /// <param name="query">Параметры запроса для фильтрации экспортируемых рецептов (опционально).</param>
    /// <param name="local">Флаг, указывающий на локальный экспорт.</param>
    public void Export(string filepath, FileFormat format, IRecipesQuery? query = null, bool local = true);
}
