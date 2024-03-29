﻿using oop_lab4_1.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace oop_lab4_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowView
    {
        public MainWindowViewModel ViewModel { get; set; }
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
            defaultCircleColor = getRecourceAsColor("Foreground");
            selectedCircleColor = getRecourceAsColor("Accent");

            InitializeComponent();
        }

        private void canvasImage_Loaded(object sender, RoutedEventArgs e)
        {
            DrawShapes();
        }

        private void canvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.DeselectAll();
                ViewModel.AddCircle(
                    (int)e.GetPosition(canvas).X, 
                    (int)e.GetPosition(canvas).Y);
                DrawShapes();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if(Keyboard.IsKeyDown(Key.LeftCtrl) == false)
                {
                    ViewModel.DeselectAll();
                }
                ViewModel.SelectCircleAt(
                    (int)e.GetPosition(canvas).X,
                    (int)e.GetPosition(canvas).Y);
                DrawShapes();
            }
        }
        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ViewModel.DeleteSelected();
                DrawShapes();
            }
        }

        System.Drawing.Color defaultCircleColor;
        System.Drawing.Color selectedCircleColor;
        void DrawShapes()
        {
            using (var bmp = new Bitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight))
            using (var gfx = Graphics.FromImage(bmp))
            using (var defaultPen = new System.Drawing.Pen(defaultCircleColor, 5))
            using (var selectedPen = new System.Drawing.Pen(selectedCircleColor, 5))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.Transparent);
                for(ViewModel.ShapeContainer.First(); 
                    ViewModel.ShapeContainer.IsEOL() == false; 
                    ViewModel.ShapeContainer.Next())
                {
                    ViewModel.ShapeContainer.GetCurrent().DrawItself(gfx, defaultPen, selectedPen);
                }
                canvasImage.Source = BmpImageFromBmp(bmp);
            }
        }
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        System.Drawing.Color getRecourceAsColor(string name)
        {
            SolidColorBrush resourceColor = FindResource(name) as SolidColorBrush;
            return System.Drawing.Color.FromArgb(
                resourceColor.Color.A,
                resourceColor.Color.R,
                resourceColor.Color.G,
                resourceColor.Color.B);
        }
    }
}

//In Windows Forms/GDI, the graphics API is an immediate mode graphics API. Each time the window is refreshed/invalidated,
//you explicitly draw the contents using Graphics.

//In WPF, however, things work differently.You rarely ever directly draw - instead, it's a retained mode graphics API.
//You tell WPF where you want the objects, and it takes care of the drawing for you.

//The best way to think of it is, in Windows Forms, you'd say "Draw a line". And you repeat this every time you need
//to "redraw" since the screen is invalidated.

//In WPF, instead, you say "I want a line from X1 to Y1. I want a line from X2 to Y2." WPF then decides when
//and how to draw it for you.

//This is done by placing the shapes on a Canvas, and then letting WPF do all of the hard work.