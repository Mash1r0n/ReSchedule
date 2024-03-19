using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Threading;
using Newtonsoft;
using Newtonsoft.Json;

namespace ReSchedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    struct Lesson
    {
        [JsonProperty]
        public string? lesson { get; private set; }
        public Lesson(string? lesson)
        {
            this.lesson = lesson;
        }
    }

    struct LessonPair
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
    class AllLessons()
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

        public List<LessonPair> GetDaysLessons(int NumberOfDay)
        {
            switch (NumberOfDay)
            {
                case 1: return Monday; 
                case 2: return Thuesday;
                case 3: return Wednesday;
                case 4: return Thursday;
                case 5: return Friday;
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

    struct SettingsProperty
    {
        //Messages
        public bool? MessageAboutLessonStart;
        public bool? MessageAboutLessonEnd;

        //Context menu
        public bool? TimeBeforeEndOfCurrentLesson;
        public bool? TimeBeforeBeginNextLesson;

        //ETC
        public bool? StartWithSystem;

        public SettingsProperty()
        {
            MessageAboutLessonStart = false;
            MessageAboutLessonEnd = false;
            TimeBeforeEndOfCurrentLesson = false;
            TimeBeforeBeginNextLesson = false;
            StartWithSystem = false;
        }

        public void SetAllProperties(SettingsProperty properties)
        {
            MessageAboutLessonStart = properties.MessageAboutLessonStart;
            MessageAboutLessonEnd = properties.MessageAboutLessonEnd;
            TimeBeforeEndOfCurrentLesson = properties.TimeBeforeEndOfCurrentLesson;
            TimeBeforeBeginNextLesson = properties.TimeBeforeBeginNextLesson;
            StartWithSystem = properties.StartWithSystem;
        }
    }

    class AllInfo : AllLessons
    {
        [JsonProperty]
        public SettingsProperty Properties { get; private set; } = new SettingsProperty();

        public void SetSettings(SettingsProperty SProperty)
        {
            Properties = SProperty;
        }
    }

    struct LessonInfo
    {
        public Lesson? NameOfLesson { get; private set; }
        public TimeSpan LessonBegin { get; private set; }
        public TimeSpan LessonEnd { get; private set; }

        public LessonInfo(Lesson? name, TimeSpan begin, TimeSpan end) {
            NameOfLesson = name;
            LessonBegin = begin;
            LessonEnd = end;
        }
    }

    static class ManageLessons
    {
        static List<LessonInfo> InformationAboutLessons = new List<LessonInfo>();

        static public void StartManage(string NameOfCurrentDay, int NumberOfCurrentDay, AllInfo AllCurrentInfo)
        {
            List<LessonPair> lesson = AllCurrentInfo.GetDaysLessons(NumberOfCurrentDay);

            for (int i = 0; i < 6; i++)
            {
                InformationAboutLessons.Add(new LessonInfo(CheckForNull((GetWeekMode() ? lesson[i].Lessons2.lesson : lesson[i].Lessons1.lesson)), lesson[i].LessonBegin, lesson[i].LessonEnd));
            }
        }

        static Lesson? CheckForNull(string? str)
        {
            if (str == "-" || str == null)
            {
                return null;
            }
            return new Lesson(str);
        }
        static bool GetWeekMode() //true - Знаменник, false - Чисельник
        {
            DateTime StartDate = new DateTime(2023, 1, 1);
            TimeSpan diff = DateTime.Now - StartDate;
            int diffWeeks = (int)diff.TotalDays / 7;
            return diffWeeks % 2 != 0;
        }
    }

    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point anchorPoint;

        AllInfo InformationForAllProgram = new AllInfo();

        static int CurrentNumberOfDay = ((int)DateTime.Now.DayOfWeek + 6) % 7 + 1;

        static string GetDaysNameByNumber(int DaysNumber)
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

        string currentDay = GetDaysNameByNumber(CurrentNumberOfDay);

        public MainWindow()
        {
            InitializeComponent();

            const int MinutesForRegistration = 7;
            const string TextAfterMinutesStages = "хвилин";
            const string TextForStages = "етапів";

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
                ManageLessons.StartManage(currentDay, CurrentNumberOfDay, InformationForAllProgram);
            }

            Registration.ScrollToHorizontalOffset(1028);

            FormPanel.CreateBlock("Початок");
            FormPanel.CreateBlock("Заповнення");
            FormPanel.CreateBlock("Налаштування");
            FormPanel.CreateBlock("Кінець");

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
            AuthorWindow authorWindow = new AuthorWindow();
            authorWindow.Owner = this;
            authorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            authorWindow.ShowDialog();
        }

        int TickTime = 0;

        //За інформацією про поточні сторінки я зможну гночку керувати кнопками скроллингу сторінки
        int CurrentRegistrationPage = 0;
        DispatcherTimer timer;

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
        DispatcherTimer FillTImer = new DispatcherTimer();
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

        int TickDropTime = 0;

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

        void WriteInfoInFile(AllInfo allinfo)
        {
            string jsonString = JsonConvert.SerializeObject(allinfo);

            using (StreamWriter write = new StreamWriter("Information.drs", false))
            {
                write.WriteLineAsync(jsonString);
            }
        }

        bool ReadInfoFromFile(out AllInfo obj)
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

        //void ApplySettings(AllInfo allinfo)
        //{
        //    List<LessonPair> TemplateOfCurrentLessons = allinfo.GetDaysLessons(CurrentNumberOfDay);

        //    for (int i = 0; i < TemplateOfCurrentLessons.Count; i++)
        //    {

        //    }
        //}
        private void EndOfRegister_Click(object sender, RoutedEventArgs e)
        {
            AllInfo TemplateOfData = new AllInfo();

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
            TemplateOfSettings.TimeBeforeEndOfCurrentLesson = EndLesson.IsChecked;
            TemplateOfSettings.TimeBeforeBeginNextLesson = TimeForBeginLesson.IsChecked;
            TemplateOfSettings.StartWithSystem = RunWithSystem.IsChecked;

            TemplateOfData.SetSettings(TemplateOfSettings);

            WriteInfoInFile(TemplateOfData);

            ReadInfoFromFile(out InformationForAllProgram);

            ManageLessons.StartManage(currentDay, CurrentNumberOfDay, InformationForAllProgram);
        }
    }
}