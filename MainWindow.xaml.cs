using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReSchedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point anchorPoint;
        public MainWindow()
        {
            const int MinutesForRegistration = 7;
            const string TextAfterMinutesStages = "хвилин";
            const string TextForStages = "етапів";

            InitializeComponent();
            Registration.ScrollToHorizontalOffset(1028);

            FormPanel.CreateBlock("AAA");
            FormPanel.CreateBlock("BBB");
            FormPanel.CreateBlock("CCC");

            

            StageLine.Children.Add(FormPanel.ReturnCompletedPanel());

            TextOnStartRegistration.Text = $"Перед тим, як почати користуватись програмою, ви маєте пройти через {FormPanel.ReturnCountOfStages()} {TextForStages}, що займуть у вас приблизно {MinutesForRegistration} {TextAfterMinutesStages}";
        }

        private void CloseWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ToLowWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}