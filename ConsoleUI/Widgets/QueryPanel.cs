using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleUI.Widgets;

public class QueryPanel : IRenderable
{
    private Rows QueryRows { get; }

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

    public Measurement Measure(RenderOptions options, int maxWidth) => ((IRenderable)QueryRows).Measure(options, maxWidth);

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) => ((IRenderable)QueryRows).Render(options, maxWidth);
}