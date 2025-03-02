/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Interfaces;
using Models;

namespace Services;

/// <summary>
/// Сервис для формирования списка покупок на основе рецептов.
/// </summary>
/// <param name="serializer">Сериализатор ингредиентов для экспорта списка покупок.</param>
public class ShoppingListService(IIngredientSerializer serializer) : IShoppingListService
{
    private IIngredientSerializer Serializer { get; } = serializer;

    /// <summary>
    /// Генерирует список покупок, объединяя ингредиенты из набора рецептов.
    /// </summary>
    /// <param name="recipes">Коллекция рецептов, по которым формируется список покупок.</param>
    /// <returns>Список ингредиентов для покупок.</returns>
    public IEnumerable<Ingredient> GenerateShoppingList(IEnumerable<Recipe> recipes)
    {
        Dictionary<(string, string?), Ingredient> res = [];
        foreach (var recipe in recipes)
        {
            if (recipe.Ingredients == null)
                continue;

            foreach (var ingredient in recipe.Ingredients)
            {
                var key = (ingredient.Name, ingredient.Measurement);
                if (res.TryGetValue(key, out var value))
                {
                    value.Quantity += ingredient.Quantity;
                    continue;
                }

                res[key] = ingredient;
            }
        }

        return res.Values;
    }

    /// <summary>
    /// Экспортирует список покупок в файл.
    /// </summary>
    /// <param name="shoppingList">Список ингредиентов для экспорта.</param>
    /// <param name="path">Путь к файлу для сохранения списка.</param>
    public void Export(IEnumerable<Ingredient> shoppingList, string path)
    {
        Serializer.FileSerialize(shoppingList, path);
    }
}