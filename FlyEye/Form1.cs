using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace FlyEye
{
    
    public partial class Form1 : Form
    {
        //structs
        public struct coordinate
        {
            public void SetPoint(double thetta, int H)
            {

                y =(int)Math.Round( H*Math.Cos(thetta), MidpointRounding.ToEven);
                x =(int)Math.Round( H*Math.Sin(thetta), MidpointRounding.ToEven);
                
            }
            public void SetPoint(coordinate equal)
            {
                x = equal.x;
                y = equal.y;
            }
            public int x, y;
        };

        public struct PointSet
        {
            public List<Line> linesonpoint;
            public bool setfinished;
            public void start()
            {
                setfinished = false;
            }
            public void settrue()
            {
                setfinished = true;
            }
        };
        //endofstructs
        //variables
        public Pen liner;
        Brush alfa;
        public Button startButton;
        public CheckBox parallelCheckbox;
        public ListBox
            xList,
            yList;
        public TextBox lineTextbox;
        List<coordinate> pointList;
        List<PointSet> totalSets;
        public int totalPoints;
        public int circleSize = 460;
        private int linesBySet = -1;
        private int stopper = -1;
        public int howmuchtime = 0;
        public Timer tmr;
        private TimeSpan timelapse;
        public Label timeLabel;
        public Label parallelLabel;
        public Label howmuch;
        //endof variables
        public Form1()
        {
            InitializeComponent();
            InitializeDynamicComponents();
            
        }
        //myfunctions
        private void InitializeDynamicComponents()
        {

            this.Size = new Size(500, 700);
            this.CenterToScreen();

            liner = new Pen(Color.FromArgb(255, 0, 0, 0));
            alfa = new SolidBrush(Color.Gray);
            startButton = new Button();
            parallelCheckbox = new CheckBox();
            lineTextbox = new TextBox();
            xList = new ListBox();
            yList = new ListBox();
            pointList = new List<coordinate>();
            totalSets = new List<PointSet>();
            parallelLabel = new Label();
            howmuch = new Label();
            tmr = new Timer();
            tmr.Enabled = true;
            tmr.Interval = 1000;

            timelapse = new TimeSpan();
            startButton.Text = "Draw";
            parallelLabel.Text = "Parallel";
            parallelCheckbox.Width = 20;
            xList.Size = new Size(75, 150);
            yList.Size = new Size(75, 150);
            timeLabel = new Label();
            timeLabel.Text = DateTime.Now.ToLongTimeString();
            startButton.Location = new Point
                (3 * this.Size.Width / 4 - startButton.Size.Width / 2, 500);
            lineTextbox.Location = new Point
                (this.Size.Width / 2 - startButton.Size.Width / 2, 500);
            xList.Location = new Point
                (50,500);
            yList.Location = new Point
                ((xList.Location.X + xList.Size.Width), 500);
            timeLabel.Location = new Point
                (lineTextbox.Location.X , lineTextbox.Location.Y+lineTextbox.Height);
            parallelCheckbox.Location = new Point
                (startButton.Location.X, startButton.Location.Y + startButton.Height);
            parallelLabel.Location = new Point
                (parallelCheckbox.Location.X +parallelCheckbox.Width, parallelCheckbox.Location.Y+5);
            howmuch.Location = new Point
                (timeLabel.Location.X, timeLabel.Location.Y + timeLabel.Height);
            xList.Items.Add("X");
            yList.Items.Add("Y");
            
            startButton.Click += new EventHandler(startButton_OnClick);
           
            lineTextbox.KeyPress += new KeyPressEventHandler(lineTextbox_OnClick);
            tmr.Tick += new EventHandler(tmr_Tick);

            Controls.Add(parallelCheckbox);
            Controls.Add(parallelLabel);
            Controls.Add(startButton);
            Controls.Add(lineTextbox);
            Controls.Add(timeLabel);
            Controls.Add(xList);
            Controls.Add(yList);
            Controls.Add(howmuch);
        }
        private void CleanEverything()
        {
            totalSets.Clear();
            pointList.Clear();
        }
        private void tmr_Tick(object sender, EventArgs e)
        {
            timelapse = timelapse.Add(TimeSpan.FromMilliseconds(100));
            timeLabel.Text = DateTime.Now.ToLongTimeString();
            if(startButton.Enabled==false)
            {
                howmuchtime += 1;
                howmuch.Text = howmuchtime.ToString() + " segundos";
            }
        }
        public double ConvertToRadians(double angle)
        {
            double tofloor = Math.Floor(angle * (Math.PI / 180) * 1000);
            return tofloor/1000;
        }
        public double ConvertToDegrees(double angle)
        {
            double tofloor = Math.Floor(angle * (180 / Math.PI) * 1000);
            return tofloor / 1000;
        }
        private void SetCoordinates()
        {
            int thetta = 360;
            xList.Items.Clear();
            yList.Items.Clear();
            pointList.Clear();
            for (int index = 0; index < totalPoints; index++)
            {
                coordinate a = new coordinate();
                int alfa = thetta * index / totalPoints;
                double b = ConvertToRadians(alfa);
                a.SetPoint(b, circleSize/2);
                a.x += circleSize / 2;
                a.y += circleSize / 2;
                pointList.Add(a);
                xList.Items.Add(a.x);
                yList.Items.Add(a.y);
            }
        }
        private void DrawnonParallel()
        {
            stopper = 0;
            bool working = true;
            while (working)
            {
                for (int i = 0; i < totalPoints; i++)
                {
                    for (int j = 0; j < linesBySet; j++)
                    {
                        if (totalSets[i].linesonpoint[j].currdis != totalSets[i].linesonpoint[j].distance)
                        {
                            int offsetX =
                                (int)((double)totalSets[i].linesonpoint[j].currdis / (double)totalSets[i].linesonpoint[j].distance * (double)totalSets[i].linesonpoint[j].dX);
                            int offsetY =
                                (int)((double)totalSets[i].linesonpoint[j].currdis / (double)totalSets[i].linesonpoint[j].distance * (double)totalSets[i].linesonpoint[j].dY);
                            Graphics e = this.CreateGraphics();
                            e.DrawLine
                                (liner,
                                totalSets[i].linesonpoint[j].start.x,
                                totalSets[i].linesonpoint[j].start.y,
                                (totalSets[i].linesonpoint[j].start.x + offsetX),
                                (totalSets[i].linesonpoint[j].start.y + offsetY));
                            totalSets[i].linesonpoint[j].currdis++;
                        }
                        if (
                            totalSets[i].linesonpoint[j].finished == false &&
                            totalSets[i].linesonpoint[j].currdis == totalSets[i].linesonpoint[j].distance)
                        {
                            totalSets[i].linesonpoint[j].finished = true;
                            stopper++;
                        }
                    }
                }
                if (stopper >= totalPoints * linesBySet)
                {
                    working = false;
                }
            }
            startButton.Enabled = true;
            lineTextbox.Enabled = true;
            
        }
        public void ParallelLine(object alfa)
        {
            int i = (int)alfa;
            int stoppedlines = 0;
            
            while (stoppedlines!=linesBySet)
            {
                for (int j = 0; j < linesBySet; j++)
                {
                    if (totalSets[i].linesonpoint[j].currdis != totalSets[i].linesonpoint[j].distance)
                    {
                        int offsetX =
                            (int)((double)totalSets[i].linesonpoint[j].currdis / (double)totalSets[i].linesonpoint[j].distance * (double)totalSets[i].linesonpoint[j].dX);
                        int offsetY =
                            (int)((double)totalSets[i].linesonpoint[j].currdis / (double)totalSets[i].linesonpoint[j].distance * (double)totalSets[i].linesonpoint[j].dY);
                        Graphics e = this.CreateGraphics();
                        e.DrawLine
                            (liner,
                            totalSets[i].linesonpoint[j].start.x,
                            totalSets[i].linesonpoint[j].start.y,
                            (totalSets[i].linesonpoint[j].start.x + offsetX),
                            (totalSets[i].linesonpoint[j].start.y + offsetY));
                        totalSets[i].linesonpoint[j].currdis++;
                    }
                    if (totalSets[i].linesonpoint[j].finished == false && totalSets[i].linesonpoint[j].currdis == totalSets[i].linesonpoint[j].distance)
                    {
                        totalSets[i].linesonpoint[j].finished = true;
                        stoppedlines++;
                    }
                }
            }
        }
        private void DrawExperimental()
        {
            int limit = totalPoints;
            for (int i = 0; i < totalPoints; i++)
            {

                System.Threading.ParameterizedThreadStart value =
                    new System.Threading.ParameterizedThreadStart(ParallelLine);
                System.Threading.Thread th = new System.Threading.Thread(value);
                th.Start(i);
                while(th.IsAlive)
                {

                }
                
            }
        }
        private void DrawParallel()
        {
            System.Threading.Thread t = new System.Threading.Thread(DrawnonParallel);
            t.Start();
        }
        private void SetLines()
        {
            linesBySet = ((totalPoints / 2));
            
            if(totalPoints%2==0)
            {
                linesBySet--;
            }
            for (int i = 0; i<totalPoints;i++)
            {
                int rounder = 0;
                PointSet ps = new PointSet();
                ps.start();
                ps.linesonpoint = new List<Line>();
                for (int j = i+1 ,v=0; v < linesBySet ;++v, ++j)
                {
                    if(j>totalPoints-1)
                    {
                        j = rounder;
                        rounder++;
                    }
                    Line p = new Line(pointList[i], pointList[j]);
                    ps.linesonpoint.Add(p);
                }
                totalSets.Add(ps);
            }
            if (parallelCheckbox.Checked == true)
                DrawParallel();
            else
                DrawnonParallel();
        }
        
        private void InitializeLines()
        {
            lineTextbox.Enabled = false;
            startButton.Enabled = false;
            ClearCanvas();
            howmuchtime = 0;
            DrawCircle(0, 0, circleSize);
            try
            {
                totalPoints = int.Parse(lineTextbox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("El valor no tiene el formato permitido \n Inserta un numero");
                lineTextbox.Text = "";
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No ingresaste ningun valor");
                lineTextbox.Text = "";
                return;
            }
            if (totalPoints < 3)
            {
                MessageBox.Show("El valor es demasiado pequeño \n"+
                                "Escriba un numero mayor a 2");
                lineTextbox.Text = "";
                return;
            }
            CleanEverything();
            SetCoordinates();
            for (int index = 0; index < totalPoints; index++)
            {
                DrawCircle(pointList[index].x-2, pointList[index].y-2, 4);
            }
            SetLines();
            
        }
        private void ClearCanvas()
        {
            Graphics Clearer = this.CreateGraphics();
            Clearer.FillRectangle(alfa, 0, 0, 500, 500);
        }
        private void DrawCircle(int x, int y,int size)
        {
            Graphics circle = this.CreateGraphics();
            circle.DrawEllipse(liner, x, y, size, size);
        }
        private void startButton_OnClick(object sender, EventArgs e)
        {
            InitializeLines();
            
        }
        private void lineTextbox_OnClick(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                InitializeLines();
            }
        }

        
        //endof myfunctions
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    public class Line
    {
        public Form1.coordinate
            start = new Form1.coordinate(),
            end = new Form1.coordinate();
        public int
            dX,
            dY,
            distance,
            currdis = -1;
        public bool finished;
        public void diferential()
        {
            dX = end.x - start.x;
            dY = end.y - start.y;
        }
       
        public Line(Form1.coordinate A, Form1.coordinate B)
        {
            start.SetPoint(A);
            end.SetPoint(B);
            dX = end.x - start.x;
            dY = end.y - start.y;
            distance = (int)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            currdis = 0;
            finished = false;
        }
    };
}
