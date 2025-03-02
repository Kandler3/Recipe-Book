using Contracts.Interfaces;
using Models;

namespace Serializers;

public class TxtIngredientSerializer : IIngredientSerializer
{
    public void FileSerialize(IEnumerable<Ingredient> ingredients, string filePath)
    {
        using StreamWriter writer = new(filePath);
        foreach (var ingredient in ingredients)
            writer.WriteLine(Serialize(ingredient));
    }

    public string Serialize(Ingredient ingredient)
    {
        var res = ingredient.Name;

        if (ingredient.Quantity != null)
            res += $" - {ingredient.Quantity}";

        if (ingredient.Measurement != null)
            res += $" {ingredient.Measurement}";

        return res;
    }

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