﻿using System.Globalization;
using Contracts.Interfaces;
using CsvHelper;
using Models;

namespace Serializers;

public class CsvRecipeSerializer : IRecipeSerializer
{
    public void FileSerialize(IEnumerable<Recipe> recipes, string outputFilepath)
    {
        using StreamWriter streamWriter = new(outputFilepath);
        using CsvWriter writer = new(streamWriter, CultureInfo.InvariantCulture);
        writer.Context.RegisterClassMap<CsvRecipeMap>();
        writer.WriteRecords(recipes);
    }

    public IEnumerable<Recipe> FileDeserialize(string inputFilepath)
    {
        using StreamReader streamReader = new(inputFilepath);
        using CsvReader reader = new(streamReader, CultureInfo.InvariantCulture);
        reader.Context.RegisterClassMap<CsvRecipeMap>();
        try
        {
            return reader.GetRecords<Recipe>().ToList();
        }
        catch (CsvHelperException e)
        {
            throw new FormatException("Unable to parse the CSV file.", e);
        }
    }
}