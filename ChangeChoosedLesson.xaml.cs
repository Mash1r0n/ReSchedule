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
using System.Windows.Threading;

namespace ReSchedule
{
    /// <summary>
    /// Логика взаимодействия для ChangeChoosedLesson.xaml
    /// </summary>
    public partial class ChangeChoosedLesson : Window
    {
        List<LessonPair> LessonsPairData;
        AllLessons LessonsData;
        int NumOfWeek;

        DispatcherTimer CheckForFill;
        public ChangeChoosedLesson(AllLessons lessonsData,  List<LessonPair> lessonsPairData)
        {
            InitializeComponent();

            LessonsPairData = lessonsPairData;

            LessonsData = lessonsData;

            NumOfWeek = ManageLessons.GetWeekNumber();

            ChoosingDayFor.Text = "Зміна роскладу за день: " + ManageLessons.GetDaysNameByNumber(NumOfWeek);

            SetValues();

            CheckForFill = new DispatcherTimer();
            CheckForFill.Tick += (s, e) =>
            {
                if (IsAllTextBlockAreNotEmpty())
                {
                    ApplyChanges.IsEnabled = true;
                }
                else
                {
                    ApplyChanges.IsEnabled = false;
                }
            };
            CheckForFill.Start();
        }

        bool IsAllTextBlockAreNotEmpty()
        {
            TextBox tempTextBlockL1;
            TextBox tempTextBlockL2;
            for (int i = 0; i < LessonsPairData.Count; i++)
            {
                tempTextBlockL1 = FindName($"C{i + 1}") as TextBox;
                tempTextBlockL2 = FindName($"Sn{i + 1}") as TextBox;

                if (tempTextBlockL1.Text == "" || tempTextBlockL2.Text == "")
                {
                    return false;
                }
            }

            return true;
        }

        void SetValues()
        {
            for (int i = 0; i < LessonsPairData.Count; i++)
            {
                TextBox tempTextBox = FindName($"C{i + 1}") as TextBox;
                tempTextBox.Text = LessonsPairData[i].Lessons1.lesson;

                tempTextBox = FindName($"Sn{i + 1}") as TextBox;
                tempTextBox.Text = LessonsPairData[i].Lessons2.lesson;
            }
        }

        void ApplyValues()
        {
            TextBox tempTextBoxL1;
            TextBox tempTextBoxL2;
            List<LessonPair> tempListLessonPair = new List<LessonPair>();
            for (int i = 0; i < LessonsPairData.Count; i++)
            {
                tempTextBoxL1 = FindName($"C{i + 1}") as TextBox;
                tempTextBoxL2 = FindName($"Sn{i + 1}") as TextBox;

                tempListLessonPair.Add(new LessonPair(new Lesson(tempTextBoxL1.Text), new Lesson(tempTextBoxL2.Text), LessonsPairData[i].LessonBegin, LessonsPairData[i].LessonEnd));
            }

            LessonsData.SetList(NumOfWeek, tempListLessonPair);

            ManageLessons.SetNewLessonInfo(LessonsData);
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            ApplyValues();
            CheckForFill.Stop();
            Close();
        }

        private void DeclineChanges_Click(object sender, RoutedEventArgs e)
        {
            CheckForFill.Stop();
            Close();
        }
    }
}
