using ConsoleUI.Prompts;
using Contracts;
using Contracts.Enums;
using Contracts.Interfaces;
using Spectre.Console;

namespace ConsoleUI.Pages;

public class ImportPage(IAnsiConsole console, IRecipeService service)
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
            new ErrorPage(Console, "Не удалось десериализовать данные из файла").Show();
        }
        catch (IOException)
        {
            new ErrorPage(Console, "Ошибка при чтении файла").Show();
        }
    }
}