﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        MainWindow WindowData;
        public SettingsWindow(ref AllInfo Settings, MainWindow windowData)
        {
            InitializeComponent();

            FunctionalOfProgram.Visibility = Visibility.Visible;
            ChangeLessons.Visibility = Visibility.Hidden;
            ImportAndExport.Visibility = Visibility.Hidden;

            CurrentSettings = Settings;

            WindowData = windowData;

            ParseSettings();
        }

        void ParseSettings()
        {
            StartLessonMessage.IsChecked = CurrentSettings.Properties.MessageAboutLessonStart;
            EndLessonMessage.IsChecked = CurrentSettings.Properties.MessageAboutLessonEnd;

            TimeBeforeCurrentLessonEnd.IsChecked = CurrentSettings.Properties.TimeBeginEndOfLesson;
            TimeBeforeBeginNextLesson.IsChecked = CurrentSettings.Properties.NextLesson;

            RunWithSystem.IsChecked = AutoStartManager.IsInStartup(); 
        }

        void SetSettings()
        {
            SettingsProperty tempSettings = new SettingsProperty();

            tempSettings.MessageAboutLessonStart = StartLessonMessage.IsChecked;
            tempSettings.MessageAboutLessonEnd = EndLessonMessage.IsChecked;

            tempSettings.TimeBeginEndOfLesson = TimeBeforeCurrentLessonEnd.IsChecked;
            tempSettings.NextLesson = TimeBeforeBeginNextLesson.IsChecked;

            tempSettings.StartWithSystem = RunWithSystem.IsChecked;

            CurrentSettings.Properties.SetAllProperties(CurrentSettings.SetSettings(tempSettings));
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();

            ManageLessons.UpgradeSettingsData(CurrentSettings);

            ManageLessons.RemakeContextMenuMouseOver();

            WindowData.WriteInfoInFile(CurrentSettings);

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
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Monday, WindowData, 1);  
                }
                break;

                case "ChangeThuesday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Thuesday, WindowData, 1);
                }
                break;

                case "ChangeWednesday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Wednesday, WindowData, 3);
                }
                break;

                case "ChangeThursday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Thursday, WindowData, 4);
                }
                break;

                case "ChangeFriday": 
                {
                    changeChoosedLesson = new ChangeChoosedLesson(CurrentSettings, CurrentSettings.Friday, WindowData, 5);
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

        private void WriteInfoInFile(AllInfo allinfo, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(allinfo);

            using (StreamWriter write = new StreamWriter(filePath, false))
            {
                write.WriteLineAsync(jsonString);
            }
        }

        private void WriteLessonsInFile(AllLessons alllessons, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(alllessons);

            using (StreamWriter write = new StreamWriter(filePath, false))
            {
                write.WriteLineAsync(jsonString);
            }
        }

        private bool ReadInfoFromFile(ref AllInfo obj, string filePath)
        {
            obj = new AllInfo();

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);

                obj.SetNewData(JsonConvert.DeserializeObject<AllInfo>(jsonString));

                ManageLessons.StartManage(obj, WindowData);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool ReadLessonsFromFile(ref AllInfo obj, string filePath)
        {
            AllLessons tempLessons;

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                tempLessons = JsonConvert.DeserializeObject<AllLessons>(jsonString);

                obj.SetList(1, tempLessons.Monday);
                obj.SetList(2, tempLessons.Thuesday);
                obj.SetList(3, tempLessons.Wednesday);
                obj.SetList(4, tempLessons.Thursday);
                obj.SetList(5, tempLessons.Friday);

                ManageLessons.StartManage(obj, WindowData);
            }
            catch
            {
                return false;
            }

            return true;
        }

        void ExportImportButtonClose(bool LessonOrAll) //true - Lesson, false - all
        {
            if (LessonOrAll)
            {
                WindowData.SetNewLessonsInformation(CurrentSettings);
            }
            else
            {
                WindowData.SetNewInfolmationAboutProgram(CurrentSettings);
            }
            ManageLessons.StartManage(WindowData.GetInfoForAllProgram(), WindowData);
            ManageLessons.RemakeContextMenuMouseOver();
            Close();
        }

        private void ImportLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            string AnyFilePath;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Відкрити файл занять";
            openFileDialog.Filter = "Файли з даними занять (*.drl)|*.drl";
            bool? openResult = openFileDialog.ShowDialog();

            if (openResult == true)
            {
                AnyFilePath = openFileDialog.FileName;
                ReadLessonsFromFile(ref CurrentSettings, AnyFilePath);
                ExportImportButtonClose(true);
            }
        }

        private void ImportAllInfo_Click(object sender, RoutedEventArgs e)
        {
            string AnyFilePath;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Відкрити файл з даними програми";
            openFileDialog.Filter = "Файли з даними програми (*.drs)|*.drs";
            bool? openResult = openFileDialog.ShowDialog();

            if (openResult == true)
            {
                AnyFilePath = openFileDialog.FileName;
                ReadInfoFromFile(ref CurrentSettings, AnyFilePath);
                ExportImportButtonClose(false);
            }
        }

        private void ExportLessons_Click(object sender, RoutedEventArgs e)
        {
            string AnyFilePath;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Файли з даними занять (*.drl)|*.drl";
            saveFileDialog.Title = "Зберегти заняття";
            bool? saveResult = saveFileDialog.ShowDialog();
            if (saveResult == true)
            {
                AnyFilePath = saveFileDialog.FileName;
                WriteLessonsInFile(CurrentSettings, AnyFilePath);
                ExportImportButtonClose(true);
            }
        }

        private void ExportAllInfo_Click(object sender, RoutedEventArgs e)
        {
            string AnyFilePath;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Файли з даними програми (*.drs)|*.drs";
            saveFileDialog.Title = "Зберегти усі дані";
            bool? saveResult = saveFileDialog.ShowDialog();

            if (saveResult == true)
            {
                AnyFilePath = saveFileDialog.FileName;
                WriteInfoInFile(CurrentSettings, AnyFilePath);
                ExportImportButtonClose(false);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void RunWithSystem_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tempToggleButton = sender as ToggleButton;
            if (tempToggleButton.IsChecked == false) {
                if (!AutoStartManager.RemoveFromStartup())
                {
                    MessageBox.Show("Помилка!", "У вас недостатньо для виконання цієї дії", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (!AutoStartManager.AddToStartup())
                {
                    MessageBox.Show("Помилка!", "У вас недостатньо для виконання цієї дії", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
