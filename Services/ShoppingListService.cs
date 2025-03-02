using Contracts.Interfaces;
using Models;

namespace Services;

public class ShoppingListService(IIngredientSerializer serializer) : IShoppingListService
{
    private IIngredientSerializer Serializer { get; } = serializer;

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

    public void Export(IEnumerable<Ingredient> shoppingList, string path)
    {
        Serializer.FileSerialize(shoppingList, path);
    }
}