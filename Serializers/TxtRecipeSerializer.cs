using Contracts;
using Contracts.Interfaces;
using Models;

namespace Serializers;

public class TxtRecipeSerializer : IRecipeSerializer
{
    private TxtIngredientSerializer IngredientSerializer { get; } = new();
    private enum Field
    {
        Title,
        Category,
        Ingredients,
        Instruction,
        Images
    }

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
        Recipe? recipe = DeserializeRecipe(reader);
        while (recipe != null)
        {
            res.Add(recipe);
            recipe = DeserializeRecipe(reader);
        }
        return res;
    }

    public void FileSerialize(IEnumerable<Recipe> recipes, string filepath)
    {
        using var writer = new StreamWriter(filepath);
        foreach (Recipe recipe in recipes)
        {
            SerializeRecipe(recipe, writer);
            writer.WriteLine();
        }
    }

    private Recipe? DeserializeRecipe(StreamReader reader)
    {
        string? title = null;
        string? category = null;
        List<Ingredient> ingredients = [];
        List<string> instruction = [];
        List<string> images = [];
        
        Field currentField = Field.Title;
        
        string? line;
        do
        {
            line = reader.ReadLine()?.Trim();
        } while (line == "");
        
        if (line == null)
        {
            return null;
        }
        
        while (!string.IsNullOrEmpty(line))
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
                
                line = reader.ReadLine()?.Trim();
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
            
            line = reader.ReadLine()?.Trim();
        }

        if (title == null)
        {
            throw new FormatException("Recipe must include title");
        }
        
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
        string res = line[(line.IndexOf(':') + 1)..].Trim();
        if (res == "")
            throw new FormatException("Invalid file format");
        
        return res;
    }

    private void SerializeRecipe(Recipe recipe, StreamWriter writer)
    {
        writer.WriteLine($"{FieldNames[Field.Title]} {recipe.Title}");
        if (recipe.Category != null)
            writer.WriteLine($"{FieldNames[Field.Category]} {recipe.Category}");

        if (recipe.Ingredients != null)
        {
            writer.WriteLine($"{FieldNames[Field.Ingredients]}");
            foreach (Ingredient ingredient in recipe.Ingredients)
                writer.WriteLine($"- {IngredientSerializer.Serialize(ingredient)}");
        }

        if (recipe.Instructions != null)
        {
            writer.WriteLine($"{FieldNames[Field.Instruction]}");
            foreach (string instruction in recipe.Instructions)
                writer.WriteLine($"- {instruction}");
        }
        if (recipe.Images != null)
        {
            writer.WriteLine($"{FieldNames[Field.Images]}");
            foreach (string image in recipe.Images)
                writer.WriteLine($"- {image}");
        }
    }
}