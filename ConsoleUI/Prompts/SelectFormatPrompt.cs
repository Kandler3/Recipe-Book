using Contracts;
using Contracts.Enums;
using Spectre.Console;

namespace ConsoleUI.Prompts;

public class SelectFormatPrompt(IAnsiConsole console, string message)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;

    public FileFormat Ask()
    {
        Console.Clear();
        return Console.Prompt(
            new SelectionPrompt<FileFormat>().AddChoices(
                FileFormat.Txt,
                FileFormat.Csv,
                FileFormat.Json
            )
        );
    }
}