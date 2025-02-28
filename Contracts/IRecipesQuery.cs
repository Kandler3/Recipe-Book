namespace Contracts;

public interface IRecipesQuery
{
    public string? TitleSearchQuery { get; }
    public IEnumerable<string>? Categories { get; }
    public IEnumerable<string>? Ingredients { get; }
    public RecipeSortingParameter? SortingParameter { get; }
    public bool? AscendingSorting { get; }
}