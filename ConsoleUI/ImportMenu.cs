using ConsoleUI.Prompts;
using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class ImportMenu(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    public void Show()
    {
        Console.Clear();
        FileFormat format = new SelectFormatPrompt(console, "Выберите формат импортируемого файла").Ask();
        string filepath = new FilepathPrompt(console, "Введите путь до файла", true, format).Ask();
        service.Import(filepath, format);
    }
}