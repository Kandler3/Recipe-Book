using Models;

namespace Contracts.Interfaces;

public interface ISingleRecipeSerializer
{
    public Recipe DeserializeRecipe(string recipe);
}