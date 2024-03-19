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
        AllLessons LessonsData;
        MainWindow AllWindowData;
        public WatchAllLessons(int NumberOfDay, AllLessons AllCurrentLessons, MainWindow windowData)
        {
            InitializeComponent();

            switch (NumberOfDay)
            {
                case 1:
                    {
                        Monday.IsChecked = true;
                        FormLesson(AllCurrentLessons.Monday);

                    }break;

                    case 2:
                    {
                        Thuesday.IsChecked = true;
                        FormLesson(AllCurrentLessons.Thuesday);
                    }
                    break;

                    case 3:
                    {
                        Wednesday.IsChecked = true;
                        FormLesson(AllCurrentLessons.Wednesday);
                    }
                    break;

                    case 4:
                    {
                        Thursday.IsChecked = true;
                        FormLesson(AllCurrentLessons.Thursday);
                    }
                    break;

                    case 5:
                    {
                        Friday.IsChecked = true;
                        FormLesson(AllCurrentLessons.Friday);
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

            LessonsData = AllCurrentLessons;

            AllWindowData = windowData;
        }

        public void FormLesson(List<LessonPair> ListOfLessons)
        {
            var LessonsData = ListOfLessons;

            for (int i = 0; i < LessonsData.Count; i++)
            {
                if (LessonsData[i].Lessons1.lesson == "-" && LessonsData[i].Lessons2.lesson == "-")
                {
                    Border tempBorder = FindName($"Lesson{i + 1}") as Border;
                    tempBorder.Visibility = Visibility.Hidden;
                }
                else
                {
                    Border tempBorder = FindName($"Lesson{i + 1}") as Border;
                    tempBorder.Visibility = Visibility.Visible;

                    TextBlock tempTextBlock = FindName($"LessonBegin{i + 1}") as TextBlock;
                    tempTextBlock.Text = "Початок: " + LessonsData[i].LessonBegin.ToString(@"hh\:mm");

                    tempTextBlock = this.FindName($"LessonEnd{i + 1}") as TextBlock;
                    tempTextBlock.Text = "Кінець: " + LessonsData[i].LessonEnd.ToString(@"hh\:mm");

                    tempTextBlock = FindName($"LessonUp{i + 1}") as TextBlock;
                    tempTextBlock.Text = LessonsData[i].Lessons1.lesson;

                    tempTextBlock = FindName($"LessonDown{i + 1}") as TextBlock;
                    tempTextBlock.Text = LessonsData[i].Lessons2.lesson;
                }
            }
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RadioButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            Button tempButton = sender as Button;
            switch (tempButton.Name)
            {
                case "Monday":
                    {
                        FormLesson(LessonsData.Monday);
                    }
                    break;

                case "Thuesday":
                    {
                        FormLesson(LessonsData.Thuesday);
                    }
                    break;

                case "Wednesday":
                    {
                        FormLesson(LessonsData.Wednesday);
                    }
                    break;

                case "Thursday":
                    {
                        FormLesson(LessonsData.Thursday);
                    }
                    break;

                case "Friday":
                    {
                        FormLesson(LessonsData.Friday);
                    }
                    break;

                default:
                    {
                        throw new Exception("Інши день не підтримується");
                    }
            }
        }
    }
}
