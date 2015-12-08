using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTD_Proj_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public int countOfMachine;
        //public Dictionary<int,int> M1;
        //public Dictionary<int, int> M2;
        //public Dictionary<int, int> M3;
        //public List<MachineTask> Tasks;
        public Random rnd;
        public const int M1 = 0;
        public const int M2 = 1;
        public const int M3 = 2;
        public MainWindow()
        {
            InitializeComponent();
            // countOfMachine = 0;
            //M1 = new Dictionary<int, int>();
            //M2 = new Dictionary<int, int>();
            //M3 = new Dictionary<int, int>();
            // Tasks = new List<MachineTask>();
            rnd = new Random();

        }
        public int CountofTask { get; set; }
        public int countOfMachine
        {
            get
            {
                if (number3.IsChecked.Value)
                {
                    return 3;
                }
                else
                {
                    return 2;
                }

            }
        }
        private void generateTask_Click(object sender, RoutedEventArgs e)
        {

            //if (number2.IsChecked.Value)
            //{
            //    countOfMachine = 2;
            //}
            //if (number3.IsChecked.Value)
            //{
            //    countOfMachine = 3;
            //}
            myChart.Children.Clear();
            int countOfTask;
            if (int.TryParse(NumberOfTask.Text, out countOfTask))
            {
                CountofTask = countOfTask;
                BuildTable(countOfMachine, countOfTask);
            }

        }

        private void BuildTable(int countOfMachine, int countOfTask)
        {
            var dt = new DataTable();

            for (int i = 0; i <= countOfTask; i++)
            {
                if (i == 0)
                    dt.Columns.Add("Nr");
                else
                    dt.Columns.Add("Z" + i.ToString(), typeof(int));
            }
            for (int i = 1; i <= countOfMachine; i++)
            {
                DataRow r = dt.NewRow();
                r[0] = "M" + i.ToString();
                for (int j = 1; j <= countOfTask; j++)
                {
                    r[j] = rnd.Next(1, 10);
                }
                dt.Rows.Add(r);
            }
            myGrid.ItemsSource = dt.AsDataView();

        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            //M1= (GetDataGridRows(myGrid).Select(r => ((DataRowView)r.Item).Row)).ToList()[0].ItemArray.OfType<int>()
            //.Select((s, i) => new { s, i }).ToDictionary(x => x.i + 1, x => x.s);
            //M2 = (GetDataGridRows(myGrid).Select(r => ((DataRowView)r.Item).Row)).ToList()[1].ItemArray.OfType<int>()
            //    .Select((s, i) => new { s, i }).ToDictionary(x => x.i+1, x => x.s);
            //if (countOfMachine.Equals(3))
            //{
            //   M3 = (GetDataGridRows(myGrid).Select(r => ((DataRowView)r.Item).Row)).ToList()[2].ItemArray.OfType<int>()
            //        .Select((s, i) => new { s, i }).ToDictionary(x => x.i + 1, x => x.s);
            //}
            var tasks = new List<MachineTask>();
            for (int i = 1; i <= CountofTask; i++)
            {
                MachineTask task = new MachineTask() { indexTask = i, duration = GetDataGridRows(myGrid).Select((r => ((DataRowView)r.Item).Row[i])).OfType<int>().ToArray() };
                tasks.Add(task);
            }
            JohnsonsAlgorithm(tasks);
        }

        private void JohnsonsAlgorithm(List<MachineTask> tasks)
        {
            List<MachineTask> sortedTasks = Sort(tasks, number3.IsChecked.Value);
            int currentTick = 0;
            MachineTask prevTask = new MachineTask();
            for (int i = 0; i < sortedTasks.Count; i++)
            {
                //var currentTask = sortedTasks[i];
                if (i > 0)
                {
                    prevTask = sortedTasks[i - 1];
                }
                sortedTasks[i].start[M1] = currentTick;
                currentTick += sortedTasks[i].duration[M1];
                sortedTasks[i].end[M1] = currentTick;

                if (prevTask != null && i > 0 && prevTask.end[M2] > sortedTasks[i].end[M1])
                {
                    sortedTasks[i].start[M2] = prevTask.end[M2];
                }
                else
                {
                    sortedTasks[i].start[M2] = sortedTasks[i].end[M1];
                }
                sortedTasks[i].end[M2] = sortedTasks[i].start[M2] + sortedTasks[i].duration[M2];
            }
            DrawnCharts(sortedTasks);
        }

        private void DrawnCharts(List<MachineTask> tasks)
        {
            //var a = test();
            //S//tringBuilder sorted = new StringBuilder("Kolejność: ");
            var sorted = tasks.Select(s => s.indexTask.ToString()).ToList();
            //int w = 0;
            sorted.Insert(0, "Kolejność: ");
            foreach (var task in tasks)
            {
                DrawChartTask(task, M1, 0);
                DrawChartTask(task, M2, 60);
                //Line ln = new Line();
                //Label title = new Label();
                //Label title2 = new Label();
                //title.Content = "M1";
                //title2.Content = "M2";
                //ln.Stroke = new SolidColorBrush(colorProperty[task.indexTask % colorProperty.Length]);
                //ln.Fill = new SolidColorBrush(Colors.Black);
                //ln.StrokeThickness = 50.0;
                //ln.X1 = 30+ task.start[M1] *50;
                //ln.X2 = 30+ task.end[M1] * 50;
                //ln.Y1 = 0;
                //ln.Y2 = 0;
                //ln.ToolTip = task.indexTask;
                //Line ln2 = new Line();
                //ln2.Stroke = new SolidColorBrush(colorProperty[task.indexTask % colorProperty.Length]);
                //ln2.Fill = new SolidColorBrush(Colors.Black);
                //ln2.StrokeThickness = 50.0;
                //ln2.X1 =30+ task.start[M2] *50 ;
                //ln2.X2 = 30+ task.end[M2] *50;
                //ln2.Y1 = 60;
                //ln2.Y2 = 60;
                //ln2.ToolTip = task.indexTask;
                //Canvas.SetTop(title, 0);
                //Canvas.SetLeft(title, 0);
                //myChart.Children.Add(title);
                //Canvas.SetTop(title2, 60);
                //Canvas.SetLeft(title2, 0);
                //myChart.Children.Add(title2);
                //Label l = new Label();
                //l.Content = "Z" + task.indexTask;
                //Label l2 = new Label();
                //l2.Content = "Z" + task.indexTask;
                //myChart.Children.Add(ln);
                //myChart.Children.Add(ln2);
                //Canvas.SetTop(l, 0);
                //Canvas.SetLeft(l, ln.X1 + 1);
                //myChart.Children.Add(l);
                //Canvas.SetTop(l2, 60);
                //Canvas.SetLeft(l2, ln2.X1 + 1);
                //myChart.Children.Add(l2);
            }


            Label label = new Label();
            label.Content = string.Join(" Z", sorted);
            Canvas.SetTop(label, 120);
            Canvas.SetLeft(label, 0);
            myChart.Children.Add(label);
            Label label2 = new Label();
            var cMax = tasks.Last().end[M2];
            label2.Content = "CMAX= " + cMax;
            Canvas.SetTop(label2, 150);
            Canvas.SetLeft(label2, 0);
            myChart.Children.Add(label2);
            //int step, last;
            AxisX(cMax, 20,25);
            AxisX(cMax, 20, 85);

            //GeometryGroup xaxis_geom2 = new GeometryGroup();
            //xaxis_geom2.Children.Add(new LineGeometry(
            //    new Point(30, 25), new Point(last, 25)));
            //for (double x = 30; x <= last; x += step)
            //{
            //    xaxis_geom2.Children.Add(new LineGeometry(
            //        new Point(x, 20),
            //        new Point(x, 30)));
            //}

            //Path xaxis_path2 = new Path();
            //xaxis_path2.StrokeThickness = 2;
            //xaxis_path2.Stroke = Brushes.Black;
            //xaxis_path2.Data = xaxis_geom2;
            //myChart.Children.Add(xaxis_path2);

        }

        private void AxisX(int cMAX,  int step, int y)
        {
            //step = 20;
            var last = 30 + cMAX * 20;
            var i = 0;
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(30, y), new Point(last, y)));
            for (double x = 30; x <= last; x += step)
            {

                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, y-5),
                    new Point(x, y+5)));
                i++;
                FormattedText text = new FormattedText(i.ToString(),
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                10,Brushes.Black);

                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext= drawingVisual.RenderOpen();
                drawingContext.DrawText(text, new Point(x, y+6));
               // xaxis_geom.Children.Add((DrawingGroup)drawingContext);
                //xaxis_geom.Children.Add(new FormattedText(i,));

            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 2;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;
            myChart.Children.Add(xaxis_path);
        }

        private void DrawChartTask(MachineTask task, int m, int w)
        {
            Line ln = new Line();
            Label title = new Label();
            Label l = new Label();

            int i = m + 1;
            title.Content = "M"+i;
            ln.Stroke = new SolidColorBrush(colorProperty[task.indexTask % colorProperty.Length]);
            ln.Fill = new SolidColorBrush(Colors.Black);
            ln.StrokeThickness = 50.0;
            ln.X1 = 30 + task.start[m] * 20;
            ln.X2 = 30 + task.end[m] * 20;
            ln.Y1 = w;
            ln.Y2 = w;
            ln.ToolTip = task.indexTask;
            l.Content = "Z" + task.indexTask;
            myChart.Children.Add(ln);
            Canvas.SetTop(title, w);
            Canvas.SetLeft(title, 0);
            myChart.Children.Add(title);
            Canvas.SetTop(l, w);
            Canvas.SetLeft(l, ln.X1 + 1);
            myChart.Children.Add(l);
        }

        public Color[] colorProperty
        {
            get
            {
                return new Color[]
                      {
                        Colors.Red,
                        Colors.Chocolate,
                        Colors.DeepPink,
                        Colors.DeepSkyBlue,
                        Colors.Orange,
                        Colors.Blue,
                        Colors.Green,
                        Colors.Yellow,
                        Colors.DarkGoldenrod,
                        Colors.Pink,
                        Colors.DarkOrchid,
                        Colors.DarkTurquoise,
                        Colors.DarkSalmon,
                        Colors.DarkOliveGreen,
                        Colors.DarkKhaki
                      };
            }

        }

        //public Color color(int i)
        //{
        //    return (Color)colorProperty[i].GetValue(null, null);
        //}
        private List<MachineTask> Sort(List<MachineTask> tasks, bool v)
        {
            List<MachineTask> list1 = new List<MachineTask>();
            List<MachineTask> list2 = new List<MachineTask>();
            foreach (var task in tasks)
            {
                if (task.duration[M1] <= task.duration[M2])
                    list1.Add(task);
                else list2.Add(task);
            }

            return list1.OrderBy(m => m.duration[M1]).Concat(list2.OrderByDescending(m => m.duration[M2])).ToList();

        }
    }
}
