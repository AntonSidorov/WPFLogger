using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using LogWriter;

namespace LoggerTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger _logWriter;
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            _logWriter = new Logger(logBoxDemo, Dispatcher);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var enumValues = Enum.GetValues(typeof (LogType));
            var rand = new Random();
            var logType = (LogType) enumValues.GetValue(rand.Next(enumValues.Length));
            _timer.Interval = TimeSpan.FromMilliseconds(rand.Next(100,3000));
            var msg = "TEsting Logger" + rand.Next();
            _logWriter.Log(msg, logType, prependLogType:true);
        }
    }
}
