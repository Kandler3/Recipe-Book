/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using ConsoleUI.Widgets;
using Contracts.Enums;
using Spectre.Console;

namespace ConsoleUI.Prompts;

/// <summary>
/// Класс запроса пути файла. Отображает запрос на ввод пути, проверяет корректность пути по заданным критериям.
/// </summary>
/// <param name="console">Интерфейс консоли для ввода и вывода.</param>
/// <param name="message">Сообщение, отображаемое пользователю при запросе пути.</param>
/// <param name="existingFile">Если true, проверяется существование файла по указанному пути.</param>
/// <param name="format">Необязательный формат файла для проверки расширения.</param>
public class FilepathPrompt(IAnsiConsole console, string message, bool existingFile, FileFormat? format = null)
{
    private IAnsiConsole Console { get; } = console;
    private string Message { get; } = message;
    private bool ExistingFile { get; } = existingFile;
    private FileFormat? Format { get; } = format;

    /// <summary>
    /// Выводит запрос на ввод пути файла, проверяет корректность введенного пути и возвращает его.
    /// Если пользователь вводит пустую строку, возвращается null.
    /// </summary>
    /// <returns>Введенный путь файла или null, если ввод отменен.</returns>
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

    /// <summary>
    /// Проверяет корректность указанного пути файла.
    /// </summary>
    /// <param name="path">Путь файла для проверки.</param>
    /// <param name="message">Сообщение об ошибке, если путь некорректен.</param>
    /// <returns>True, если путь корректен; иначе false.</returns>
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
