using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Waterfall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// 屏保使用方法:编译后将文件后缀名改为.scr 邮件安装即可.
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer t = new Timer();
        public MainWindow()
        {
            InitializeComponent();
            t.Interval = 5000;
            t.Tick += delegate {
                cl.Text = DateTime.Now.ToString("hh");
                min.Text = DateTime.Now.ToString("mm");
                date.Text = DateTime.Now.Month + " / " + DateTime.Now.Day + "    " + DateTime.Now.DayOfWeek.ToString();
            };
            cl.Text = DateTime.Now.ToString("hh");
            min.Text= DateTime.Now.ToString("mm");
            date.Text = DateTime.Now.Month + " / " + DateTime.Now.Day + "    " + DateTime.Now.DayOfWeek.ToString();
            Top = 0;
            Left = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            string path = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Themes\TranscodedWallpaper");
            var dt = new Bitmap(path);
            var rect = new Rectangle(0, 0, dt.Width, dt.Height);
            dt.GaussianBlur(ref rect, 80);
            window.Background = new ImageBrush(new BitmapImage(new Uri(path)));
            bk.Background = new ImageBrush(dt.ToBitmapImage()) { Stretch = Stretch.UniformToFill };
            Loaded += MainWindow_LoadedAsync;
            t.Start();
        }

        private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            var a=await WeatherLib.GetWeatherAsync("临沧");
            im.ImageBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/" + a.code+".png")));
            qw.Text = a.Qiwen + "  " + a.MiaoShu + "  " + a.Data[0].QiWen;
            kq.Text = "空气质量:"+a.KongQiZhiLiang;
            var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            pp.BeginAnimation(OpacityProperty, da);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dta = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            dta.Completed += async delegate {
                await Task.Delay(500);
                var value = 0 - SystemParameters.PrimaryScreenHeight;
                var ti = TimeSpan.FromSeconds(0.8);
                var da = new DoubleAnimationUsingKeyFrames();
                var ed = new EasingDoubleKeyFrame(value, ti);
                ed.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseInOut };
                da.KeyFrames.Add(ed);
                da.Completed += async delegate {
                    await Task.Delay(1000);
                    System.Windows.Application.Current.Shutdown();
                };
                BeginAnimation(TopProperty, da);
            };
            bn.BeginAnimation(OpacityProperty, dta);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Window_MouseDown(null, null);
        }
    }
}
