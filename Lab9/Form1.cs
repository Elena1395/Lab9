using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

namespace Lab9
{
    public partial class MainForm : Form
    {
        int Radius;
        int NumOfThreads;
        int Speed;
        List<Color> Colors = new List<Color>();
        int X;//коодринаты левого верхнего угла квадрата
        int Y;
        double res = 0;//методом МК
        double prRes = 0;//по обычной формуле
        int CountRect=0;//количество точек в квадрате
        int CountCrc=0;//количество точек в круге
        public MainForm()
        {
            InitializeComponent();
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (CheckFields()) {
                prRes = Math.Round((Math.PI * Radius * Radius), 0);
                res = 0;
                textBox_ResultPR.Text = prRes.ToString();
                Colors.Clear();

                PaintRectEllp();

                 Async_func();
            }
        }

        public async Task Async_func() {
            Task[] tasks = new Task[NumOfThreads];

            Random rnd = new Random();
            for (int i = 0; i < tasks.Length; i++)
            {
                Colors.Add(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));

                tasks[i] = await Task.Run(async () => Actions(i));
            }
        }

        public async Task Actions(int i) {
            Random rand = new Random();

            while (Math.Abs(res - prRes) >= 40)
            {          
                int x = rand.Next(X, X+2*Radius);
                int y = rand.Next(Y, Y+2*Radius);

                if ( ((X+Radius)-x)* ((X + Radius) - x)+ ((Y + Radius) - y) * ((Y + Radius) - y) <=Radius*Radius) {
                    Interlocked.Increment(ref CountCrc);
                }
                Interlocked.Increment(ref CountRect);

                Brush b = new SolidBrush(Colors[i]);
                PaintPoint(x,y,b);

                int localres = (4*Radius * Radius * CountCrc / CountRect);
                Interlocked.Exchange(ref res, localres);

                textBox_ResultMK.Invoke(new Action (() => textBox_ResultMK.Text = res.ToString()));
                //Thread.Sleep(Speed);              
                //Task.Delay(Speed).Wait();
                await Task.Delay(Speed);
            }
        }

        private void PaintRectEllp() {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(pictureBox1.BackColor);//Color.White

            X = (pictureBox1.Right-pictureBox1.Left)/2 - Radius;
            Y = (pictureBox1.Bottom-pictureBox1.Top)/2 - Radius;
           
            Rectangle r = new Rectangle(X, Y, 2 * Radius, 2 * Radius);
            Brush b = new SolidBrush(Color.Black);///////////
            Pen p = new Pen(b,3);
            g.DrawRectangle(p, r);

            //g.FillRectangle(b2, r);
            b = new SolidBrush(Color.Red);/////////////
            p = new Pen(b, 3);
            g.DrawEllipse(p, r);          
        }

        private void PaintPoint(int x, int y, Brush b)
        {
            Graphics g = pictureBox1.CreateGraphics();

            Pen p = new Pen(b);
            Rectangle point = new Rectangle(x, y, 3, 3);
            g.DrawRectangle(p, point);
            g.FillRectangle(b, point);
        }

        public bool CheckFields()
        {
            bool res=true;
            if (CheckPositiveInt(textBox_Radius.Text))
            {
                Int32.TryParse(textBox_Radius.Text, out Radius);
                if (Radius*2>pictureBox1.Width || Radius*2>pictureBox1.Height || Radius<50) {
                    MessageBox.Show("Радиус слишком большой или слишком маленький!");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Некорректный радиус!");
                return false;
            }
            if (CheckPositiveInt(textBox_NumOfThreads.Text))
            {
                Int32.TryParse(textBox_NumOfThreads.Text, out NumOfThreads);
            }
            else
            {
                MessageBox.Show("Некорректное число потоков!");
                return false;
            }
            if (CheckPositiveInt(textBox_Speed.Text))
            {
                Int32.TryParse(textBox_Speed.Text, out Speed);
            }
            else
            {
                MessageBox.Show("Некорректная скорость!");
                return false;
            }
            return res;
        }
        public bool CheckPositiveInt(string s) {
            int n;
            if (Int32.TryParse(textBox_Radius.Text, out n))
            {
                if (n >= 0)
                {
                    return true;
                }
            }
            return false;
        }
       
    }
}
