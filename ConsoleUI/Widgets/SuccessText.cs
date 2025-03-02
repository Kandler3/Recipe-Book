/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

/// <summary>
/// Виджет для отображения успешного сообщения зеленым текстом.
/// </summary>
/// <param name="message">Сообщение для отображения.</param>
public class SuccessText(string message) : IRenderable
{
    private Text RenderText { get; } = new(message, new Style(ConsoleColor.Green));

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Render(options, maxWidth);
    }
}