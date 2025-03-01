using Models;

namespace Contracts.Interfaces;

public interface IIngredientSerializer
{
    public void FileSerialize(IEnumerable<Ingredient> ingredients, string filepath);
}