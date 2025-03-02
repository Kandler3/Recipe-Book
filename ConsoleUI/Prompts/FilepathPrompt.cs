using ConsoleUI.Widgets;
using Contracts.Enums;
using Spectre.Console;

namespace ConsoleUI.Prompts;

public class FilepathPrompt(IAnsiConsole console, string message, bool existingFile, FileFormat? format = null)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;
    private bool ExistingFile { get; } = existingFile;
    private FileFormat? Format { get; } = format;

    public string? Ask()
    {
        Console.Clear();
        Console.Write(new HintText("Чтобы вернуться введите пустую строку\n"));
        var result = Console.Prompt(
            new TextPrompt<string>(Message).AllowEmpty()
        );

        if (result == "") return null;

        while (!IsValidPath(result, out var error))
        {
            Console.Clear();
            Console.Write(new HintText("Чтобы вернуться введите пустую строку\n"));
            Console.Write(new ErrorText(error + "\n"));
            result = Console.Prompt(
                new TextPrompt<string>(Message).AllowEmpty()
            );

            if (result == "") return null;
        }

        return result;
    }

    private bool IsValidPath(string path, out string message)
    {
        if (Format != null && !path.EndsWith(Utils.GetFileFormatPostfix(Format.Value)))
        {
            message = "Неверный формат файла";
            return false;
        }

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            message = $"Директория {directory} не найдена";
            return false;
        }

        if (ExistingFile && !File.Exists(path))
        {
            message = "Файл не существует";
            return false;
        }

        message = "";
        return true;
    }
}