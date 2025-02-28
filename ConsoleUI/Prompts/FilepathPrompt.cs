using Contracts;
using Spectre.Console;

namespace ConsoleUI.Prompts;

public class FilepathPrompt(IAnsiConsole console, string message, bool existingFile, FileFormat format)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;
    private bool ExistingFile { get; } = existingFile;
    private FileFormat Format { get; } = format;

    public string Ask()
    {
        Console.Clear();
        string result = Console.Prompt(
            new TextPrompt<string>(Message)
        );
        
        while (!IsValidPath(result, out string error))
        {
            Console.Clear();
            result = Console.Prompt(
                new TextPrompt<string>($"{error}\n{Message}")
            );
        }
        
        return result;
    }

    private bool IsValidPath(string path, out string message)
    {
        if (!path.EndsWith(Utils.GetFileFormatPostfix(Format)))
        {
            message = "Неверный формат файла";
            return false;
        }
        
        string? directory = Path.GetDirectoryName(path);
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