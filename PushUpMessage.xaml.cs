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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ReSchedule
{
    /// <summary>
    /// Логика взаимодействия для PushUpMessage.xaml
    /// </summary>
    public partial class PushUpMessage : Window
    {

        MainWindow WindowData;

        DispatcherTimer TimeBeforeEnd;

        int TickIterator = 0;
        public PushUpMessage(MainWindow windowData)
        {
            InitializeComponent();
            Opacity = 0;
            WindowData = windowData;
            TimeBeforeEnd = new DispatcherTimer();
            TimeBeforeEnd.Interval = TimeSpan.FromSeconds(5);

            TimeBeforeEnd.Tick += (s, e) => {

                var screenWidth = SystemParameters.WorkArea.Width;
                var screenHeight = SystemParameters.WorkArea.Height;

                var windowWidth = this.Width;
                var windowHeight = this.Height;

                var EndHeighCoef = 15;

                var notificationAreaHeight = 50;

                this.Left = screenWidth - windowWidth - 15;
                this.Top = screenHeight - windowHeight + notificationAreaHeight;

                DoubleAnimation topAnimation = new DoubleAnimation();
                topAnimation.From = Top;
                topAnimation.To = screenHeight - windowHeight;
                topAnimation.Duration = TimeSpan.FromSeconds(1);
                topAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut };

                DoubleAnimation opacityAnimation = new DoubleAnimation();
                opacityAnimation.From = 1;
                opacityAnimation.To = 0;
                opacityAnimation.Duration = TimeSpan.FromSeconds(0.3);

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(topAnimation);
                storyboard.Children.Add(opacityAnimation);

                Storyboard.SetTarget(topAnimation, this);
                Storyboard.SetTargetProperty(topAnimation, new PropertyPath(Window.TopProperty));
                Storyboard.SetTarget(opacityAnimation, this);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));

                storyboard.Completed += (s, e) => {
                    WindowData.DestroyPushUp(this);
                };

                storyboard.Begin();

            };
        }

        public const int EndOfLesson = 0;
        public const int BeginOfLesson = 1;
        public const int EndOfLessons = 2;

        public void ShowTheMessage(int EventForMessage, string textOfMessage, string timeBeforeSomething = "")
        {
            switch (EventForMessage)
            {
                case EndOfLesson:
                {
                    Title.Text = "Кінець заняття";

                    PlaySound("C:\\Users\\Mashiroon\\source\\repos\\ReSchedule\\Sounds\\EndLesson.mp3");

                    if (timeBeforeSomething == "") throw new Exception("Необхіднно навести текст у часове поле");

                    BodyText.Visibility = Visibility.Visible;
                    TimeText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;
                    TimeText.Text = timeBeforeSomething;
                }break;

                case BeginOfLesson:
                {
                    Title.Text = "Початок заняття";

                    PlaySound("C:\\Users\\Mashiroon\\source\\repos\\ReSchedule\\Sounds\\BeginLesson.mp3");

                    if (timeBeforeSomething == "") throw new Exception("Необхіднно навести текст у часове поле");

                    BodyText.Visibility = Visibility.Visible;
                    TimeText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;
                    TimeText.Text = timeBeforeSomething;
                }break;

                case EndOfLessons: {

                    Title.Text = "Кінець занять";

                    PlaySound("C:\\Users\\Mashiroon\\source\\repos\\ReSchedule\\Sounds\\EndLesson.mp3");

                    BodyText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;

                }break;

                default: { throw new Exception("Недопустимий варіант"); }
            }
            Show();
            AnimateMessage();
        }

        private void PlaySound(string filePath)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
        }

        void AnimateMessage()
        {
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            var windowWidth = this.Width;
            var windowHeight = this.Height;

            var EndHeighCoef = 15;

            var notificationAreaHeight = 50;

            this.Left = screenWidth - windowWidth - 15;
            this.Top = screenHeight - windowHeight + notificationAreaHeight;

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.From = this.Top;
            topAnimation.To = screenHeight - windowHeight - EndHeighCoef;
            topAnimation.Duration = TimeSpan.FromSeconds(0.3);
            topAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut };

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.3);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(topAnimation);
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTarget(topAnimation, this);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath(Window.TopProperty));
            Storyboard.SetTarget(opacityAnimation, this);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));

            storyboard.Begin();

            TimeBeforeEnd.Start();
        }
    }
}
