using Contracts;
using Models;

namespace ConsoleUI;

public class RecipesQuery : IRecipesQuery
{
    public string? TitleSearchQuery { get; set; }

    private readonly List<string> _categories = [];
    public List<string> CategoriesList => _categories;
    public IEnumerable<string>? Categories => _categories.Count > 0 ? _categories : null;

    private readonly List<string> _ingredients = [];
    public List<string> IngredientsList => _ingredients;
    public IEnumerable<string>? Ingredients => _ingredients.Count > 0 ? _ingredients : null;
    public RecipeSortingParameter? SortingParameter { get; set; }
    public bool? AscendingSorting { get; set; }

    public void ResetFilter()
    {
        TitleSearchQuery = null;
        CategoriesList.Clear();
        IngredientsList.Clear();
    }

    public void ResetSorting()
    {
        SortingParameter = null;
    }
}