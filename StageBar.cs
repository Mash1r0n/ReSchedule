using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;


namespace ReSchedule
{
    interface InitStage
    {
        string Types { get; set; }
        Rectangle Figure { get; set; }

        Canvas Space { get; set; }

        TextBlock Text_Block { get; set; }

        void SetActive();

        void SetDeactive();

        void BringToCompleted();

        void BringToUncompleted();

        int GetStageRadius();
    }
    struct PreStage : InitStage
    {
        const int FixedWidth = 80;
        const int FixedHeight = 2;
        public string Types { get; set; }
        public Rectangle Figure { get; set; }

        public Canvas Space { get; set; }

        public TextBlock Text_Block { get; set; }

        public int GetStageRadius() { return FixedWidth; }
        public PreStage(string types, string PreStageName)
        {
            Types = types;

            Figure = new Rectangle
            {
                Width = FixedWidth,
                Height = FixedHeight,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350")),
                Margin = new Thickness(0, 1, 10, 0),
                Name = PreStageName
            };

            Space = new Canvas
            {
                Width = FixedWidth,
                Height = FixedHeight,
                Margin = new Thickness(0, 0, 0, 0),
            };

            Text_Block = new TextBlock();

            Space.Children.Add(Figure);

        }

        public void SetActive()
        {

        }

        public void BringToCompleted()
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 0);

            Color ToColor = (Color)ColorConverter.ConvertFromString("#CED6E3");
            Color FromColor = (Color)ColorConverter.ConvertFromString("#233350");
            gradientBrush.GradientStops.Add(new GradientStop(ToColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(ToColor, 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(FromColor, 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(FromColor, 1.0));

            Figure.Fill = gradientBrush;

            DoubleAnimation gradientStopAnimation = new DoubleAnimation();
            gradientStopAnimation.From = 0.0;
            gradientStopAnimation.To = 1.0;
            gradientStopAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            gradientBrush.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, gradientStopAnimation);
            gradientBrush.GradientStops[2].BeginAnimation(GradientStop.OffsetProperty, gradientStopAnimation);
        }

        public void SetDeactive()
        {

        }

        public void BringToUncompleted()
        {
            // Создаем LinearGradientBrush
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 0);

            Color ToColor = (Color)ColorConverter.ConvertFromString("#CED6E3");
            Color FromColor = (Color)ColorConverter.ConvertFromString("#233350");
            // Добавляем градиентные стопы
            gradientBrush.GradientStops.Add(new GradientStop(ToColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(ToColor, 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(FromColor, 0.5));
            gradientBrush.GradientStops.Add(new GradientStop(FromColor, 1.0));

            // Устанавливаем кисть для Rectangle
            Figure.Fill = gradientBrush;

            // Создаем анимацию для второго и третьего GradientStop
            DoubleAnimation gradientStopAnimation = new DoubleAnimation();
            gradientStopAnimation.From = 1.0;
            gradientStopAnimation.To = 0.0;
            gradientStopAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Применяем анимацию
            gradientBrush.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, gradientStopAnimation);
            gradientBrush.GradientStops[2].BeginAnimation(GradientStop.OffsetProperty, gradientStopAnimation);
        }
    }
    struct Stage : InitStage
    {
        const int StageRadius = 15;
        public string Types { get; set; }
        public Rectangle Figure { get; set; }

        public Canvas Space { get; set; }

        public TextBlock Text_Block { get; set; }

        public int GetStageRadius()
        {
            return StageRadius;
        }

        public Stage(string types, string StageName)
        {
            Types = types;
            Figure = new Rectangle
            {
                Width = StageRadius * 2,
                Height = StageRadius * 2,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350")),
                //Margin = new Thickness(0, 0, 10, 0),
                RadiusX = 50,
                RadiusY = 50,
                Name = StageName,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350")),
                StrokeThickness = 1.9
            };

            Text_Block = new TextBlock
            {
                FontFamily = new System.Windows.Media.FontFamily("Roboto"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350")),
                FontWeight = FontWeights.Bold,
                FontSize = 11,
            };

            Space = new Canvas
            {
                Height = StageRadius * 2,
                Width = StageRadius * 2,
                Margin = new Thickness(0, 0, 0, 0),
            };

            Space.Children.Add(Figure);
            Space.Children.Add(Text_Block);

        }

        public void SetActive()
        {
            Figure.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CED6E3"));
            Text_Block.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8D9EA5"));
            Figure.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#041723"));
        }
        public void BringToCompleted()
        {
            DoubleAnimation FillCircle = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.19),
                From = 1.9,
                To = 15
            };

            //ColorAnimation CompleteColor = new ColorAnimation
            //{
            //    Duration = TimeSpan.FromSeconds(0.3),
            //    From = ((SolidColorBrush)Text_Block.Foreground).Color,
            //    To = Colors.Red,
            //};

            Storyboard MakeCompleted = new Storyboard();

            MakeCompleted.Children.Add(FillCircle);
            Storyboard.SetTarget(FillCircle, Figure);
            Storyboard.SetTargetProperty(FillCircle, new PropertyPath("StrokeThickness"));

            ObjectAnimationUsingKeyFrames colorAnimation = new ObjectAnimationUsingKeyFrames();
            colorAnimation.Duration = TimeSpan.FromSeconds(0.27);
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#8D9EA5"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#93A4AB"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.03))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#9AA9B1"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.06))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#A1AFB8"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.09))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#A7B4BE"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.12))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#ADBAC4"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#ADBAC4"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.18))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#BBC5D0"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.21))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#C1CBD7"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.24))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#C7D0DD"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.27))));
            colorAnimation.FillBehavior = FillBehavior.Stop;

            TextBlock localTextBlock = Text_Block;
            colorAnimation.Completed += new EventHandler(ColorAnimation_Completed);

            void ColorAnimation_Completed(object sender, EventArgs e)
            {
                localTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7D0DD"));
            }

            MakeCompleted.Children.Add(colorAnimation);
            Storyboard.SetTarget(colorAnimation, Text_Block);
            Storyboard.SetTargetName(colorAnimation, "Text_Block");
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(TextBlock.ForegroundProperty));

            MakeCompleted.Begin();
        }

        public void SetDeactive()
        {
            Figure.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350"));
            Text_Block.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350"));
            Figure.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#233350"));
        }

        public void BringToUncompleted()
        {
            DoubleAnimation FillCircle = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.19),
                From = 15,
                To = 1.9
            };

            //ColorAnimation CompleteColor = new ColorAnimation
            //{
            //    Duration = TimeSpan.FromSeconds(0.3),
            //    From = ((SolidColorBrush)Text_Block.Foreground).Color,
            //    To = Colors.Red,
            //};

            Storyboard MakeCompleted = new Storyboard();

            MakeCompleted.Children.Add(FillCircle);
            Storyboard.SetTarget(FillCircle, Figure);
            Storyboard.SetTargetProperty(FillCircle, new PropertyPath("StrokeThickness"));

            ObjectAnimationUsingKeyFrames colorAnimation = new ObjectAnimationUsingKeyFrames();
            colorAnimation.Duration = TimeSpan.FromSeconds(0.27);
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#C7D0DD"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#C1CBD7"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.03))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#BBC5D0"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.06))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#ADBAC4"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.09))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#ADBAC4"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.12))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#A7B4BE"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#A1AFB8"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.18))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#9AA9B1"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.21))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#93A4AB"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.24))));
            colorAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame((SolidColorBrush)new BrushConverter().ConvertFromString("#8D9EA5"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.27))));
            colorAnimation.FillBehavior = FillBehavior.Stop;

            TextBlock localTextBlock = Text_Block;
            colorAnimation.Completed += new EventHandler(ColorAnimation_Completed);

            void ColorAnimation_Completed(object sender, EventArgs e)
            {
                localTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8D9EA5"));
            }

            MakeCompleted.Children.Add(colorAnimation);
            Storyboard.SetTarget(colorAnimation, Text_Block);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(TextBlock.ForegroundProperty));

            MakeCompleted.Begin();
        }
    }

    class StageBar
    {
        const string PreStageType = "PST";
        const string StageType = "PT";

        string Name;

        List<InitStage> StagesList = new List<InitStage>();

        public StageBar(string BlockName)
        {
            Name = BlockName;
            GenerateStage();
        }

        void GenerateStage()
        {
            if (FormPanel.GetCount() != 0)
            {
                StagesList.Add(new PreStage(PreStageType, $"PreStage{FormPanel.GetPreCount()}"));
            }
            StagesList.Add(new Stage(StageType, $"Stage{FormPanel.GetCount()}"));
        }

        public string GetName() { return Name; }
        public List<InitStage> GetStageList() { return StagesList; }

        public InitStage ReturnPT()
        {
            foreach (InitStage stage in StagesList)
            {
                if (stage.Types == "PT")
                {
                    return stage;
                }
            }
            throw new Exception();
        }
    }

    static class FormPanel
    {
        static List<StageBar> StagesInBlock = new List<StageBar>();

        static int CurrentStage = 0;

        static int PreCount = 0;
        static int Count = 0;

        public static int ReturnCountOfStages() 
        {
            return StagesInBlock.Count();
        }

        public static void CreateBlock(string TheBlockName)
        {
            StageBar StageObject = new StageBar(TheBlockName);
            StagesInBlock.Add(StageObject);
            if (Count != 0)
            {
                PreCount++;
            }
            else if (Count == 0)
            {
                StageObject.GetStageList()[0].SetActive();
            }
            Count++;
        }

        public static void NextStage() //
        {
            if (StagesInBlock.Count <= 1)
            {
                return;
            }

            if (CurrentStage + 1 < Count)
            {
                StagesInBlock[CurrentStage].GetStageList()[CurrentStage == 0 ? 0 : 1].BringToCompleted();
                CurrentStage++;
                StagesInBlock[CurrentStage].GetStageList()[0].BringToCompleted();
                StagesInBlock[CurrentStage].GetStageList()[1].SetActive();
            }
            else if (CurrentStage + 1 == Count)
            {
                StagesInBlock[CurrentStage].GetStageList()[1].BringToCompleted();
                CurrentStage++;
            }
        }

        public static void PreviousStage() //
        {
            if (StagesInBlock.Count <= 1)
            {
                return;
            }
            if (CurrentStage == Count)
            {
                StagesInBlock[CurrentStage - 1].GetStageList()[1].BringToUncompleted();
                CurrentStage--;
            }
            else if (CurrentStage > 0)
            {
                StagesInBlock[CurrentStage].GetStageList()[0].BringToUncompleted();
                StagesInBlock[CurrentStage].GetStageList()[1].SetDeactive();
                CurrentStage--;
                StagesInBlock[CurrentStage].GetStageList()[CurrentStage == 0 ? 0 : 1].BringToUncompleted();
            }
            else if (CurrentStage - 1 == 0)
            {
                StagesInBlock[CurrentStage].GetStageList()[0].BringToUncompleted();
                CurrentStage--;
            }
        }

        public static int GetCount() { return Count; }
        public static int GetPreCount() { return PreCount; }

        public static StackPanel ReturnCompletedPanel()
        {

            StackPanel mid = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#041723")),
                Margin = new Thickness(0, 10, 0, 0),
                
                Name = "MidPan"
            };

            int ForeachCount = 0;

            Grid.SetColumn(mid, 1);

            foreach (StageBar stage in StagesInBlock)
            {


                List<InitStage> t = stage.GetStageList();
                mid.Children.Add(t[0].Space);
                try
                {
                    mid.Children.Add(t[1].Space);
                    mid.Children.Add(t[2].Space);
                }
                catch (Exception)
                {

                }

                InitStage tmp = stage.ReturnPT();

                stage.ReturnPT().Text_Block.Loaded += (s, e) =>
                {

                    ForeachCount++;
                    double left = (tmp.Space.Width - tmp.Text_Block.ActualWidth) / 2;
                    double top = (tmp.Space.Height - tmp.Text_Block.ActualHeight) / 2;
                    Canvas.SetLeft(tmp.Text_Block, left);
                    Canvas.SetTop(tmp.Text_Block, ForeachCount % 2 == 0 ? top + tmp.GetStageRadius() + 11 : top - tmp.GetStageRadius() - 9);

                };

                if (ForeachCount % 2 == 0)
                {
                    tmp.Text_Block.Text = stage.GetName();

                }
                else
                {
                    tmp.Text_Block.Text = stage.GetName();

                }

                ForeachCount++;
            }
            ForeachCount = 0;
            //Идея для реализации подписи текста: Я создам грид, в котором будет три стак панели: горизонтальная (тест сверху), горизонтальная (уже есть), горизонтальная (текст снизу)
            return mid;
        }
    }
}







namespace PagesTest
{
    

}
