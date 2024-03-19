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
using System.Windows.Shapes;

namespace ReSchedule
{
    /// <summary>
    /// Логика взаимодействия для WatchAllLessons.xaml
    /// </summary>
    public partial class WatchAllLessons : Window
    {
        public WatchAllLessons(int NumberOfDay)
        {
            InitializeComponent();

            switch (NumberOfDay)
            {
                case 1:
                    {
                        Monday.IsChecked = true;
                    }break;

                    case 2:
                    {
                        Thuesday.IsChecked = true;
                    }
                    break;

                    case 3:
                    {
                        Wednesday.IsChecked = true;
                    }
                    break;

                    case 4:
                    {
                        Thursday.IsChecked = true;
                    }
                    break;

                    case 5:
                    {
                        Friday.IsChecked = true;
                    }
                    break;

                    case 6:{
                        Monday.IsChecked = true;
                    }
                    break;

                    case 7: {
                        Monday.IsChecked = true;
                    } break;

                    default:
                    {
                        throw new Exception("Такого дня не існує");
                    }

            }
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
