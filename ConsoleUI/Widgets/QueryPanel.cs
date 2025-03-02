using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class QueryPanel : IRenderable
{
    public QueryPanel(RecipesQuery query)
    {
        var rows = Utils.GenerateQueryRows(query);
        if (rows.Count == 0)
        {
            QueryRows = new Rows(new Text("Тут отобразятся применяемые параметры"));
            return;
        }

        QueryRows = new Rows(rows.Select(r => new Text(r)));
    }

    private Rows QueryRows { get; }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)QueryRows).Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return ((IRenderable)QueryRows).Render(options, maxWidth);
    }
}