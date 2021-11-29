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
            defaultCircleColor = getRecourceAsColor("GreyForeground");
            selectedCircleColor = getRecourceAsColor("Accent");

            InitializeComponent();
        }

        private void canvasImage_Loaded(object sender, RoutedEventArgs e)
        {
            DrawShapes();
        }

        private void canvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine($"{(int)e.GetPosition(canvas).X} {(int)e.GetPosition(canvas).Y}");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.AddCircle(
                    (int)e.GetPosition(canvas).X, 
                    (int)e.GetPosition(canvas).Y);
                DrawShapes();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if(ViewModel.SelectCircleAt(
                    (int)e.GetPosition(canvas).X,
                    (int)e.GetPosition(canvas).Y))
                {
                    DrawShapes();
                }
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
