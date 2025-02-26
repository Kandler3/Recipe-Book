using Models;

namespace Interfaces;

public interface IRecipesQuery
{
    public string? TitleSearchQuery { get; }
    public IEnumerable<string>? Categories { get; }
    public IEnumerable<Ingridient>? Ingridients { get; }
    public string? SortingParameter { get; }
    public bool? AscendingSorting { get; }
}