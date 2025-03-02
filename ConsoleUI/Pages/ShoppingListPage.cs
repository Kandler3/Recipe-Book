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

        var option = Console.Prompt(
            new SelectionPrompt<string>().AddChoices("Сохранить список в txt файл", "Назад")
        );

        switch (option)
        {
            case "Сохранить список в txt файл":
                OnExport();
                break;
            case "Назад": return;
        }
    }

    private void OnExport()
    {
        var filepath = new FilepathPrompt(Console, "Введите путь до файла", false, FileFormat.Txt).Ask();
        if (filepath == null) return;
        if (
            File.Exists(filepath)
            && !new ConfirmPrompt(Console, "Файл уже существует. Перезаписать?").Ask()
        ) return;

        try
        {
            Service.Export(ShoppingList, filepath);
            new MessagePage(Console, new SuccessText("Список сохранен\n")).Show();
        }
        catch (UnauthorizedAccessException)
        {
            new MessagePage(Console, new ErrorText("Нет доступа к файлу\n")).Show();
        }
        catch (IOException)
        {
            new ErrorPage(Console, "Ошибка при записи в файл").Show();
        }
    }
}