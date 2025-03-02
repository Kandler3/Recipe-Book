/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Contracts.Enums;
using Contracts.Interfaces;
using Models;

namespace Services;

/// <summary>
/// Сервис для работы с рецептами, реализующий импорт, экспорт, поиск и управление рецептами.
/// </summary>
public class RecipeService : IRecipeService
{
    /// <summary>
    /// Конструктор сервиса рецептов.
    /// </summary>
    /// <param name="txtSerializer">Сериализатор рецептов в формате TXT.</param>
    /// <param name="jsonSerializer">Сериализатор рецептов в формате JSON.</param>
    /// <param name="csvSerializer">Сериализатор рецептов в формате CSV.</param>
    /// <param name="yandexDiskService">Сервис для работы с Яндекс.Диск.</param>
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

    /// <summary>
    /// Локальное хранилище рецептов.
    /// </summary>
    private List<Recipe> Recipes { get; set; } = [];
    private IRecipeSerializer TxtSerializer { get; }
    private IRecipeSerializer JsonSerializer { get; }
    private IRecipeSerializer CsvSerializer { get; }
    private YandexDiskService DiskService { get; }

    /// <summary>
    /// Возвращает список рецептов, удовлетворяющих заданному запросу.
    /// </summary>
    /// <param name="query">Параметры запроса.</param>
    /// <returns>Список найденных рецептов.</returns>
    public IList<Recipe> GetRecipes(IRecipesQuery query)
    {
        var res = Recipes.Where(
            recipe =>
            {
                var titleCondition = query.TitleSearchQuery == null || recipe.Title.Contains(query.TitleSearchQuery);
                var categoryCondition = query.Categories == null || query.Categories.Contains(recipe.Category);
                var ingredientsCondition =
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
                _ => throw new ArgumentException("Invalid sorting parameter")
            };

            res = query.AscendingSorting == true ? res.OrderBy(keySelector) : res.OrderByDescending(keySelector);
        }

        return res.ToList();
    }

    /// <summary>
    /// Возвращает список уникальных категорий рецептов.
    /// </summary>
    /// <returns>Список категорий.</returns>
    public IEnumerable<string> GetCategories()
    {
        return Recipes.Select(recipe => recipe.Category ?? "Без категории").Distinct();
    }

    /// <summary>
    /// Возвращает список уникальных ингредиентов, используемых в рецептах.
    /// </summary>
    /// <returns>Список ингредиентов.</returns>
    public IEnumerable<string> GetIngredients()
    {
        return Recipes.Where(recipe => recipe.Ingredients != null)
            .SelectMany(
                recipe =>
                    recipe.Ingredients?
                        .Select(ingredient => ingredient.Name) ?? []
            )
            .Distinct();
    }

    /// <summary>
    /// Добавляет новый рецепт.
    /// </summary>
    /// <param name="recipe">Рецепт для добавления.</param>
    public void AddRecipe(Recipe recipe)
    {
        Recipes.Add(recipe);
    }

    /// <summary>
    /// Удаляет указанный рецепт.
    /// </summary>
    /// <param name="recipe">Рецепт для удаления.</param>
    public void DeleteRecipe(Recipe recipe)
    {
        Recipes.Remove(recipe);
    }

    /// <summary>
    /// Добавляет изображение к рецепту.
    /// </summary>
    /// <param name="recipe">Рецепт, к которому добавляется изображение.</param>
    /// <param name="imagePath">Путь к изображению.</param>
    public void AddRecipeImage(Recipe recipe, string imagePath)
    {
        recipe.Images ??= [];
        recipe.Images.Add(imagePath);
    }

    /// <summary>
    /// Импортирует рецепты из файла. При local = false используется удалённое хранилище.
    /// </summary>
    /// <param name="filepath">Путь к файлу импорта.</param>
    /// <param name="format">Формат файла импорта.</param>
    /// <param name="local">Флаг локального импорта.</param>
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

    /// <summary>
    /// Экспортирует рецепты в файл. При local = false используется удалённое хранилище.
    /// </summary>
    /// <param name="filepath">Путь к файлу экспорта.</param>
    /// <param name="format">Формат файла экспорта.</param>
    /// <param name="query">Параметры запроса для фильтрации рецептов (опционально).</param>
    /// <param name="local">Флаг локального экспорта.</param>
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

    /// <summary>
    /// Импортирует рецепты из файла с заданным форматом.
    /// </summary>
    /// <param name="filepath">Путь к файлу импорта.</param>
    /// <param name="format">Формат файла импорта.</param>
    public void Import(string filepath, FileFormat format)
    {
        Recipes.AddRange(
            format switch
            {
                FileFormat.Txt => TxtSerializer.FileDeserialize(filepath),
                FileFormat.Json => JsonSerializer.FileDeserialize(filepath),
                FileFormat.Csv => CsvSerializer.FileDeserialize(filepath),
                _ => throw new ArgumentException("Invalid file format")
            }
        );
    }

    /// <summary>
    /// Экспортирует рецепты в файл с заданным форматом.
    /// </summary>
    /// <param name="filepath">Путь к файлу экспорта.</param>
    /// <param name="format">Формат файла экспорта.</param>
    /// <param name="query">Параметры запроса для фильтрации рецептов (опционально).</param>
    public void Export(string filepath, FileFormat format, IRecipesQuery? query = null)
    {
        if (query != null)
            Recipes = GetRecipes(query).ToList();

        switch (format)
        {
            case FileFormat.Txt:
                TxtSerializer.FileSerialize(Recipes, filepath);
                break;
            case FileFormat.Csv:
                CsvSerializer.FileSerialize(Recipes, filepath);
                break;
            case FileFormat.Json:
                JsonSerializer.FileSerialize(Recipes, filepath);
                break;
            default: throw new ArgumentException("Invalid file format");
        }
    }
}
