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
            var sorted = tasks.Select(s => s.indexTask.ToString()).ToList();

            sorted.Insert(0, "Kolejność: ");
            foreach (var task in tasks)
            {
                DrawChartTask(task, M1, 0);
                DrawChartTask(task, M2, 70);
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
            AxisX(cMax, 20, 95);
        }

        private void AxisX(int cMAX,  int step, int y)
        {
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
               
                FormattedText text = new FormattedText(i.ToString(),
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                8,Brushes.Black);
                xaxis_geom.Children.Add(text.BuildGeometry(new Point(x, y + 6)));
                i++;
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

        private List<MachineTask> Sort(List<MachineTask> tasks, bool isThirdMachine)
        {
            List<MachineTask> list1 = new List<MachineTask>();
            List<MachineTask> list2 = new List<MachineTask>();
            MachineTask toCompareTask;
            foreach (var task in tasks)
            {
                if (isThirdMachine)
                {
                    var d1 = task.duration[M1] + task.duration[M2];
                    var d2 = task.duration[M2] + task.duration[M3];
                    toCompareTask = new MachineTask()
                    {
                        indexTask = task.indexTask,
                        duration = new int[] { d1, d2 }
                    };
                }
                else
                {
                    toCompareTask = task;
                }
                if (toCompareTask.duration[M1] <= toCompareTask.duration[M2])
                    list1.Add(toCompareTask);
                else list2.Add(toCompareTask);
            }

            var sortedTasks= list1.OrderBy(m => m.duration[M1]).Concat(list2.OrderByDescending(m => m.duration[M2])).ToList();
            if (isThirdMachine)
            {
                //todo
            }
            return sortedTasks;
        }
    }
}
