/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

/// <summary>
/// Виджет для отображения сообщения об ошибке в виде красного текста.
/// </summary>
/// <param name="message">Сообщение об ошибке.</param>
public class ErrorText(string message) : IRenderable
{
    private Text RenderText { get; } = new(message, new Style(ConsoleColor.Red));
    
    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Render(options, maxWidth);
    }
}