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
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        AllInfo CurrentSettings;
        public SettingsWindow(AllInfo Settings)
        {
            InitializeComponent();

            FunctionalOfProgram.Visibility = Visibility.Visible;
            ChangeLessons.Visibility = Visibility.Hidden;
            ImportAndExport.Visibility = Visibility.Hidden;

            CurrentSettings = Settings;

            ParseSettings();
        }

        void ParseSettings()
        {
            StartLessonMessage.IsChecked = CurrentSettings.Properties.MessageAboutLessonStart;
            EndLessonMessage.IsChecked = CurrentSettings.Properties.MessageAboutLessonEnd;

            TimeBeforeCurrentLessonEnd.IsChecked = CurrentSettings.Properties.TimeBeforeEndOfCurrentLesson;
            TimeBeforeBeginNextLesson.IsChecked = CurrentSettings.Properties.TimeBeforeBeginNextLesson;

            RunWithSystem.IsChecked = CurrentSettings.Properties.StartWithSystem; 
        }

        void SetSettings()
        {
            SettingsProperty tempSettings = new SettingsProperty();

            tempSettings.MessageAboutLessonStart = StartLessonMessage.IsChecked;
            tempSettings.MessageAboutLessonEnd = EndLessonMessage.IsChecked;

            tempSettings.TimeBeforeEndOfCurrentLesson = TimeBeforeCurrentLessonEnd.IsChecked;
            tempSettings.TimeBeforeBeginNextLesson = TimeBeforeBeginNextLesson.IsChecked;

            tempSettings.StartWithSystem = RunWithSystem.IsChecked;

            CurrentSettings.SetSettings(tempSettings);
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();

            this.Close();
        }

        private void OpenLessonChangeWindow_Click(object sender, RoutedEventArgs e)
        {
            Button ButtonObject = sender as Button;
            ChangeChoosedLesson changeChoosedLesson = null;
            switch (ButtonObject.Name)
            {
                case "ChangeMonday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Monday);  
                }
                break;

                case "ChangeThuesday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Thuesday);
                }
                break;

                case "ChangeWednesday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Wednesday);
                }
                break;

                case "ChangeThursday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Thursday);
                }
                break;

                case "ChangeFriday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Friday);
                }
                break;

                default: { throw new Exception("Непередбачувана дія"); }
            }

            changeChoosedLesson.Owner = this;
            changeChoosedLesson.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changeChoosedLesson.ShowDialog();
        }

        private void ChangePageOfSettings_Click(object sender, RoutedEventArgs e)
        {
            RadioButton tempRadioButton = sender as RadioButton;

            switch (tempRadioButton.Name)
            {
                case "FunctionalPage": 
                {
                    FunctionalOfProgram.Visibility = Visibility.Visible;
                    ChangeLessons.Visibility = Visibility.Hidden;
                    ImportAndExport.Visibility = Visibility.Hidden;
                }
                break;

                case "ChangeLessonsPage": 
                {
                    FunctionalOfProgram.Visibility = Visibility.Hidden;
                    ChangeLessons.Visibility = Visibility.Visible;
                    ImportAndExport.Visibility = Visibility.Hidden;
                }
                break;

                case "ImportAndExportPage":
                {
                    FunctionalOfProgram.Visibility = Visibility.Hidden;
                    ChangeLessons.Visibility = Visibility.Hidden;
                    ImportAndExport.Visibility = Visibility.Visible;
                } break;

                default: { throw new Exception("Інших дій не передбачено"); }
            }
        }
    }
}
