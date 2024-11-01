﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ReSchedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public struct Lesson
    {
        [JsonProperty]
        public string? lesson { get; private set; }
        public Lesson(string? lesson)
        {
            this.lesson = lesson;
        }
    }

    public struct LessonPair
    {
        [JsonProperty]
        public TimeSpan LessonBegin { get; private set; }
        [JsonProperty]
        public TimeSpan LessonEnd { get; private set; }
        [JsonProperty]
        public Lesson Lessons1 { get; private set; }
        [JsonProperty]
        public Lesson Lessons2 { get; private set; }

        public LessonPair(Lesson l1, Lesson l2, TimeSpan LB, TimeSpan LE)
        {
            Lessons1 = l1;
            Lessons2 = l2;
            LessonBegin = LB;
            LessonEnd = LE;
        }
    }

    public class AllLessons()
    {
        [JsonProperty]
        public List<LessonPair> Monday { get; private set; } = new List<LessonPair>();
        [JsonProperty]
        public List<LessonPair> Thuesday { get; private set; } = new List<LessonPair>();
        [JsonProperty]
        public List<LessonPair> Wednesday { get; private set; } = new List<LessonPair>();
        [JsonProperty]
        public List<LessonPair> Thursday { get; private set; } = new List<LessonPair>();
        [JsonProperty]
        public List<LessonPair> Friday { get; private set; } = new List<LessonPair>();

        [JsonIgnore]
        public List<LessonPair> Saturday { get; private set; } = new List<LessonPair>();

        [JsonIgnore]
        public List<LessonPair> Sunday { get; private set; } = new List<LessonPair>();

        public List<LessonPair> GetDaysLessons(int NumberOfDay)
        {
            switch (NumberOfDay)
            {
                case 1: return Monday; 
                case 2: return Thuesday;
                case 3: return Wednesday;
                case 4: return Thursday;
                case 5: return Friday;
                case 6: return Saturday;
                case 7: return Sunday;
                default: { throw new Exception("Для такого дня немає занять"); }
            }
        }

        public void SetList(int NumberOfDay,  List<LessonPair> List)
        {
            switch (NumberOfDay)
            {
                case 1:
                {
                    Monday = List;
                }break;

                case 2:
                {
                    Thuesday = List;
                }break;

                case 3: { 
                    Wednesday = List;
                }break;

                case 4: { 
                    Thursday = List;
                }break;

                case 5: {
                    Friday = List;
                }break;

                default:
                {
                    throw new Exception("Не існує заняття на такий день");
                }
            }
        }
    }

    public struct SettingsProperty
    {
        //Messages
        public bool? MessageAboutLessonStart;
        public bool? MessageAboutLessonEnd;

        //Context menu
        public bool? TimeBeginEndOfLesson;
        public bool? NextLesson;

        //ETC
        public bool? StartWithSystem;

        public SettingsProperty()
        {
            MessageAboutLessonStart = false;
            MessageAboutLessonEnd = false;
            TimeBeginEndOfLesson = false;
            NextLesson = false;
            StartWithSystem = false;
        }

        public void SetAllProperties(SettingsProperty properties)
        {
            MessageAboutLessonStart = properties.MessageAboutLessonStart;
            MessageAboutLessonEnd = properties.MessageAboutLessonEnd;
            TimeBeginEndOfLesson = properties.TimeBeginEndOfLesson;
            NextLesson = properties.NextLesson;
            StartWithSystem = properties.StartWithSystem;
        }
    }

    public class AllInfo : AllLessons
    {
        [JsonProperty]
        public SettingsProperty Properties { get; private set; } = new SettingsProperty();

        public void SetNewData(AllInfo info)
        {
            Properties = info.Properties;
            SetList(1, info.Monday);
            SetList(2, info.Thuesday);
            SetList(3, info.Wednesday);
            SetList(4, info.Thursday);
            SetList(5, info.Friday);
        }

        public SettingsProperty SetSettings(SettingsProperty SProperty)
        {
            SettingsProperty originalSettings = Properties;
            originalSettings.SetAllProperties(SProperty); 

            Properties = originalSettings;
            return Properties;
        }

    }

    struct LessonInfo
    {
        public Lesson NameOfLesson { get; private set; }
        public TimeSpan LessonBegin { get; private set; }
        public TimeSpan LessonEnd { get; private set; }

        public LessonInfo(Lesson name, TimeSpan begin, TimeSpan end) {
            NameOfLesson = name;
            LessonBegin = begin;
            LessonEnd = end;
        }
    }

    struct ManageActiveLessonsBorder
    {
        public Border CurrentLessonBorder { get; private set; }

        public bool IsColorized { get; private set; }

        public ManageActiveLessonsBorder(Border border)
        {
            CurrentLessonBorder = border;
            IsColorized = false;
        }

        public void SetNewLessonBorder(Border newBorder, bool EndCurrentBorder)
        {
            if (EndCurrentBorder)
            {
                EndLessonBorder();
            }

            CurrentLessonBorder = newBorder;
        }

        public void StartLessonBorder()
        {
            if (CurrentLessonBorder != null)
            {
                CurrentLessonBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350"));

                ColorAnimation colorAnimation = new ColorAnimation
                {
                    From = ((SolidColorBrush)CurrentLessonBorder.Background).Color,
                    To = (Color)ColorConverter.ConvertFromString("#5523AF7F"),
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                CurrentLessonBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                IsColorized = true;
            }
        }

        public void EndLessonBorder()
        {
            if (CurrentLessonBorder != null)
            {
                CurrentLessonBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5523AF7F"));

                ColorAnimation colorAnimation = new ColorAnimation
                {
                    From = ((SolidColorBrush)CurrentLessonBorder.Background).Color,
                    To = (Color)ColorConverter.ConvertFromString("#233350"),
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                CurrentLessonBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                IsColorized = false;
            }
        }
    }

    public static class AutoStartManager
    {
        private const string AppName = "ReSchedule";
        private const string RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static bool AddToStartup()
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

                if (rkApp == null)
                    rkApp = Registry.CurrentUser.CreateSubKey(RegistryKey);

                rkApp.SetValue(AppName, System.Reflection.Assembly.GetExecutingAssembly().Location);

                rkApp.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RemoveFromStartup()
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

                if (rkApp == null)
                    return true;

                rkApp.DeleteValue(AppName, false);

                rkApp.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInStartup()
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

                if (rkApp == null)
                    return false;

                string[] valueNames = rkApp.GetValueNames();
                foreach (string valueName in valueNames)
                {
                    if (valueName == AppName)
                    {
                        rkApp.Close();
                        return true;
                    }
                }

                rkApp.Close();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    static class ManageLessons
    {
        static List<LessonInfo> InformationAboutLessons = new List<LessonInfo>();

        static AllInfo CurrentSettings;

        static MainWindow AllElementsInWindow;

        static DispatcherTimer ManageLoop;

        static DispatcherTimer MessageForLesson;

        static DispatcherTimer SyncForMessages;

        static ManageActiveLessonsBorder ManagedBorder;

        static DateTime CurrentDate;

        public static void UpgradeSettingsData(AllInfo updSettings)
        {
            CurrentSettings = updSettings;
        }

        public static void RemakeContextMenuMouseOver()
        {
            AllElementsInWindow.InformationToolTip.Children.Clear();

            TextBlock textBlock = new TextBlock
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CED6E3")),
                FontSize = 16,
                FontFamily = new FontFamily("Inter"),
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30,
                Padding = new Thickness(0, 8, 0, 0)
            };

            Run run = new Run
            {
                FontWeight = FontWeights.Regular,
                Text = "ReSchedule"
            };

            textBlock.Inlines.Add(run);

            AllElementsInWindow.InformationToolTip.Children.Add(textBlock);

            if (CurrentSettings.Properties.TimeBeginEndOfLesson == true)
            {
                Separator separator = new Separator
                {
                    Width = 147,
                };

                AllElementsInWindow.InformationToolTip.Children.Add(separator);

                TextBlock txtBlock = new TextBlock
                {
                    //Name = "ContextTimeBeforeEndOfCurrentLesson",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CED6E3")),
                    FontSize = 14,
                    FontFamily = new FontFamily("Inter"),
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 30,
                    Padding = new Thickness(3, 7, 3, 7)
                };

                run = new Run
                {
                    FontWeight = FontWeights.Light,
                    Text = "Завантаження даних...",
                };

                txtBlock.SetValue(FrameworkElement.NameProperty, "ContextTimeBeforeEndOfCurrentLesson");

                txtBlock.Inlines.Add(run);

                AllElementsInWindow.InformationToolTip.Children.Add(txtBlock);
            }

            if (CurrentSettings.Properties.NextLesson == true)
            {
                Separator separator = new Separator
                {
                    Width = 147,
                };

                AllElementsInWindow.InformationToolTip.Children.Add(separator);

                TextBlock txtBlock = new TextBlock
                {
                    //Name = "ContextTimeBeforeEndOfCurrentLesson",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CED6E3")),
                    FontSize = 14,
                    FontFamily = new FontFamily("Inter"),
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(3, 7, 3, 7)
                };

                run = new Run
                {
                    FontWeight = FontWeights.Light,
                    Text = "Наступна пара завантажується...",
                };

                txtBlock.SetValue(FrameworkElement.NameProperty, "ContextNextLesson");

                txtBlock.Inlines.Add(run);

                AllElementsInWindow.InformationToolTip.Children.Add(txtBlock);
            }
        }

        static public int CurrentNumberOfDay { get; private set; }

        static public  string CurrentNameOfDay { get; private set; }

        public const int CountOfManagedDays = 5; 

        static public void StartManage(AllInfo AllCurrentInfo, MainWindow AEIW)
        {
            CurrentNumberOfDay = GetWeekNumber();

            CurrentNameOfDay = GetDaysNameByNumber(CurrentNumberOfDay);

            AllElementsInWindow = AEIW;

            ManagedBorder = new ManageActiveLessonsBorder(AllElementsInWindow.FindName("BorderLesson1") as Border);

            CurrentDate = DateTime.Now;

            List<LessonPair> lesson = AllCurrentInfo.GetDaysLessons(CurrentNumberOfDay);

            if (lesson.Count == 0)
            {
                AEIW.BorderLesson1.Visibility = Visibility.Hidden;
                AEIW.BorderLesson2.Visibility = Visibility.Hidden;
                AEIW.BorderLesson3.Visibility = Visibility.Hidden;
                AEIW.BorderLesson4.Visibility = Visibility.Hidden;
                AEIW.BorderLesson5.Visibility = Visibility.Hidden;
                AEIW.BorderLesson6.Visibility = Visibility.Hidden;
                AEIW.WeekendPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AEIW.BorderLesson1.Visibility = Visibility.Visible;
                AEIW.BorderLesson2.Visibility = Visibility.Visible;
                AEIW.BorderLesson3.Visibility = Visibility.Visible;
                AEIW.BorderLesson4.Visibility = Visibility.Visible;
                AEIW.BorderLesson5.Visibility = Visibility.Visible;
                AEIW.BorderLesson6.Visibility = Visibility.Visible;
                AEIW.WeekendPanel.Visibility = Visibility.Hidden;
            }

            if (lesson.Count != 0)
            {
                InformationAboutLessons = new List<LessonInfo>();

                for (int i = 0; i < 6; i++)
                {
                    InformationAboutLessons.Add(new LessonInfo((GetWeekMode() ? lesson[i].Lessons2 : lesson[i].Lessons1), lesson[i].LessonBegin, lesson[i].LessonEnd));
                }
            }

            CurrentSettings = AllCurrentInfo;

            ManageLoop = new DispatcherTimer();

            MessageForLesson = new DispatcherTimer();

            SyncForMessages = new DispatcherTimer();

            SetSettings();

            RemakeContextMenuMouseOver();

            SyncMessages();
        }

        static void SyncMessages()
        {
            SyncForMessages.Interval = TimeSpan.FromSeconds(1);

            SyncForMessages.Tick += (s, e) => {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                if (MessageForLesson.IsEnabled)
                {
                    SyncForMessages.Stop();
                }

                else if (currentTime.Seconds == 0)
                {
                    StartListenForNotification();
                    SyncForMessages.Stop();
                }
            };

            SyncForMessages.Start();
        }

        static bool CheckForAnothersLessons(int BeginNumber)
        {
            for (int i = BeginNumber+1; i < InformationAboutLessons.Count; i++)
            {
                if (InformationAboutLessons[i].NameOfLesson.lesson != "-")
                {
                    return true;
                }
            }
            return false;
        }

        static void StartListenForNotification()
        {
            TimeSpan TimeDiff = TimeSpan.FromMinutes(3);

            MessageForLesson.Interval = TimeSpan.FromSeconds(1);

            MessageForLesson.Tick += (s, e) =>
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                currentTime = new TimeSpan(currentTime.Hours, currentTime.Minutes, 0);
                for (int i = 0; i < InformationAboutLessons.Count; i++)
                {
                    
                    if (currentTime + TimeDiff == InformationAboutLessons[i].LessonBegin && InformationAboutLessons[i].NameOfLesson.lesson != "-")
                    {
                        PushUpMessage pushUpMessage = new PushUpMessage(AllElementsInWindow);
                        AllElementsInWindow.AddMessageToList(pushUpMessage);
                        pushUpMessage.ShowTheMessage(PushUpMessage.BeginOfLesson, $"Почнеться заняття: {InformationAboutLessons[i].NameOfLesson.lesson}", $"Через {TimeDiff.ToString("mm")} хв");
                        break;
                    }
                    else if (currentTime == InformationAboutLessons[i].LessonEnd)
                    {
                        PushUpMessage pushUpMessage = new PushUpMessage(AllElementsInWindow);
                        AllElementsInWindow.AddMessageToList(pushUpMessage);
                        if (i + 1 > InformationAboutLessons.Count)
                        {
                            pushUpMessage.ShowTheMessage(PushUpMessage.EndOfLessons, $"Більше занять на сьогодні немає");
                            break;
                        }
                        else if (!CheckForAnothersLessons(i) && currentTime < InformationAboutLessons[InformationAboutLessons.Count - 1].LessonEnd)
                        {
                            pushUpMessage.ShowTheMessage(PushUpMessage.EndOfLessons, $"Більше занять на сьогодні немає");
                            break;
                        }
                        else if (InformationAboutLessons[i].NameOfLesson.lesson != "-")
                        {
                            pushUpMessage.ShowTheMessage(PushUpMessage.EndOfLesson, $"Наступне заняття: {InformationAboutLessons[i+1].NameOfLesson.lesson}", $"Через {(InformationAboutLessons[i+1].LessonBegin - currentTime).ToString("mm")} хв");
                            break;
                        }
                        
                    }
                }

                if (MessageForLesson.Interval == TimeSpan.FromSeconds(1))
                {
                    MessageForLesson.Interval = TimeSpan.FromMinutes(1);
                }
            };

            MessageForLesson.Start();
        }

        static public void SetNewLessonInfo(AllLessons AllCurrentInfo)
        {
            List<LessonPair> lesson = AllCurrentInfo.GetDaysLessons(CurrentNumberOfDay);

            InformationAboutLessons = new List<LessonInfo>();

            for (int i = 0; i < 6; i++)
            {
                InformationAboutLessons.Add(new LessonInfo((GetWeekMode() ? lesson[i].Lessons2 : lesson[i].Lessons1), lesson[i].LessonBegin, lesson[i].LessonEnd));
            }

            SetSettings();
        }

        public static string GetDaysNameByNumber(int DaysNumber)
        {
            switch (DaysNumber)
            {
                case 1: return "Понеділок";
                case 2: return "Вівторок";
                case 3: return "Середа";
                case 4: return "Четвер";
                case 5: return "П'ятниця";
                case 6: return "Субота";
                case 7: return "Неділя";
                default: throw new Exception("Такого дня не існує");
            }
        }
        
        static void TimeBeforeEndOfLessonCreate()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            
            for (int i = 0; i < InformationAboutLessons.Count; i++)
            {
                if (currentTime < InformationAboutLessons[i].LessonBegin)
                {
                    TextBlock tempTextBlock = AllElementsInWindow.FindName($"LessonStatus{i + 1 }") as TextBlock;
                    tempTextBlock.Text = "До закінчення (хв): Не почалось";
                }
                else if(currentTime > InformationAboutLessons[i].LessonEnd)
                {
                    TextBlock tempTextBlock = AllElementsInWindow.FindName($"LessonStatus{i + 1}") as TextBlock;
                    tempTextBlock.Text = "До закінчення (хв): Завершено";
                }
                else if (currentTime > InformationAboutLessons[i].LessonBegin && currentTime < InformationAboutLessons[i].LessonEnd)
                {
                    TimeSpan TimeDiff = InformationAboutLessons[i].LessonEnd - currentTime;

                    TextBlock tempTextBlock = AllElementsInWindow.FindName($"LessonStatus{i + 1}") as TextBlock;
                    tempTextBlock.Text = "До закінчення (хв): " + ((int)TimeDiff.TotalMinutes + 1).ToString();
                }
            }
        }

        static void ContextTimeBeforeEndOfLesson()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            TimeSpan TimeDiff = new TimeSpan();

            TextBlock tempContextTempBlock = AllElementsInWindow.InformationToolTip.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Name == "ContextTimeBeforeEndOfCurrentLesson");
            
            TextBlock tempContextTempBlockNextLesson = AllElementsInWindow.InformationToolTip.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Name == "ContextNextLesson");

            if (tempContextTempBlockNextLesson != null)
            {
                tempContextTempBlockNextLesson.Text = "Наступна пара: відсутня";
            }

            //if (tempContextTempBlock != null)
            //{
            //    tempContextTempBlock.Text = "До закінчення (хв): " + ((int)TimeDiff.TotalMinutes + 1).ToString();
            //}
            bool AlreadyHaveANextLesson = false;

            //TODO: сделать уведомления.

            for (int i = 0; i < InformationAboutLessons.Count; i++)
            {
                if (currentTime < InformationAboutLessons[i].LessonEnd)
                {
                    if (InformationAboutLessons[i].NameOfLesson.lesson == "-")
                    {
                        ManagedBorder.SetNewLessonBorder(AllElementsInWindow.FindName($"BorderLesson{0}") as Border, ManagedBorder.IsColorized);
                    }

                    else
                    {
                        if (ManagedBorder.CurrentLessonBorder.Name != $"BorderLesson{i + 1}")
                        {
                            ManagedBorder.SetNewLessonBorder(AllElementsInWindow.FindName($"BorderLesson{i + 1}") as Border, ManagedBorder.IsColorized);
                        }

                        if (currentTime > InformationAboutLessons[i].LessonBegin)
                        {
                            if (!ManagedBorder.IsColorized)
                            {
                                ManagedBorder.StartLessonBorder();
                            }
                        }
                        else
                        {
                            if (ManagedBorder.IsColorized)
                            {
                                ManagedBorder.EndLessonBorder();
                            }
                        }
                    }
                }

                if (currentTime < InformationAboutLessons[i].LessonEnd)
                {
                    if (!AlreadyHaveANextLesson)
                    {
                        AlreadyHaveANextLesson = true;
                        if (tempContextTempBlockNextLesson != null)
                        {
                            if (i+1 >= InformationAboutLessons.Count)
                            {
                                tempContextTempBlockNextLesson.Text = "Наступної пари немає";
                            }
                            else if (currentTime < InformationAboutLessons[i].LessonBegin)
                            {
                                tempContextTempBlockNextLesson.Text = $"Наступна пара: {InformationAboutLessons[i].NameOfLesson.lesson}";
                            }
                            else
                            {
                                tempContextTempBlockNextLesson.Text = $"Наступна пара: {InformationAboutLessons[i+1].NameOfLesson.lesson}";
                            }
                        }
                    }
                    if (currentTime > InformationAboutLessons[i].LessonBegin)
                    {
                        if (InformationAboutLessons[i].NameOfLesson.lesson == "-")
                        {
                            if (tempContextTempBlock != null)
                            {
                                tempContextTempBlock.Text = "До закінчення (хв): -";
                            }
                            break;
                        }

                        TimeDiff = InformationAboutLessons[i].LessonEnd - currentTime;

                        if (tempContextTempBlock != null)
                        {
                            tempContextTempBlock.Text = "До закінчення (хв): " + ((int)TimeDiff.TotalMinutes + 1).ToString();
                        }
                        break;
                    }
                    else
                    {
                        TimeDiff = InformationAboutLessons[i].LessonBegin - currentTime;

                        if (tempContextTempBlock != null)
                        {
                            tempContextTempBlock.Text = "До початку (хв): " + ((int)TimeDiff.TotalMinutes + 1).ToString();
                        }
                        break;
                    }
                }
                else
                {
                    if (tempContextTempBlock != null && tempContextTempBlockNextLesson != null)
                    {
                        tempContextTempBlock.Text = "Пари закінчились!";
                        tempContextTempBlockNextLesson.Text = "Наступної пари немає";
                    }
                }
            }
            
        }

        static public void SetSettings()
        {
            AllElementsInWindow.TodaysDay.Text = CurrentNameOfDay;
            AllElementsInWindow.WeekModeNow.Text = ReturnNameOfWeekMode(GetWeekMode());
            ManageLoop.Interval = TimeSpan.FromSeconds(1);

            ManageLoop.Tick += (s, e) =>
            {
                string CurrentTextTime = "";
                var TempDate = DateTime.Now;
                CurrentTextTime = TempDate.ToString("HH : mm");

                TimeBeforeEndOfLessonCreate();

                ContextTimeBeforeEndOfLesson();

                if (TempDate.Day != CurrentDate.Day || TempDate.Month != CurrentDate.Month || TempDate.Year != CurrentDate.Year)
                {
                    StartManage(CurrentSettings, AllElementsInWindow);
                }

                AllElementsInWindow.CurrentTime.Text = CurrentTextTime;
            };

            ManageLoop.Start();

            for (int i = 0; i < InformationAboutLessons.Count; i++)
            {
                if (InformationAboutLessons[i].NameOfLesson.lesson == "-")
                {
                    Rectangle tempRectg = AllElementsInWindow.FindName($"WithoutLesson{i + 1}") as Rectangle;
                    tempRectg.Visibility = Visibility.Visible;

                    Grid tempGrid = AllElementsInWindow.FindName($"LessonsInfo{i + 1}") as Grid;
                    tempGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    Rectangle tempRectg = AllElementsInWindow.FindName($"WithoutLesson{i + 1}") as Rectangle;
                    tempRectg.Visibility = Visibility.Hidden;

                    Grid tempGrid = AllElementsInWindow.FindName($"LessonsInfo{i + 1}") as Grid;
                    tempGrid.Visibility = Visibility.Visible;

                    var tempTextBlock = AllElementsInWindow.FindName($"LessonName{i + 1}") as TextBlock;
                    tempTextBlock.Text = InformationAboutLessons[i].NameOfLesson.lesson;

                    tempTextBlock = AllElementsInWindow.FindName($"LessonBegin{i + 1}") as TextBlock;
                    tempTextBlock.Text = "Початок: " + InformationAboutLessons[i].LessonBegin.ToString(@"hh\:mm");

                    tempTextBlock = AllElementsInWindow.FindName($"LessonEnd{i + 1}") as TextBlock;
                    tempTextBlock.Text = "Кінець: " + InformationAboutLessons[i].LessonEnd.ToString(@"hh\:mm");
                }
            }
        }

        static bool GetWeekMode() //true - Знаменник, false - Чисельник
        {
            DateTime StartDate = new DateTime(2023, 1, 1);
            TimeSpan diff = DateTime.Now - StartDate;
            int diffWeeks = (int)diff.TotalDays / 7;
            return diffWeeks % 2 != 0;
        }

        static string ReturnNameOfWeekMode(bool Mode)
        {
            return (Mode ? "Знаменник" : "Чисельник");
        }

        public static int GetWeekNumber()
        {
            return ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1;
        }
    }

    public partial class MainWindow : Window
    {
        AllInfo InformationForAllProgram;

        List<PushUpMessage> pushUpMessagesList;

        int TickDropTime = 0;

        private const string MutexName = "Global\\ReSchedule359";
        private bool _mutexCreated;
        private Mutex _mutex;

        int TickTime = 0;

        int CurrentRegistrationPage = 0;
        DispatcherTimer timer;
        DispatcherTimer FillTImer = new DispatcherTimer();

        public void AddMessageToList(PushUpMessage message)
        {
            pushUpMessagesList.Add(message);
        }

        public AllInfo GetInfoForAllProgram()
        {
            return InformationForAllProgram;
        }

        public void SetNewInfolmationAboutProgram(AllInfo newInfo)
        {
            InformationForAllProgram.SetNewData(newInfo);
        }

        public void SetNewLessonsInformation(AllLessons newLessons)
        {
            InformationForAllProgram.SetList(1, newLessons.Monday);
            InformationForAllProgram.SetList(2, newLessons.Thuesday);
            InformationForAllProgram.SetList(3, newLessons.Wednesday);
            InformationForAllProgram.SetList(4, newLessons.Thursday);
            InformationForAllProgram.SetList(5, newLessons.Friday);
        }

        public MainWindow()
        {
            InitializeComponent();
            _mutex = new Mutex(true, MutexName, out _mutexCreated);

            if (!_mutexCreated)
            {
                MessageBox.Show("Програма вже запущена!", "Помлика запуску програми", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            bool isNewInstance;
            Mutex mutex = new Mutex(true, "{8F6F0AC5-B9A1-55fd-A8CF-73F04E6BDE8F}", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("Програма вже запущена!");
                Application.Current.Shutdown();
            }

            pushUpMessagesList = new List<PushUpMessage>();

            const int MinutesForRegistration = 7;
            const string TextAfterMinutesStages = "хвилин";
            const string TextForStages = "етапи";
            InformationForAllProgram = new AllInfo();

            //bool ShowRegistration = true;

            Registration.Visibility = Visibility.Hidden;

            //TodaysDay.Text = currentDay;
            //WeekModeNow.Text = (GetWeekMode() ? "Знаменник" : "Чисельник");

            if (!ReadInfoFromFile(out InformationForAllProgram))
            {
                Registration.Visibility = Visibility.Visible;
            }

            else
            {
                ManageLessons.StartManage(InformationForAllProgram, this);
            }

            Registration.ScrollToHorizontalOffset(1028);

            FormPanel.CreateBlock("Початок");
            FormPanel.CreateBlock("Заповнення");
            FormPanel.CreateBlock("Налаштування");
            FormPanel.CreateBlock("Кінець");

            StageLine.Children.Add(FormPanel.ReturnCompletedPanel());

            TextOnStartRegistration.Text = $"Перед тим, як почати користуватись програмою, ви маєте пройти через {FormPanel.ReturnCountOfStages()} {TextForStages}, що займуть у вас приблизно {MinutesForRegistration} {TextAfterMinutesStages}";

            Focus();
        }

        private void CloseWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Hide();
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
            AuthorWindow authorWindow = new AuthorWindow();
            authorWindow.Owner = this;
            authorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            authorWindow.ShowDialog();
        }

        void ScrollRegistration(bool TickSide) //false - праворуч true - ліворуч
        {
            timer = new DispatcherTimer();

            if (!TickSide && CurrentRegistrationPage > 0)
            {
                FormPanel.NextStage();
            }
            else if (CurrentRegistrationPage > 0)
            {
                FormPanel.PreviousStage();
            }

            timer.Interval = TimeSpan.FromMilliseconds(10);

            timer.Tick += (s, e) =>
            {
                if (TickTime == 16)
                {
                    TickTime = 0;
                    CurrentRegistrationPage += (!TickSide ? 1 : -1);
                    timer.Stop();
                }
                else if (!TickSide)
                {
                    TickTime++;
                    Registration.ScrollToHorizontalOffset(Registration.HorizontalOffset + 64.25);
                    
                    if (CurrentRegistrationPage > 0)
                    {
                        double currentLeftMargin = StageLine.Margin.Left;
                        StageLine.Margin = new Thickness(currentLeftMargin + 64.25, 0, 0, 0);
                    }

                }
                else
                {
                    TickTime++;
                    Registration.ScrollToHorizontalOffset(Registration.HorizontalOffset - 64.25);

                    if (CurrentRegistrationPage > 1)
                    {
                        double currentLeftMargin = StageLine.Margin.Left;
                        StageLine.Margin = new Thickness(currentLeftMargin - 64.25, 0, 0, 0);
                    }
                    
                }
            };

            timer.Start();  
        }
        
        void ManageNextPrev(bool ScrollSide) //false - праворуч true - ліворуч
        {
            if (!ScrollSide)
            {
                if (CurrentRegistrationPage == 0)
                {
                    NextScroll.Visibility = Visibility.Hidden;
                    ScrollRegistration(ScrollSide);

                }

                else if (CurrentRegistrationPage == 1)
                {
                    NextScroll.Visibility = Visibility.Visible;
                    NextScroll.IsEnabled = false;

                    FillTImer.Interval = TimeSpan.FromMilliseconds(100);

                    FillTImer.Tick += (s, e) =>
                    {
                        if (FillStatusOfFridayDownLessons() && FillStatusOfFridayUpLessons() && FillStatusOfMondayDownLessons() && FillStatusOfMondayUpLessons() && FillStatusOfThuesdayDownLessons() && FillStatusOfThuesdayUpLessons() && FillStatusOfThursdayDownLessons() && FillStatusOfThursdayUpLessons() && FillStatusOfWednesdayDownLessons() && FillStatusOfWednesdayUpLessons())
                        {
                            NextScroll.IsEnabled = true;
                        }
                        else
                        {
                            NextScroll.IsEnabled = false;
                        }
                    };

                    FillTImer.Start();

                    ScrollRegistration(ScrollSide);
                }

                else if (CurrentRegistrationPage == 2)
                {
                    FillTImer.Stop();
                    ScrollRegistration(ScrollSide);
                }
                else if (CurrentRegistrationPage == 3)
                {
                    ScrollRegistration(ScrollSide);
                    NextScroll.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (CurrentRegistrationPage == 1)
                {
                    NextScroll.Visibility = Visibility.Hidden;
                    ScrollRegistration(ScrollSide);
                    FillTImer.Stop();
                    FillTImer = new DispatcherTimer();
                }

                else if (CurrentRegistrationPage == 2)
                {
                    NextScroll.Visibility = Visibility.Hidden;
                    ScrollRegistration(ScrollSide);
                }
                else if (CurrentRegistrationPage == 3)
                {
                    FillTImer.Interval = TimeSpan.FromMilliseconds(100);

                    FillTImer.Tick += (s, e) =>
                    {
                        if (FillStatusOfFridayDownLessons() && FillStatusOfFridayUpLessons() && FillStatusOfMondayDownLessons() && FillStatusOfMondayUpLessons() && FillStatusOfThuesdayDownLessons() && FillStatusOfThuesdayUpLessons() && FillStatusOfThursdayDownLessons() && FillStatusOfThursdayUpLessons() && FillStatusOfWednesdayDownLessons() && FillStatusOfWednesdayUpLessons())
                        {
                            NextScroll.IsEnabled = true;
                        }
                        else
                        {
                            NextScroll.IsEnabled = false;
                        }
                    };

                    FillTImer.Start();
                    ScrollRegistration(ScrollSide);
                }
                else if(CurrentRegistrationPage == 4)
                {
                    ScrollRegistration(ScrollSide);
                    NextScroll.Visibility = Visibility.Visible;
                }
            }
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            ManageNextPrev(false);
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            ManageNextPrev(true);
        }

        private void BackFromDrop_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(10);

            timer.Tick += (s, e) =>
            {
                if (TickDropTime == 16)
                {
                    TickDropTime = 0;
                    timer.Stop();
                }
                TickDropTime++;
                Registration.ScrollToHorizontalOffset(Registration.HorizontalOffset + 64.25);

            };

            timer.Start();
        }

        private void DripFilePage_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(10);

            timer.Tick += (s, e) =>
            {
                if (TickDropTime == 16)
                {
                    TickDropTime = 0;
                    timer.Stop();
                }
                TickDropTime++;
                Registration.ScrollToHorizontalOffset(Registration.HorizontalOffset - 64.25);

            };

            timer.Start();
        }

        private void UpWeekButton(object sender, RoutedEventArgs e)
        {
            MondayLessonsC.Visibility = Visibility.Visible;
            ThuesdayLessonsC.Visibility = Visibility.Visible;
            WednesdayLessonsC.Visibility = Visibility.Visible;
            ThursdayLessonsC.Visibility = Visibility.Visible;
            FridayLessonsC.Visibility= Visibility.Visible;

            MondayLessonsSn.Visibility = Visibility.Hidden;
            ThuesdayLessonsSn.Visibility = Visibility.Hidden;
            WednesdayLessonsSn.Visibility = Visibility.Hidden;
            ThursdayLessonsSn.Visibility = Visibility.Hidden;
            FridayLessonsSn.Visibility = Visibility.Hidden;
        }

        private void DownWeekButton(object sender, RoutedEventArgs e)
        {
            MondayLessonsSn.Visibility = Visibility.Visible;
            ThuesdayLessonsSn.Visibility = Visibility.Visible;
            WednesdayLessonsSn.Visibility = Visibility.Visible;
            ThursdayLessonsSn.Visibility = Visibility.Visible;
            FridayLessonsSn.Visibility = Visibility.Visible;

            MondayLessonsC.Visibility = Visibility.Hidden;
            ThuesdayLessonsC.Visibility = Visibility.Hidden;
            WednesdayLessonsC.Visibility = Visibility.Hidden;
            ThursdayLessonsC.Visibility = Visibility.Hidden;
            FridayLessonsC.Visibility = Visibility.Hidden;
        }

        bool FillStatusOfMondayUpLessons()
        {
            if (!string.IsNullOrEmpty(M1C.Text))
            {
                if (!string.IsNullOrEmpty(M2C.Text))
                {
                    if (!string.IsNullOrEmpty(M3C.Text))
                    {
                        if (!string.IsNullOrEmpty(M4C.Text))
                        {
                            if (!string.IsNullOrEmpty(M5C.Text))
                            {
                                if (!string.IsNullOrEmpty(M6C.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfMondayDownLessons()
        {
            if (!string.IsNullOrEmpty(M1Sn.Text))
            {
                if (!string.IsNullOrEmpty(M2Sn.Text))
                {
                    if (!string.IsNullOrEmpty(M3Sn.Text))
                    {
                        if (!string.IsNullOrEmpty(M4Sn.Text))
                        {
                            if (!string.IsNullOrEmpty(M5Sn.Text))
                            {
                                if (!string.IsNullOrEmpty(M6Sn.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfThuesdayUpLessons()
        {
            if (!string.IsNullOrEmpty(TU1C.Text))
            {
                if (!string.IsNullOrEmpty(TU2C.Text))
                {
                    if (!string.IsNullOrEmpty(TU3C.Text))
                    {
                        if (!string.IsNullOrEmpty(TU4C.Text))
                        {
                            if (!string.IsNullOrEmpty(TU5C.Text))
                            {
                                if (!string.IsNullOrEmpty(TU6C.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfThuesdayDownLessons()
        {
            if (!string.IsNullOrEmpty(TU1Sn.Text))
            {
                if (!string.IsNullOrEmpty(TU2Sn.Text))
                {
                    if (!string.IsNullOrEmpty(TU3Sn.Text))
                    {
                        if (!string.IsNullOrEmpty(TU4Sn.Text))
                        {
                            if (!string.IsNullOrEmpty(TU5Sn.Text))
                            {
                                if (!string.IsNullOrEmpty(TU6Sn.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfWednesdayUpLessons()
        {
            if (!string.IsNullOrEmpty(W1C.Text))
            {
                if (!string.IsNullOrEmpty(W2C.Text))
                {
                    if (!string.IsNullOrEmpty(W3C.Text))
                    {
                        if (!string.IsNullOrEmpty(W4C.Text))
                        {
                            if (!string.IsNullOrEmpty(W5C.Text))
                            {
                                if (!string.IsNullOrEmpty(W6C.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfWednesdayDownLessons()
        {
            if (!string.IsNullOrEmpty(W1Sn.Text))
            {
                if (!string.IsNullOrEmpty(W2Sn.Text))
                {
                    if (!string.IsNullOrEmpty(W3Sn.Text))
                    {
                        if (!string.IsNullOrEmpty(W4Sn.Text))
                        {
                            if (!string.IsNullOrEmpty(W5Sn.Text))
                            {
                                if (!string.IsNullOrEmpty(W6Sn.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfThursdayUpLessons()
        {
            if (!string.IsNullOrEmpty(TH1C.Text))
            {
                if (!string.IsNullOrEmpty(TH2C.Text))
                {
                    if (!string.IsNullOrEmpty(TH3C.Text))
                    {
                        if (!string.IsNullOrEmpty(TH4C.Text))
                        {
                            if (!string.IsNullOrEmpty(TH5C.Text))
                            {
                                if (!string.IsNullOrEmpty(TH6C.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfThursdayDownLessons()
        {
            if (!string.IsNullOrEmpty(TH1Sn.Text))
            {
                if (!string.IsNullOrEmpty(TH2Sn.Text))
                {
                    if (!string.IsNullOrEmpty(TH3Sn.Text))
                    {
                        if (!string.IsNullOrEmpty(TH4Sn.Text))
                        {
                            if (!string.IsNullOrEmpty(TH5Sn.Text))
                            {
                                if (!string.IsNullOrEmpty(TH6Sn.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfFridayUpLessons()
        {
            if (!string.IsNullOrEmpty(F1C.Text))
            {
                if (!string.IsNullOrEmpty(F2C.Text))
                {
                    if (!string.IsNullOrEmpty(F3C.Text))
                    {
                        if (!string.IsNullOrEmpty(F4C.Text))
                        {
                            if (!string.IsNullOrEmpty(F5C.Text))
                            {
                                if (!string.IsNullOrEmpty(F6C.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool FillStatusOfFridayDownLessons()
        {
            if (!string.IsNullOrEmpty(F1Sn.Text))
            {
                if (!string.IsNullOrEmpty(F2Sn.Text))
                {
                    if (!string.IsNullOrEmpty(F3Sn.Text))
                    {
                        if (!string.IsNullOrEmpty(F4Sn.Text))
                        {
                            if (!string.IsNullOrEmpty(F5Sn.Text))
                            {
                                if (!string.IsNullOrEmpty(F6Sn.Text))
                                {
                                    return true; //Таким чином краще зрозуміти що тут коїться, як на мене
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void WriteInfoInFile(AllInfo allinfo)
        {
            string jsonString = JsonConvert.SerializeObject(allinfo);

            using (StreamWriter write = new StreamWriter("Information.drs", false))
            {
                write.WriteLineAsync(jsonString);
            }
        }

        public bool ReadInfoFromFile(out AllInfo obj)
        {
            obj = null;
            string filePath = "Information.drs";

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                obj = JsonConvert.DeserializeObject<AllInfo>(jsonString);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void EndOfRegister_Click(object sender, RoutedEventArgs e)
        {
            AllInfo TemplateOfData = new AllInfo();

            FormPanel.NextStage();

            List<LessonPair> TemplateOfPairLessonForMonday = new List<LessonPair>();
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M1C.Text), new Lesson(M1Sn.Text), new TimeSpan(9,0,0), new TimeSpan(10,20,0)));
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M2C.Text), new Lesson(M2Sn.Text), new TimeSpan(10,30,0), new TimeSpan(11,50,0)));
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M3C.Text), new Lesson(M3Sn.Text), new TimeSpan(12,20,0), new TimeSpan(13,40,0)));
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M4C.Text), new Lesson(M4Sn.Text), new TimeSpan(13,50,0), new TimeSpan(15,10,0)));
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M5C.Text), new Lesson(M5Sn.Text), new TimeSpan(15,20,0), new TimeSpan(16,40,0)));
            TemplateOfPairLessonForMonday.Add(new LessonPair(new Lesson(M6C.Text), new Lesson(M6Sn.Text), new TimeSpan(16,50,0), new TimeSpan(18,10,0)));

            TemplateOfData.SetList(1, TemplateOfPairLessonForMonday);

            List<LessonPair> TemplateOfPairLessonForThuesday = new List<LessonPair>();
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU1C.Text), new Lesson(TU1Sn.Text), new TimeSpan(9, 0, 0), new TimeSpan(10, 20, 0)));
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU2C.Text), new Lesson(TU2Sn.Text), new TimeSpan(10, 30, 0), new TimeSpan(11, 50, 0)));
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU3C.Text), new Lesson(TU3Sn.Text), new TimeSpan(12, 20, 0), new TimeSpan(13, 40, 0)));
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU4C.Text), new Lesson(TU4Sn.Text), new TimeSpan(13, 50, 0), new TimeSpan(15, 10, 0)));
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU5C.Text), new Lesson(TU5Sn.Text), new TimeSpan(15, 20, 0), new TimeSpan(16, 40, 0)));
            TemplateOfPairLessonForThuesday.Add(new LessonPair(new Lesson(TU6C.Text), new Lesson(TU6Sn.Text), new TimeSpan(16, 50, 0), new TimeSpan(18, 10, 0)));

            TemplateOfData.SetList(2, TemplateOfPairLessonForThuesday);

            List<LessonPair> TemplateOfPairLessonForWednesday = new List<LessonPair>();
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W1C.Text), new Lesson(W1Sn.Text), new TimeSpan(9, 0, 0), new TimeSpan(10, 20, 0)));
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W2C.Text), new Lesson(W2Sn.Text), new TimeSpan(10, 30, 0), new TimeSpan(11, 50, 0)));
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W3C.Text), new Lesson(W3Sn.Text), new TimeSpan(12, 20, 0), new TimeSpan(13, 40, 0)));
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W4C.Text), new Lesson(W4Sn.Text), new TimeSpan(13, 50, 0), new TimeSpan(15, 10, 0)));
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W5C.Text), new Lesson(W5Sn.Text), new TimeSpan(15, 20, 0), new TimeSpan(16, 40, 0)));
            TemplateOfPairLessonForWednesday.Add(new LessonPair(new Lesson(W6C.Text), new Lesson(W6Sn.Text), new TimeSpan(16, 50, 0), new TimeSpan(18, 10, 0)));

            TemplateOfData.SetList(3, TemplateOfPairLessonForWednesday);

            List<LessonPair> TemplateOfPairLessonForThursday = new List<LessonPair>();
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH1C.Text), new Lesson(TH1Sn.Text), new TimeSpan(9, 0, 0), new TimeSpan(10, 20, 0)));
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH2C.Text), new Lesson(TH2Sn.Text), new TimeSpan(10, 30, 0), new TimeSpan(11, 50, 0)));
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH3C.Text), new Lesson(TH3Sn.Text), new TimeSpan(12, 20, 0), new TimeSpan(13, 40, 0)));
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH4C.Text), new Lesson(TH4Sn.Text), new TimeSpan(13, 50, 0), new TimeSpan(15, 10, 0)));
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH5C.Text), new Lesson(TH5Sn.Text), new TimeSpan(15, 20, 0), new TimeSpan(16, 40, 0)));
            TemplateOfPairLessonForThursday.Add(new LessonPair(new Lesson(TH6C.Text), new Lesson(TH6Sn.Text), new TimeSpan(16, 50, 0), new TimeSpan(18, 10, 0)));

            TemplateOfData.SetList(4, TemplateOfPairLessonForThursday);

            List<LessonPair> TemplateOfPairLessonForFriday = new List<LessonPair>();
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F1C.Text), new Lesson(F1Sn.Text), new TimeSpan(9, 0, 0), new TimeSpan(10, 20, 0)));
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F2C.Text), new Lesson(F2Sn.Text), new TimeSpan(10, 30, 0), new TimeSpan(11, 50, 0)));
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F3C.Text), new Lesson(F3Sn.Text), new TimeSpan(12, 20, 0), new TimeSpan(13, 40, 0)));
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F4C.Text), new Lesson(F4Sn.Text), new TimeSpan(13, 50, 0), new TimeSpan(15, 10, 0)));
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F5C.Text), new Lesson(F5Sn.Text), new TimeSpan(15, 20, 0), new TimeSpan(16, 40, 0)));
            TemplateOfPairLessonForFriday.Add(new LessonPair(new Lesson(F6C.Text), new Lesson(F6Sn.Text), new TimeSpan(16, 50, 0), new TimeSpan(18, 10, 0)));

            TemplateOfData.SetList(5, TemplateOfPairLessonForFriday);

            SettingsProperty TemplateOfSettings = new SettingsProperty();
            TemplateOfSettings.MessageAboutLessonStart = StartLesson.IsChecked;
            TemplateOfSettings.MessageAboutLessonEnd = EndLesson.IsChecked;
            TemplateOfSettings.TimeBeginEndOfLesson = EndLesson.IsChecked;
            TemplateOfSettings.NextLesson = TimeForBeginLesson.IsChecked;
            TemplateOfSettings.StartWithSystem = RunWithSystem.IsChecked;

            TemplateOfData.Properties.SetAllProperties(TemplateOfSettings);//Тут было дело поменяял способ сохранения

            WriteInfoInFile(TemplateOfData);

            ReadInfoFromFile(out InformationForAllProgram);

            ManageLessons.StartManage(InformationForAllProgram, this);

            Registration.Visibility = Visibility.Hidden;
        }

        private void AllLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            WatchAllLessons watchAllLessons = new WatchAllLessons(ManageLessons.CurrentNumberOfDay, InformationForAllProgram, this);
            watchAllLessons.Owner = this;
            watchAllLessons.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            watchAllLessons.ShowDialog();
        }

        private void ShowSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow ShowAllSettings = new SettingsWindow(ref InformationForAllProgram, this);
            ShowAllSettings.Owner = this;
            ShowAllSettings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowAllSettings.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mutex?.Close();
            if (Registration.Visibility != Visibility.Visible)
            {
                WriteInfoInFile(InformationForAllProgram);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PushUpMessage pushUpMessage = new PushUpMessage(this);
            pushUpMessagesList.Add(pushUpMessage);
            pushUpMessage.Show();
            pushUpMessage.ShowTheMessage(PushUpMessage.EndOfLesson, "AAAAAAAAAA", "BBBBBBBBBBBBB");
        }

        public void DestroyPushUp(PushUpMessage pushUpMessage)
        {
            if (pushUpMessagesList.Contains(pushUpMessage))
            {
                pushUpMessage.Close();
                pushUpMessagesList.Remove(pushUpMessage);
            }
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowApp_Click(object sender, RoutedEventArgs e)
        {
            Show();
            Focus();
        }

        void EndRegistrationAnimation()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1.0;
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTarget(opacityAnimation, Registration);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            storyboard.Completed += new EventHandler(Storyboard_Completed);

            storyboard.Begin();


        }

        private void ChooseTheFile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string AnyFilePath;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Відкрити файл з даними програми";
            openFileDialog.Filter = "Файли з даними програми (*.drs)|*.drs";
            bool? openResult = openFileDialog.ShowDialog();

            if (openResult == true)
            {
                AnyFilePath = openFileDialog.FileName;
                if (ReadInfoFromFile(out InformationForAllProgram, AnyFilePath))
                {
                    ManageLessons.StartManage(InformationForAllProgram, this);
                    EndRegistrationAnimation();
                }
                else
                {
                    MessageBox.Show("Неможливо прочитати дані файлу, тому що вони некоректні або пошкоджені");
                    return;
                }
            }
        }

        public bool ReadInfoFromFile(out AllInfo obj, string filePath)
        {
            obj = null;

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                obj = JsonConvert.DeserializeObject<AllInfo>(jsonString);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Registration.Visibility = Visibility.Hidden;
        }

        private void DropRectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (file.Substring(file.Length - 4) == ".drs")
                    {
                        if (ReadInfoFromFile(out InformationForAllProgram, file))
                        {
                            EndRegistrationAnimation();
                            ManageLessons.StartManage(InformationForAllProgram, this);
                        }
                        else
                        {
                            DropRectangle_DragLeave(sender, e);
                            MessageBox.Show("Неможливо прочитати дані файлу, тому що вони некоректні або пошкоджені");
                            return;
                        }

                    }
                    else
                    {
                        DropRectangle_DragLeave(sender, e);
                        MessageBox.Show("Неможливо прочитати дані файлу, тому що він неправильного розширення");
                        return;
                    }
                    break;
                }
            }
        }

        private void DropRectangle_DragEnter(object sender, DragEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1.0;
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.19));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTarget(opacityAnimation, DropStackPanel);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            storyboard.Begin();

            DropRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#041723"));

            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = ((SolidColorBrush)DropRectangle.Fill).Color,
                To = (Color)ColorConverter.ConvertFromString("#30C4C4C4"),
                Duration = TimeSpan.FromSeconds(0.19)
            };

            DropRectangle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void DropRectangle_DragLeave(object sender, DragEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0.0;
            opacityAnimation.To = 1.0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.19));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTarget(opacityAnimation, DropStackPanel);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            storyboard.Begin();

            DropRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30C4C4C4"));

            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = ((SolidColorBrush)DropRectangle.Fill).Color,
                To = (Color)ColorConverter.ConvertFromString("#041723"),
                Duration = TimeSpan.FromSeconds(0.19)
            };

            DropRectangle.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }
    }
}