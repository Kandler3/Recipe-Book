/*
 * Ковальчук Артём Игоревич
 * БПИ 2410-2
 * Вариант 3
 */

using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

/// <summary>
/// Виджет для отображения подсказки в виде текста тёмно-серого цвета.
/// </summary>
/// <param name="message">Сообщение подсказки.</param>
public class HintText(string message) : IRenderable
{
    private Text RenderText { get; } = new(message, new Style(ConsoleColor.DarkGray));

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)RenderText).Render(options, maxWidth);
    }
}