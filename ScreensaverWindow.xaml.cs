using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bongo
{
    public partial class ScreensaverWindow : Window
    {
        private Random rnd = new Random();
        bool canExit = false;

        public ScreensaverWindow()
        {
            InitializeComponent();

            Loaded += async (_, __) =>
            {
                await Dispatcher.InvokeAsync(CreateAnimation, DispatcherPriority.Render);

                await Task.Delay(500);
                canExit = true;
            };

            KeyDown += ExitIfAllowed;
            MouseMove += ExitIfAllowed;
            MouseDown += ExitIfAllowed;
        }
        private void ExitIfAllowed(object sender, EventArgs e)
        {
            if (!canExit)
                return;

            Close();
        }



        private void CreateAnimation()
        {
            for (int i = 0; i < 15; i++)
            {
                var ellipse = new Ellipse
                {
                    Width = 50,
                    Height = 50,
                    Fill = new SolidColorBrush(
                        Color.FromRgb(
                            (byte)rnd.Next(255),
                            (byte)rnd.Next(255),
                            (byte)rnd.Next(255)))
                };

                MainCanvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, rnd.Next((int)ActualWidth));
                Canvas.SetTop(ellipse, rnd.Next((int)ActualHeight));

                Animate(ellipse);
            }
        }

        private void Animate(UIElement element)
        {
            var xAnim = new DoubleAnimation
            {
                From = 0,
                To = ActualWidth - 100,
                Duration = TimeSpan.FromSeconds(rnd.Next(5, 12)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var yAnim = new DoubleAnimation
            {
                From = 0,
                To = ActualHeight - 100,
                Duration = TimeSpan.FromSeconds(rnd.Next(6, 14)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            element.BeginAnimation(Canvas.LeftProperty, xAnim);
            element.BeginAnimation(Canvas.TopProperty, yAnim);
        }

        private void Exit(object sender, EventArgs e)
        {
            Close();
        }
    }
}
