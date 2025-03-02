using Contracts.Enums;
using Contracts.Interfaces;

namespace ConsoleUI;

public class RecipesQuery : IRecipesQuery
{
    public List<string> CategoriesList { get; } = [];

    public List<string> IngredientsList { get; } = [];

    public string? TitleSearchQuery { get; set; }
    public IEnumerable<string>? Categories => CategoriesList.Count > 0 ? CategoriesList : null;
    public IEnumerable<string>? Ingredients => IngredientsList.Count > 0 ? IngredientsList : null;
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