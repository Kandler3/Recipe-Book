using System.Net.Mime;
using Contracts;
using Contracts.Enums;
using Contracts.Interfaces;
using Models;

namespace Services;

public class RecipeService : IRecipeService
{
    private List<Recipe> Recipes { get; set; } = [];
    private IRecipeSerializer TxtSerializer { get; }
    private IRecipeSerializer JsonSerializer { get; }
    private IRecipeSerializer CsvSerializer { get; }
    private YandexDiskService DiskService { get; }

    public RecipeService(
        IRecipeSerializer txtSerializer, 
        IRecipeSerializer jsonSerializer,
        IRecipeSerializer csvSerializer,
        YandexDiskService yandexDiskService
    )
    {
        TxtSerializer = txtSerializer;
        JsonSerializer = jsonSerializer;
        CsvSerializer = csvSerializer;
        DiskService = yandexDiskService;
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
                    recipe.Ingredients?
                        .Select(ingredient => ingredient.Name) ?? []
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
                FileFormat.Txt => TxtSerializer.FileDeserialize(filepath),
                FileFormat.Json => JsonSerializer.FileDeserialize(filepath),
                FileFormat.Csv => CsvSerializer.FileDeserialize(filepath),
                _ => throw new ArgumentException("Invalid file format"),
            }
        );
    }

    public void Import(string filepath, FileFormat format, bool local)
    {
        if (local)
        {
            Import(filepath, format);
            return;
        }
        DiskService.DownloadFile(".temp", filepath);
        Import(".temp", format);
        File.Delete(".temp");
    }

    public void Export(string filepath, FileFormat format, IRecipesQuery? query = null)
    {
        if (query != null)
            Recipes = GetRecipes(query).ToList();
            
        switch (format)
        {
            case FileFormat.Txt: TxtSerializer.FileSerialize(Recipes, filepath); break;
            case FileFormat.Csv: CsvSerializer.FileSerialize(Recipes, filepath); break;
            case FileFormat.Json: JsonSerializer.FileSerialize(Recipes, filepath); break;
            default: throw new ArgumentException("Invalid file format");
        }
    }

    public void Export(string filepath, FileFormat format, IRecipesQuery? query, bool local)
    {
        if (local)
        {
            Export(filepath, format, query);
            return;
        }
        
        Export(".temp", format, query);
        DiskService.UploadFile(".temp", filepath);
        File.Delete(".temp");
    }
}