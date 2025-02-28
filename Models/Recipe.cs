namespace Models;

public class Recipe(string title, string? category = null, List<Ingredient>? ingredients = null, List<string>? instructions = null)
{
    public string Title { get; set; } = title;
    public string? Category { get; set; } = category;
    public List<Ingredient>? Ingredients { get; set; } =  ingredients;
    public List<string>? Instructions { get; set; } = instructions;
}