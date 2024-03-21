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

namespace ReSchedule
{
    /// <summary>
    /// Логика взаимодействия для PushUpMessage.xaml
    /// </summary>
    public partial class PushUpMessage : Window
    {
        public bool IsAnimated { get; private set; }
        public PushUpMessage()
        {
            InitializeComponent();
            IsAnimated = false;
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

                    if (timeBeforeSomething == "") throw new Exception("Необхіднно навести текст у часове поле");

                    BodyText.Visibility = Visibility.Visible;
                    TimeText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;
                    TimeText.Text = timeBeforeSomething;
                }break;

                case BeginOfLesson:
                {
                    Title.Text = "Початок заняття";

                    if (timeBeforeSomething == "") throw new Exception("Необхіднно навести текст у часове поле");

                    BodyText.Visibility = Visibility.Visible;
                    TimeText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;
                    TimeText.Text = timeBeforeSomething;
                }break;

                case EndOfLessons: {

                    Title.Text = "Кінець занять";
                    BodyText.Visibility = Visibility.Visible;

                    BodyText.Text = textOfMessage;

                }break;

                default: { throw new Exception("Недопустимий варіант"); }
            }
            AnimateMessage();
        }

        void AnimateMessage()
        {
           

            // Получить размеры рабочей области экрана
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            // Получить размеры окна
            var windowWidth = this.Width;
            var windowHeight = this.Height;

            var EndHeighCoef = 15;

            // Расстояние для всплытия системных уведомлений
            var notificationAreaHeight = 50; // Замените на реальное значение

            // Переместить окно в правый нижний угол
            this.Left = screenWidth - windowWidth - 15;
            this.Top = screenHeight - windowHeight + notificationAreaHeight;

            // Создание анимации для Top свойства
            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.From = this.Top;
            topAnimation.To = screenHeight - windowHeight - EndHeighCoef;
            topAnimation.Duration = TimeSpan.FromSeconds(0.3);
            topAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }; // функция плавности

            // Создание анимации для Opacity свойства
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0; // начальная прозрачность
            opacityAnimation.To = 1; // конечная прозрачность
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.3); // продолжительность

            // Создание Storyboard для управления анимациями
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(topAnimation);
            storyboard.Children.Add(opacityAnimation);

            // Привязка анимаций к свойствам окна
            Storyboard.SetTarget(topAnimation, this);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath(Window.TopProperty));
            Storyboard.SetTarget(opacityAnimation, this);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));

            // Запуск анимаций
            storyboard.Begin();

            IsAnimated = true;
        }
    }
}
