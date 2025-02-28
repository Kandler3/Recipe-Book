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
        FileFormat format = new SelectFormatPrompt(Console, "Выберите формат импортируемого файла").Ask();
        string filepath = new FilepathPrompt(Console, "Введите путь до файла", true, format).Ask();
        try
        {
            Service.Import(filepath, format);
        }
        catch (FormatException)
        {
            new ErrorMessage(Console, "Не удалось десериализовать данные из файла").Show();
        }
        catch (IOException)
        {
            new ErrorMessage(Console, "Ошибка при чтении файла").Show();
        }
    }
}