using Contracts;
using Spectre.Console;

namespace ConsoleUI;

public class ExportMenu(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    public void Show()
    {
        Console.Clear();
        FileFormat format = new SelectFormatPrompt(Console, "Выберите формат импортируемого файла").Ask();
        string filepath = new FilepathPrompt(Console, "Введите путь до файла", false, format).Ask();
        Service.Export(filepath, format);
    }
}