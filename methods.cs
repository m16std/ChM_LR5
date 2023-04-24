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
        public string log;
        private void add_text_to_log(string text, int endl)
        {
            log += text;
            log += " ";
            for (int i = 0; i < endl; i++)
                log += "\n";
        }
        public IList<DataPoint> tab(IList<DataPoint> Points, List<double> xi, List<double> yi)
        {
            Points = new List<DataPoint>();
            int n = xi.Count;
            for (int i = 0; i < n; i++)
                Points.Add(new DataPoint(xi[i], yi[i]));

            return Points;
        }
        public IList<DataPoint> Lagrange(IList<DataPoint> Points, List<double> x, List<double> y, double x0, double xn)
        {
            add_text_to_log("Метод Лагранжа", 1);
            int n = x.Count;
            double Val, xi, P, znam;
            Points = new List<DataPoint>();
            for (xi = x0; xi < xn; xi += 0.05)
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

            add_text_to_log("Коэффициенты знаменателя слагаемых:", 1);
            for (int i = 0; i < n - 1; i++)
            {
                add_text_to_log((x[n - 1] - x[i]).ToString(), 1);
            }
            add_text_to_log((x[n - 2] - x[n - 1]).ToString(), 2);
            return Points;
        }
        public IList<DataPoint> Newton(IList<DataPoint> Points, List<double> x, List<double> y, double x0, double xn)
        {
            add_text_to_log("Метод Ньютона", 1);
            int n = x.Count;
            double xi;
            Points = new List<DataPoint>();
            for (xi = x0; xi < xn; xi += 0.05) //общий цикл
            {
                double Val = y[0];
                for (int i = 1; i < n; ++i) //проход по всем точкам
                {
                    double F = 0;
                    for (int j = 0; j <= i; ++j) //проход по точкам до этой точки
                    {

                        double znam = 1;

                        for (int k = 0; k <= i; ++k)
                            if (k != j)
                                znam *= (x[j] - x[k]);

                        F += y[j] / znam;

                    }

                    for (int k = 0; k < i; ++k)
                        F *= (xi - x[k]);

                    Val += F;
                }
                Points.Add(new DataPoint(xi, Val));
            }

            add_text_to_log("Таблица разделенных разностей:", 1);
            List<List<double>> delta = new List<List<double>>(6);
            for (int i = 0; i <= n; i++)
            {
                delta.Add(new List<double>() { 0, 0, 0, 0, 0, 0 });
            }
            int _n = n - 1;
            for (int i = 0; i < n; ++i)
            {
                delta[0][i] = x[i];
                delta[1][i] = y[i];
            }
            for (int i = 2; i <= n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    delta[i][j] = (delta[i - 1][j + 1] - delta[i - 1][j]) / (delta[0][j + i - 1] - delta[0][j]);
                }
                _n--;
            }
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    add_text_to_log(delta[i][j].ToString("F3"), 0);
                }
                add_text_to_log("", 1);
            }
            add_text_to_log("", 1);
            return Points;
        }
        public IList<DataPoint> Line_spline(IList<DataPoint> Points, List<double> x, List<double> y)
        {
            add_text_to_log("Линейный сплайн", 1);
            int n = x.Count;
            double xi;
            Points = new List<DataPoint>();
            for (int i = 0; i < n - 1; ++i) //проход по всем точкам
            {
                for (xi = x[i]; xi < x[i + 1]; xi += 0.05) //проход по отрезку между точкой и следующей
                {
                    Points.Add(new DataPoint(xi, y[i] + (y[i + 1] - y[i]) / (x[i + 1] - x[i]) * (xi - x[i])));
                }

                double coef = (y[i + 1] - y[i]) / (x[i + 1] - x[i]);
                add_text_to_log(coef.ToString("F3"), 1);
            }
            add_text_to_log("", 1);
            return Points;
        }
        public class spline
        {
            public double b, c, d, x, y;
        };
        List<spline> splines;
        public void Build_spline(List<double> x, List<double> y)
        {
            int n = x.Count;

            splines = new List<spline>(n);

            for (int i = 0; i < n; ++i)
            {
                splines.Add(new spline());
                splines[i].x = x[i];
                splines[i].y = y[i];
            }

            splines[0].c = 0;

            // Решение СЛАУ относительно коэффициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
            // Вычисление прогоночных коэффициентов - прямой ход метода прогонки

            List<double> alpha = new List<double>(n);
            List<double> beta = new List<double>(n);
            for (int i = 1; i < n; i++)
            {
                alpha.Add(0);
                beta.Add(0);
            }
            double A = 0, B, C = 0, F = 0, h_i, h_i1, z;

            for (int i = 1; i < n - 1; i++)
            {
                h_i = x[i] - x[i - 1];
                h_i1 = x[i + 1] - x[i];
                A = h_i;
                C = 2 * (h_i + h_i1);
                B = h_i1;
                F = 6 * ((y[i + 1] - y[i]) / h_i1 - (y[i] - y[i - 1]) / h_i);
                z = (A * alpha[i - 1] + C);
                alpha[i] = -B / z;
                beta[i] = (F - A * beta[i - 1]) / z;
            }

            splines[n - 1].c = (F - A * beta[n - 2]) / (C + A * alpha[n - 2]);

            // Нахождение решения - обратный ход метода прогонки
            for (int i = n - 2; i > 0; i--)
                splines[i].c = alpha[i] * splines[i + 1].c + beta[i];


            // По известным коэффициентам c[i] находим значения b[i] и d[i]
            for (int i = n - 1; i > 0; i--)
            {
                h_i = x[i] - x[i - 1];
                splines[i].d = (splines[i].c - splines[i - 1].c) / h_i;
                splines[i].b = h_i * (2 * splines[i].c + splines[i - 1].c) / 6 + (y[i] - y[i - 1]) / h_i;
            }

        }
        public IList<DataPoint> Cube_spline(IList<DataPoint> Points, List<double> x, List<double> y)
        {
            add_text_to_log("Кубический сплайн", 1);
            Points = new List<DataPoint>();
            Build_spline(x, y);
            int n = x.Count;
            add_text_to_log("Коэффициенты b, c, d:", 1);
            for (int i = 1; i < n; i++)
            {
                add_text_to_log(splines[i].b.ToString("F3"), 0);
                add_text_to_log(splines[i].c.ToString("F3"), 0);
                add_text_to_log(splines[i].d.ToString("F3"), 1);
            }

            spline s = new spline();
            for (double xi = x[0]; xi <= x[n - 1]; xi += 0.05)
            {
                int i = 0, j = n - 1;
                while (i + 1 < j) //поиск сплана
                {
                    int k = i + (j - i) / 2;
                    if (xi <= splines[k].x)
                        j = k;
                    else
                        i = k;
                }

                s = splines[j];

                double dx = (xi - s.x);
                Points.Add(new DataPoint(xi, s.y + (s.b + (s.c / 2 + s.d * dx / 6) * dx) * dx)); //получаем точку
            }
            return Points;
        }
        public IList<DataPoint> Line_approx(IList<DataPoint> Points, List<double> x, List<double> y)
        {
            add_text_to_log("Линейная апроксимация", 1);
            int n = x.Count;
            double xi;
            Points = new List<DataPoint>();
            double sumx = 0, sumy = 0, sumx2 = 0, sumxy = 0, a, b;
            for (int i = 0; i < n; i++)
            {
                sumx += x[i];
                sumy += y[i];
                sumx2 += Math.Pow(x[i], 2);
                sumxy += x[i] * y[i];
            }
            b = (n * sumxy - (sumx * sumy)) / (n * sumx2 - sumx * sumx);
            a = (sumy - b * sumx) / n;

            add_text_to_log("Коэффициенты a, b:", 1);
            add_text_to_log(a.ToString("F3"), 0);
            add_text_to_log(b.ToString("F3"), 1);

            for (xi = x[0]; xi < x[n - 1]; xi += 0.05)
            {
                Points.Add(new DataPoint(xi, a + b * xi));
            }

            add_text_to_log("X, Y, ApproxY:", 1);
            double summa = 0, oshibka, delta, approx;
            for (int i = 0; i < n; i++)
            {
                approx = a + b * x[i];
                delta = y[i] - approx;
                summa += Math.Pow(delta, 2);
                add_text_to_log(x[i].ToString("F3"), 0);
                add_text_to_log(y[i].ToString("F3"), 0);
                add_text_to_log(approx.ToString("F3"), 1);
            }
            oshibka = (summa / (n + 1));
            add_text_to_log("Ошибка:", 1);
            add_text_to_log(oshibka.ToString("F3"), 1);
            add_text_to_log("", 1);

            return Points;
        }
        public IList<DataPoint> Exp_approx(IList<DataPoint> Points, List<double> x, List<double> y)
        {
            add_text_to_log("Апроксимация функцией e^(a+bx)", 1);
            int n = x.Count;
            double xi;
            Points = new List<DataPoint>();
            double sumx = 0, sumy = 0, sumx2 = 0, sumxy = 0, a, b;
            for (int i = 0; i < n; i++)
            {
                if (y[i] > 0) //патамушта аппроксимирующая функция положительная
                {
                    sumx += x[i];
                    sumy += Math.Log(y[i]);
                    sumx2 += Math.Pow(x[i],2);
                    sumxy += x[i] * Math.Log(y[i]);
                }
            }
            b = (n * sumxy - (sumx * sumy)) / (n * sumx2 - sumx * sumx);
            a = (sumy - b * sumx) / n;

            add_text_to_log("Коэффициенты a, b:", 1);
            add_text_to_log(a.ToString("F3"), 0);
            add_text_to_log(b.ToString("F3"), 1);

            for (xi = x[0]; xi < x[n - 1]; xi += 0.05)
            {
                Points.Add(new DataPoint(xi, Math.Exp(a + b * xi)));
            }

            add_text_to_log("X, Y, ApproxY:", 1);
            double summa = 0, oshibka, delta, approx;
            for (int i = 0; i < n; i++)
            {
                approx = Math.Exp(a + b * x[i]);
                delta = y[i] - approx;
                summa += Math.Pow(delta, 2);
                add_text_to_log(x[i].ToString("F3"), 0);
                add_text_to_log(y[i].ToString("F3"), 0);
                add_text_to_log(approx.ToString("F3"), 1);
            }
            oshibka = (summa / (n + 1));
            add_text_to_log("Ошибка:", 1);
            add_text_to_log(oshibka.ToString("F3"), 1);
            add_text_to_log("", 1);

            return Points;
        }
    }
}
