namespace Models;

public class Recipe
{
    public string Title { get; set; }
    public string Category { get; set; }
    
    public List<Ingridient> Ingredients { get; set; }
}