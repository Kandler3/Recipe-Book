using System.Text;
using Contracts.Interfaces;
using Models;

namespace Serializers;

public class TxtRecipeSerializer : IRecipeSerializer, ISingleRecipeSerializer
{
    private TxtIngredientSerializer IngredientSerializer { get; } = new();

    private Dictionary<Field, string> FieldNames { get; } = new
    ([
        new KeyValuePair<Field, string>(Field.Title, "Название рецепта:"),
        new KeyValuePair<Field, string>(Field.Category, "Категория:"),
        new KeyValuePair<Field, string>(Field.Ingredients, "Ингредиенты:"),
        new KeyValuePair<Field, string>(Field.Instruction, "Инструкция:"),
        new KeyValuePair<Field, string>(Field.Images, "Изображения:")
    ]);

    public IEnumerable<Recipe> FileDeserialize(string filepath)
    {
        List<Recipe> res = [];
        using var reader = new StreamReader(filepath);
        StringBuilder sb = new();
        var line = reader.ReadLine();
        while (line != null)
        {
            if (line == "")
            {
                if (sb.Length > 0)
                {
                    res.Add(DeserializeRecipe(sb.ToString()));
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(line + "\n");
            }

            line = reader.ReadLine();
        }

        if (sb.Length > 0)
            res.Add(DeserializeRecipe(sb.ToString()));

        return res;
    }

    public void FileSerialize(IEnumerable<Recipe> recipes, string filepath)
    {
        using var writer = new StreamWriter(filepath);
        foreach (var recipe in recipes)
        {
            writer.WriteLine(SerializeRecipe(recipe));
            writer.WriteLine();
        }
    }

    public Recipe DeserializeRecipe(string recipeString)
    {
        string? title = null;
        string? category = null;
        List<Ingredient> ingredients = [];
        List<string> instruction = [];
        List<string> images = [];

        var currentField = Field.Title;
        foreach (var line in recipeString.Split("\n", StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith('-'))
            {
                switch (currentField)
                {
                    case Field.Ingredients:
                        ingredients.Add(IngredientSerializer.Deserialize(line.TrimStart('-').Trim()));
                        break;

                    case Field.Instruction:
                        instruction.Add(line.TrimStart('-').Trim());
                        break;

                    case Field.Images:
                        images.Add(line.TrimStart('-').Trim());
                        break;

                    default: throw new FormatException("Invalid file format");
                }

                continue;
            }

            if (title != null)
                do
                {
                    currentField++;
                    if (currentField > Field.Images)
                        throw new FormatException("Invalid file format");
                } while (!line.StartsWith(FieldNames[currentField]));

            else if (!line.StartsWith(FieldNames[currentField]))
                throw new FormatException("Invalid file format");

            try
            {
                switch (currentField)
                {
                    case Field.Title:
                        title = GetContent(line);
                        break;
                    case Field.Category:
                        category = GetContent(line);
                        break;
                    default:
                        if (!string.Equals(line, FieldNames[currentField]))
                            throw new FormatException("Invalid file format");
                        break;
                }
            }

            catch (ArgumentOutOfRangeException e)
            {
                throw new FormatException("Invalid file format", e);
            }
        }

        if (title == null) throw new FormatException("Recipe must include title");

        return new Recipe(
            title,
            category,
            ingredients.Count != 0 ? ingredients : null,
            instruction.Count != 0 ? instruction : null,
            images.Count != 0 ? images : null
        );
    }

    private static string GetContent(string line)
    {
        var res = line[(line.IndexOf(':') + 1)..].Trim();
        if (res == "")
            throw new FormatException("Invalid file format");

        return res;
    }

    private string SerializeRecipe(Recipe recipe)
    {
        StringBuilder sb = new();
        sb.Append($"{FieldNames[Field.Title]} {recipe.Title}\n");
        if (recipe.Category != null)
            sb.Append($"{FieldNames[Field.Category]} {recipe.Category}\n");

        if (recipe.Ingredients != null)
        {
            sb.Append($"{FieldNames[Field.Ingredients]}\n");
            foreach (var ingredient in recipe.Ingredients)
                sb.Append($"- {IngredientSerializer.Serialize(ingredient)}\n");
        }

        if (recipe.Instructions != null)
        {
            sb.Append($"{FieldNames[Field.Instruction]}\n");
            foreach (var instruction in recipe.Instructions)
                sb.Append($"- {instruction}\n");
        }

        if (recipe.Images != null)
        {
            sb.Append($"{FieldNames[Field.Images]}\n");
            foreach (var image in recipe.Images)
                sb.Append($"- {image}\n");
        }

        return sb.ToString();
    }

    private enum Field
    {
        Title,
        Category,
        Ingredients,
        Instruction,
        Images
    }
}