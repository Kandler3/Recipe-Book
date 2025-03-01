using Models;

namespace Contracts.Interfaces;

public interface IShoppingListService
{
    public IEnumerable<Ingredient> GenerateShoppingList(IEnumerable<Recipe> recipes);
    public void Export(IEnumerable<Ingredient> ingredients, string fileName);
}