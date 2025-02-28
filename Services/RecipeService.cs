using System.Net.Mime;
using Contracts;
using Models;

namespace Services;

public class RecipeService : IRecipeService
{
    private List<Recipe> Recipes { get; set; } = [];
    private IRecipeSerializer TxtSerializer { get; }
    private IRecipeSerializer JsonSerializer { get; }
    private IRecipeSerializer CsvSerializer { get; }

    public RecipeService(
        IRecipeSerializer txtSerializer, 
        IRecipeSerializer jsonSerializer,
        IRecipeSerializer csvSerializer
    )
    {
        TxtSerializer = txtSerializer;
        JsonSerializer = jsonSerializer;
        CsvSerializer = csvSerializer;
    }
    public IList<Recipe> GetRecipes(IRecipesQuery query)
    {
        IEnumerable<Recipe> res = Recipes.Where(
            recipe =>
            {
                bool titleCondition = query.TitleSearchQuery == null || recipe.Title.Contains(query.TitleSearchQuery);
                bool categoryCondition = query.Categories == null || query.Categories.Contains(recipe.Category);
                bool ingredientsCondition = 
                    query.Ingredients == null 
                    || (recipe.Ingredients != null 
                        && recipe.Ingredients.Any(
                            ingredient => 
                                query.Ingredients.Any(
                                    name => name.Equals(ingredient.Name)
                                    )
                                )
                        );
                return titleCondition && categoryCondition && ingredientsCondition;
            }
        );

        if (query.SortingParameter != null)
        {
            var keySelector = (Recipe recipe) => query.SortingParameter switch
            {
                RecipeSortingParameter.Title => recipe.Title,
                RecipeSortingParameter.Category => recipe.Category,
                _ => throw new ArgumentException("Invalid sorting parameter"),
            };

            res = query.AscendingSorting == true ? res.OrderBy(keySelector) : res.OrderByDescending(keySelector);
        }
        
        return res.ToList();
    }

    public IEnumerable<string> GetCategories() => Recipes.Select(recipe => recipe.Category ?? "Без категории").Distinct();
    
    public IEnumerable<string> GetIngredients() =>
        Recipes.Where(recipe => recipe.Ingredients != null)
            .SelectMany(
                recipe => 
                    recipe.Ingredients
                        .Select(ingredient => ingredient.Name)
            )
            .Distinct();

    public void AddRecipe(Recipe recipe)
    {
        Recipes.Add(recipe);
    }

    public void DeleteRecipe(Recipe recipe)
    {
        Recipes.Remove(recipe);
    }

    public void AddRecipeImage(Recipe recipe, string imagePath)
    {
        recipe.Images ??= [];
        recipe.Images.Add(imagePath);
    }

    public void Import(string filepath, FileFormat format)
    {
        Recipes.AddRange(
            format switch
            {
                FileFormat.Txt => TxtSerializer.Deserialize(filepath),
                FileFormat.Json => JsonSerializer.Deserialize(filepath),
                FileFormat.Csv => CsvSerializer.Deserialize(filepath),
                _ => throw new ArgumentException("Invalid file format"),
            }
        );
    }

    public void Export(string filepath, FileFormat format, IRecipesQuery? query = null)
    {
        if (query != null)
            Recipes = GetRecipes(query).ToList();
            
        switch (format)
        {
            case FileFormat.Txt: TxtSerializer.Serialize(Recipes, filepath); break;
            case FileFormat.Csv: CsvSerializer.Serialize(Recipes, filepath); break;
            case FileFormat.Json: JsonSerializer.Serialize(Recipes, filepath); break;
            default: throw new ArgumentException("Invalid file format");
        }
    }
}