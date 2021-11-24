using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using org.mariuszgromada.math.mxparser;

namespace Laba5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double f(double x)//вынес подставления значения в функцию в отдельный метод
        {
            double result = 0;
            Function f = new Function("f(x) = " + function.Text);
            string sklt = "f()";
            string fx = sklt.Insert(2, x.ToString());
            fx = fx.Replace(",", ".");
            Expression fxx = new Expression(fx, f);
            result = fxx.calculate();
            return result;
        }
        double LeftRectangle(double a, double b, int n)
        {
            double[] rectangles = new double[n+1];
            var h = (b - a) / n;
            var sum = 0d;
            for (var i = 0; i <= n - 1; i++)
            {
                var x = a + i * h;
                rectangles[i] = x;
                sum += f(x);
            }

            rectangles[rectangles.GetLength(0)-1] = b;
            var result = h * sum;
            graph(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), 1, rectangles);
            return result;
        }
        double Trapezia(double a, double b, int n)
        {
            double[] rectangles = new double[n + 1];
            var h = (b - a) / n;
            var sum = (f(a)+f(b))/2;
            for (double i = 1; i <= n - 1; i++)
            {
                var x = a + i * h;
                rectangles[(int)i] = x;
                sum += f(x);
            }

            rectangles[rectangles.GetLength(0) - 1] = b;
            var result = h * sum;
            graph(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), 1, rectangles);
            return result;
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            double res = Trapezia(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), Convert.ToInt32(textBox1.Text));
            label1.Text = res.ToString();
        }
        public void graph(double min, double max, int step, double[] rectangles)//сама отрисовка графика
        {
            try
            {

                int count = (int)Math.Ceiling((max - min) / step) + 1;

                double[] x = new double[count];
                double[] y1 = new double[count];
                double[] y2 = new double[rectangles.GetLength(0)];
                for (int i = 0; i < count; i++)
                {
                    x[i] = min + step * i;
                    y1[i] = f(x[i]);
                }
                for (int i = 0; i < rectangles.GetLength(0); i++)
                {
                    y2[i] = f(rectangles[i]);
                }
                chart1.Series[0].Points.DataBindXY(x, y1);
                for (int i = 0; i < rectangles.GetLength(0); i++)
                {
                    chart1.Series[2].Points.AddXY(rectangles[i], 0);
                    chart1.Series[2].Points.AddXY(rectangles[i], f(rectangles[i]));
                }
            }
            catch (System.OverflowException)
            {
                DialogResult err = MessageBox.Show("Границы введены неверно!!!\nУбедитесь в правильности введеных данных и повторите поптыку!", "Ошибка!");
            }
        }
    }
}
