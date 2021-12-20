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
using Expr = MathNet.Symbolics.SymbolicExpression;

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

        private void clearall()
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();

            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart2.Series[2].Points.Clear();
            chart2.Series[3].Points.Clear();

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
            graphRectangles(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), 1, rectangles);
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
            graphTrapez(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), 1, rectangles);
            return result;
        }

        double parabolasMethod(double a, double b, double h) //метод симпсона 
                                 //расчеты неверны, поэтому отрисовка говно
        {
            double S = 0;
            int count = 0;
            int lastCount = (int)((b - a) / h);


            for (double x = a; x <= (b + 0.01); x += h)
            {
                var y = f(x);

                if (count == 0 || count == lastCount)
                {
                    S += y;
                }
                else
                {
                    S += 2 * (count % 2 + 1) * y;
                }

                ++count;
            }

            for (double x = a; x <= (b + 0.01); x += 2 * h)
            {
                var f1 = f(x);
                var f2 = f(x + h);
                var f3 = f(x + 2 * h);
                var min = x - h;
                var max = x + h;
                var h2 = (max - min) / 10;
                var funcA = (1.0 / 2) * (f1 - 2 * f2 + f3) / Math.Pow(h, 2);
                var funcB = -(1.0 / 2) * (3 * f1 - 4 * f2 + f3) / h;
                var funcC = f1;
                var expression = Expr.Parse("a*x^2+b*x+c");
                var func = expression.Compile("a", "b", "c", "x");
                var y = func(funcA, funcB, funcC, x);

            }
            S = S * h / 3;
            return Math.Abs(S);
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            try
            {
                clearall();
                if (checkBox1.Checked == true)
                {
                    double res = Math.Abs(LeftRectangle(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), Convert.ToInt32(textBox1.Text)));
                    label1.Text = res.ToString();
                }
                if (checkBox2.Checked == true)
                {
                    double res = Math.Abs(Trapezia(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), Convert.ToInt32(textBox1.Text)));
                    label4.Text = res.ToString();
                }
                if (checkBox3.Checked == true)
                {
                    double h = (Convert.ToDouble(upper.Text) - Convert.ToDouble(lower.Text)) / Convert.ToInt32(textBox1.Text);
                    double res = Math.Round(parabolasMethod(Convert.ToDouble(lower.Text), Convert.ToDouble(upper.Text), h), 5);
                    label7.Text = res.ToString();

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void graphTrapez(double min, double max, int step, double[] trapez)//сама отрисовка графика
        {
            try
            {

                int count = (int)Math.Ceiling((max - min) / step) + 1;

                double[] x = new double[count];
                double[] y1 = new double[count];
                double[] y2 = new double[trapez.GetLength(0)];
                for (int i = 0; i < count; i++)
                {
                    x[i] = min + step * i;
                    y1[i] = f(x[i]);
                }
                for (int i = 0; i < trapez.GetLength(0); i++)
                {
                    y2[i] = f(trapez[i]);
                }
                chart1.Series[0].Points.DataBindXY(x, y1);
                for (int i = 0; i < trapez.GetLength(0); i++)
                {
                    chart2.Series[0].Points.AddXY(trapez[i], f(trapez[i]));
                    chart2.Series[1].Points.AddXY(trapez[i], f(trapez[i]));

                }
            }
            catch (System.OverflowException)
            {
                DialogResult err = MessageBox.Show("Границы введены неверно!!!\nУбедитесь в правильности введеных данных и повторите поптыку!", "Ошибка!");
            }
        }

        public void graphRectangles(double min, double max, int step, double[] rectangles)//сама отрисовка графика
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

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearall();
            textBox1.Text = "";
            function.Text = "";
            lower.Text = "";
            upper.Text = "";

            label1.Text = "";
            label4.Text = "";

            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label4.Text = "";
            label7.Text = "";
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
            this.Close();
        }
    }
}
