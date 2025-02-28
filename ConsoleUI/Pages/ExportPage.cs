using ConsoleUI.Prompts;
using Contracts;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ExportPage(IAnsiConsole console, IRecipeService service)
{
    private IAnsiConsole Console { get; } = console;
    private IRecipeService Service { get; } = service;

    public void Show()
    {
        Console.Clear();
        FileFormat format = new SelectFormatPrompt(Console, "Выберите формат импортируемого файла").Ask();
        string filepath = new FilepathPrompt(Console, "Введите путь до файла", false, format).Ask();
        try
        {
            Service.Export(filepath, format);
        }
        catch (IOException)
        {
            new ErrorPage(Console, "Ошибка при записи в файл").Show();
        }
    }
}