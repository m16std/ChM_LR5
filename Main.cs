
namespace WpfApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using OxyPlot;
    using OxyPlot.Series;

    public class Main
    {
        List<double> Xi = new List<double>() { 0.134, 0.561, 1.341, 2.291, 6.913 };
        List<double> Yi = new List<double>() { 2.156, 3.348, 3.611, 4.112, 4.171 };

        methods metod = new methods();
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }
        public IList<DataPoint> Lagrange { get; private set; }
        public IList<DataPoint> Newton { get; private set; }

        public Main()
        {
            Lagrange = metod.Lagrange(Lagrange, Xi, Yi, -1, 7);
            Newton = metod.Newton(Newton, Xi, Yi, -1, 7);
            Points = metod.tab(Points, Xi, Yi);
        }

    }

}
//<oxy:PlotView Model="{Binding MyModel}"/>