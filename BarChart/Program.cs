using System.Globalization;


public class BarChart
{
    // Campos privados (ou "atributos")
    private double[] y_data;
    private string[] labels;

    private double height;
    private double width;

    private double font_size = 500 / 50;
    private double padding;

    private double wc;
    private double hc;

    private string svg;

    private string color = "#c8c8c8";

    private string font_family = "font-family=\"Helvetica\"";

    public BarChart(double[] y_data, string[] labels, double height, double width)
    {
        this.height = height;
        this.width = width;

        this.y_data = y_data;
        this.labels = labels;

        this.padding = y_data.Max().ToString().Length * 2 * font_size;

        this.wc = width - (padding * 2);
        this.hc = height - (padding * 2);

       
        svg = $"<svg width=\"{width}\" height=\"{height}\" xmlns=\"http://www.w3.org/2000/svg\" style=\"background-color: white\">\n";
    }

    double Plot_Y(double yc, double maxy)
    {
        return hc - (yc / maxy * hc) + padding;
    }

    public static List<double> arange(double end, double ratio)
    {
        double n = (end / ratio) + 1;

        List<double> values = new List<double>();

        for (double i = 1; i < n; i++)
        {
            double an = (i - 1) * ratio;
            values.Add(an);
        }

        values.Add(end);

        return values;
    }
    public void Draw_bars()
    {
        double perfect_width = wc / y_data.Length;
        double adjust = perfect_width * 0.25;

        bool color_bool = true;

        for (int i = 0; i < y_data.Length; i++)
        {
            double xp = padding + perfect_width * i;
            double yp = Math.Round(Plot_Y(y_data[i], y_data.Max()));

            string rect_color = color_bool ? "#459cff" : "#cecece";

            double initial_x = xp + adjust;

            double rect_width = perfect_width - adjust * 2;

            double text_x = initial_x + (rect_width - (labels.Length/2) * font_size);

            string rect = $"<rect x=\"{initial_x}\" y=\"{yp}\" width=\"{rect_width}\" height=\"{height - padding - yp}\"  fill=\"{rect_color}\"/>\n";
            svg += rect;
            // xp + adjust
            string text = $"<text x=\"{text_x}\" y=\"{(height - padding) + font_size * 2}\" font-size=\"{font_size}\" fill=\"{color}\" {font_family}>{labels[i]}</text>\n";
            svg += text;

            color_bool = !color_bool;
        }
    }

    public void Draw_grid(double length, int n_guides, char axis='h')
    {
        double jump = length / n_guides;

        for (int i = 0; i < n_guides + 1; i++)
        {
            double ix = padding;
            double iy = padding + (i * jump);

            double fx = wc + padding;
            double fy = padding + (i * jump);

            if (axis == 'v')
            {
                double temp_x = ix;

                ix = iy;
                iy = temp_x;

                fx = fy;
                fy = hc + padding;

            }

            string path = $"<path d=\"M {ix} {iy} L {fx} {fy}\" stroke=\"{color}\" stroke-width=\"0.5\"/>\n";
            svg += path;

        }

    }

    public void Draw_Vertical(int ticks = 1)
    {
        double Base = y_data.Min();
        double top = y_data.Max();

        List<double> values = arange(top, ticks);
        int size = values.Count;
 
        double jump = hc / (size - 1);

        Draw_grid(hc, size - 1);

        for (int i = 0; i < size; i++)
        {
            double x = padding - (font_size * (values[i].ToString().Length));
            double y = (hc + padding) - (i * jump);
            string text = $"<text x=\"{x}\" y=\"{y}\" font-size=\"{font_size}\" fill=\"{color}\" {font_family}>{values[i]}</text>\n";
            svg += text;

        }

    }

    public void Launch(string? path, int ticks = 1)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        if (y_data.Length != labels.Length)
        {
            throw new Exception("Erro! os dados possuem valores não representáveis");
        }
        Draw_grid(wc, labels.Length, 'v');
        Draw_Vertical(ticks);
        Draw_bars();

        svg += "</svg>";

        if (path != null)
        {
            File.WriteAllText(path, svg);
        }
    }

}
