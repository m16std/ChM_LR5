using OxyPlot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;

namespace WpfApplication1
{
    public class methods
    {
        public IList<DataPoint> owo(IList<DataPoint> Points)
        {
            int n = 100;
            Points = new List<DataPoint>(n);
            for (double i = 0; i < n; i += 0.1)
            {
                var x = (double)i / (n - 1);
                Points.Add(new DataPoint(x, y(x)));
            }

            return Points;
        }
        public IList<DataPoint> tab(IList<DataPoint> Points, List<double> xi, List<double> yi)
        {
            Points = new List<DataPoint>();

            for (int i = 0; i < 5; i++)
                Points.Add(new DataPoint(xi[i], yi[i]));

            return Points;
        }

        public IList<DataPoint> Lagrange(IList<DataPoint> Points, List<double> x, List<double> y, int x0, int xn)
        {
            int n = x.Count;
            double Val, xi, P, znam;
            Points = new List<DataPoint>();
            for (xi = x0; xi < xn; xi += 0.1)
            {
                Val = 0;
                for (int i = 0; i < n; i++)
                { 
                    P = 1;
                    znam = 1;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                        {
                            P *= (xi - x[j]) / (x[i] - x[j]);
                            znam *= (x[i] - x[j]);
                        }
                        else continue;
                    }
                    Val += P * y[i];
                }
                Points.Add(new DataPoint(xi, Val));
            }
            return Points;
        }

        public IList<DataPoint> Newton(IList<DataPoint> Points, List<double> x, List<double> y, int x0, int xn)
        {
            int n = x.Count;
            double xi;
            Points = new List<DataPoint>();
            for (xi = x0; xi < xn; xi += 0.1)
            {
                double Val = y[0];
                for (int i = 1; i < n; ++i)
                {

                    double F = 0;
                    for (int j = 0; j <= i; ++j)
                    {
                        double den = 1;

                        for (int k = 0; k <= i; ++k)
                            if (k != j)
                                den *= (x[j] - x[k]);

                        F += y[j] / den;
                    }


                    for (int k = 0; k < i; ++k)
                        F *= (xi - x[k]);

                    Val += F;
                }
                Points.Add(new DataPoint(xi, Val-0.1));
            }
            return Points;
        }

        private static double y(double x)
        {
            return Math.Sin(3 / x) * Math.Sin(5 / (1 - x));
        }
    }

}
