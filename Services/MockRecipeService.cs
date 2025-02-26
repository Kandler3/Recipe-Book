using Interfaces;
using Models;

namespace Services;

public class MockRecipeService : IRecipeService
{
    public IList<Recipe> GetRecipes(IRecipesQuery query)
    {
        return
        [
            new() { Title = "Apple", Category = "Fruits" },
            new() { Title = "Apple Pie", Category = "Fruits" },
            new() { Title = "Banana Pie", Category = "Fruits" },
            new() { Title = "Classic Burger", Category = "Dinner" },
        ];
    }

    public void AddRecipe(Recipe recipe)
    {
    }

    public void DeleteRecipe(Recipe recipe)
    {
    }
    
    public void AddRecipeImage(Recipe recipe, string imagePath) {}
}