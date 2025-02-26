using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using Interfaces;
using Models;

namespace Services;

public class MockRecipeService : IRecipeService
{

    private List<Recipe> Recipes { get; } =
    [
        new() { Title = "Apple", Category = "Fruits" },
        new() { Title = "Apple Pie", Category = "Fruits" },
        new() { Title = "Banana Pie", Category = "Fruits" },
        new() { Title = "Classic Burger", Category = "Dinner" },
    ];

    public IList<Recipe> GetRecipes(IRecipesQuery query)
    {
        IEnumerable<Recipe> res = Recipes.Where(
            recipe =>
                (query.TitleSearchQuery == null || recipe.Title.Contains(query.TitleSearchQuery))
                      && (query.Categories == null || query.Categories.Contains(recipe.Category))
                          && (query.Ingridients == null || recipe.Ingredients.Any(ingredient => query.Ingridients.Contains(ingredient)))
        );

        if (query.SortingParameter != null)
        {
            var keySelector = (Recipe recipe) => query.SortingParameter switch
            {
                "title" => recipe.Title,
                "category" => recipe.Category,
                _ => throw new ArgumentException("Invalid sorting parameter"),
            };

            if (query.AscendingSorting == true)
                res = res.OrderBy(keySelector);
            
            else
                res = res.OrderByDescending(keySelector);
        }
        
        return res.ToList();
    }

    public void AddRecipe(Recipe recipe)
    {
        Recipes.Add(recipe);
    }

    public void DeleteRecipe(Recipe recipe)
    {
        Recipes.Remove(recipe);
    }
    
    public void AddRecipeImage(Recipe recipe, string imagePath) {}
}