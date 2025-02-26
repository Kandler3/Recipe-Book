using Interfaces;
using Models;

namespace ConsoleUI;

public class RecipesQuery : IRecipesQuery
{
    public string? TitleSearchQuery { get; set; }

    private readonly List<string> _categories = [];
    public IEnumerable<string>? Categories => _categories.Count > 0 ? _categories : null;

    private readonly List<Ingridient> _ingredients = [];
    public IEnumerable<Ingridient>? Ingridients => _ingredients.Count > 0 ? _ingredients : null;
    public string? SortingParameter { get; set; }
    public bool? AscendingSorting { get; set; }
}