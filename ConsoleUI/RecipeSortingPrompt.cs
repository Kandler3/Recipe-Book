﻿using ConsoleUI.Prompts;
using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class RecipeSortingPrompt(IAnsiConsole console, RecipesQuery query)
{
    private IAnsiConsole Console { get; } = console;
    private RecipesQuery Query { get; } = query;

    public void Show()
    {
        Console.Clear();
        
        string choice = Console.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле для сортировки")
                .AddChoices("Название", "Категория", "Сбросить", "Назад")
        );

        switch (choice)
        {
            case "Название": Query.SortingParameter = RecipeSortingParameter.Title; break;
            case "Категория": Query.SortingParameter = RecipeSortingParameter.Category; break;
            case "Сбросить": ShowResetPrompt(); return;
            case "Назад": return;
            default: return;
        }

        Query.AscendingSorting = new ConfirmPrompt(Console, "Сортировать по возрастанию?").Ask();
    }

    private void ShowResetPrompt()
    {
        Console.Clear();
        if (new ConfirmPrompt(Console, "Сбросить настройки сортировки?").Ask())
            Query.ResetSorting();
    }
}