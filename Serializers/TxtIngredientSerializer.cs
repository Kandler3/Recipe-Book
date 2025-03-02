/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Interfaces;
using Models;

namespace Serializers;

/// <summary>
/// Сериализатор для ингредиентов рецептов в текстовом формате.
/// </summary>
public class TxtIngredientSerializer : IIngredientSerializer
{
    /// <summary>
    /// Сериализует коллекцию ингредиентов и сохраняет их в файл.
    /// </summary>
    /// <param name="ingredients">Коллекция ингредиентов для сериализации.</param>
    /// <param name="filePath">Путь к файлу для сохранения результатов.</param>
    public void FileSerialize(IEnumerable<Ingredient> ingredients, string filePath)
    {
        using StreamWriter writer = new(filePath);
        foreach (var ingredient in ingredients)
            writer.WriteLine(Serialize(ingredient));
    }

    /// <summary>
    /// Сериализует один ингредиент в строку.
    /// </summary>
    /// <param name="ingredient">Ингредиент для сериализации.</param>
    /// <returns>Строковое представление ингредиента.</returns>
    public string Serialize(Ingredient ingredient)
    {
        var res = ingredient.Name;

        if (ingredient.Quantity != null)
            res += $" - {ingredient.Quantity}";

        if (ingredient.Measurement != null)
            res += $" {ingredient.Measurement}";

        return res;
    }

    /// <summary>
    /// Десериализует ингредиент из текстовой строки.
    /// </summary>
    /// <param name="txtString">Строковое представление ингредиента.</param>
    /// <returns>Экземпляр десериализованного ингредиента.</returns>
    /// <exception cref="FormatException">
    /// Выбрасывается, если количество ингредиента не является целым числом больше 0.
    /// </exception>
    public Ingredient Deserialize(string txtString)
    {
        var args = txtString.Split(" - ", 2, StringSplitOptions.TrimEntries);
        var name = args[0];

        if (args.Length == 1)
            return new Ingredient(name);

        var quantityArgs =
            args[1].Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (!int.TryParse(quantityArgs[0], out var quantity) || quantity <= 0)
            throw new FormatException("Ingredient quantity must be an integer greater than 0");

        var measurement = quantityArgs.Length > 1 ? quantityArgs[1] : null;
        return new Ingredient(name, quantity, measurement);
    }
}
