﻿namespace Models;

public class Ingredient(string name, int? quantity = null, string? measurement = null)
{
    public string Name { get; } = name;
    public int? Quantity { get; set; } = quantity;
    public string? Measurement { get; } = measurement;
}