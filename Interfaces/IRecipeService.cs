using Models;

namespace Interfaces;

public interface IRecipeService
{
    public IList<Recipe> GetRecipes(IRecipesQuery query);
    public void AddRecipe(Recipe recipe);
    public void DeleteRecipe(Recipe recipe);
    public void AddRecipeImage(Recipe recipe, string imagePath);
}