using ConsoleUI.Prompts;
using ConsoleUI.Widgets;
using Contracts.Enums;
using Contracts.Interfaces;
using Models;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ShoppingListPage(IAnsiConsole console, IShoppingListService service, IEnumerable<Ingredient> shoppingList)
{
    private IAnsiConsole Console { get; } = console;
    private IShoppingListService Service { get; } = service;
    private IEnumerable<Ingredient> ShoppingList { get; } = shoppingList;

    public void Show()
    {
        Console.Clear();
        Console.Write(new ShoppingListPanel(ShoppingList));

        string option = Console.Prompt(
            new SelectionPrompt<string>().AddChoices("Сохранить список в файл", "Назад")
        );

        switch (option)
        {
            case "Сохранить список в файл": OnExport(); break;
            case "Назад": return;
        }
    }

    private void OnExport()
    {
        string filepath = new FilepathPrompt(Console, "Введите путь до файла", false, FileFormat.Txt).Ask();
        try
        {
            Service.Export(ShoppingList, filepath);
        }
        catch (IOException)
        {
            new ErrorPage(Console, "Ошибка при записи в файл").Show();
        }
    }
}