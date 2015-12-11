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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MockupPerfect.Styles;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Drawing.Imaging;

namespace MockupPerfect
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            System.Drawing.Bitmap placeholder = MockupPerfect.Properties.Resources.placeholder;
            if (placeholder != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    placeholder.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    rotation = 0;
                    scale = .5;
                    originalImage = bitmapImage;
                    updateImage();
                }
            } 
        }

        #region grag and drop
        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                image.OpacityMask = new SolidColorBrush(Color.FromRgb(155, 155, 155));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            image.OpacityMask = new SolidColorBrush(Color.FromRgb(155, 155, 155));
        }

        private void DropBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Count() > 0)
                {
                    string fileName = files[0];
                    rotation = 0;
                    scale = .5;
                    originalImage = new BitmapImage(new Uri(fileName));
                    updateImage();
                }
            }

            image.OpacityMask = new SolidColorBrush(Color.FromRgb(155, 155, 155));
        }
        #endregion

        private BitmapImage originalImage;
        private BitmapSource bitmapImage;
        private double rotation = 0;
        private double scale = 1;

        private bool updatingImageInProgress = false;
        private void updateImage()
        {
            updatingImageInProgress = true;

            TransformedBitmap TempImage = new TransformedBitmap();

            TempImage.BeginInit();
            TempImage.Source = originalImage;

            RotateTransform rotateTransform = new RotateTransform(rotation);
            ScaleTransform scaleTransform = new ScaleTransform(scale, scale, .5, .5);

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(scaleTransform);

            TempImage.Transform = transformGroup;
            TempImage.EndInit();

            setImageSource(TempImage);

            updatingImageInProgress = false;
        }

        private void setImageSource(BitmapSource bitmap)
        {
            bitmapImage = bitmap;
            image.Source = bitmapImage;
            
            Width = bitmapImage.PixelWidth + getHorisontalMargins();
            Height = bitmapImage.PixelHeight + getVerticalMargins();
        }

        private double getVerticalMargins()
        {
            if (IsLoaded)
            {
                Point thisPosition = this.PointToScreen(new Point(0d, 0d));
                Point thisPosition1 = this.PointToScreen(new Point(0d, ActualHeight));
                Point imagePosition = image.PointToScreen(new Point(0d, 0d));
                Point imagePosition1 = image.PointToScreen(new Point(0d, grid.ActualHeight));

                return imagePosition.Y - thisPosition.Y + thisPosition1.Y - imagePosition1.Y;
            }
            return 48;
        }

        private double getHorisontalMargins()
        {
            if (IsLoaded)
            {
                Point thisPosition = this.PointToScreen(new Point(0d, 0d));
                Point thisPosition1 = this.PointToScreen(new Point(ActualWidth, 0d));
                Point imagePosition = image.PointToScreen(new Point(0d, 0d));
                Point imagePosition1 = image.PointToScreen(new Point(image.ActualWidth, 0d));

                return imagePosition.X - thisPosition.X + thisPosition1.X - imagePosition1.X;
            }
            return 16;
        }

        #region hot keys
        List<Key> _pressedKeys = new List<Key>();

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (originalImage == null)
                return;

            if (_pressedKeys.Count() == 1)
            {
                if (_pressedKeys.Contains(Key.Left))
                {
                    Left -= 1;
                }
                else if (_pressedKeys.Contains(Key.Right))
                {
                    Left += 1;
                }
                else if (_pressedKeys.Contains(Key.Up))
                {
                    Top -= 1;
                }
                else if (_pressedKeys.Contains(Key.Down))
                {
                    Top += 1;
                } 
            }
            if (_pressedKeys.Count() >= 2)
            {
                if (_pressedKeys.Contains(Key.LeftShift) || _pressedKeys.Contains(Key.RightShift))
                {
                    if (_pressedKeys.Contains(Key.D1))
                    {
                        scale = .25;
                        updateImage();
                    }
                    else if (_pressedKeys.Contains(Key.D2))
                    {
                        scale = .33;
                        updateImage();
                    }
                    else if (_pressedKeys.Contains(Key.D3))
                    {
                        scale = .5;
                        updateImage();
                    }
                    else if (_pressedKeys.Contains(Key.D4))
                    {
                        scale = .75;
                        updateImage();
                    }
                    else if (_pressedKeys.Contains(Key.D5))
                    {
                        scale = 1;
                        updateImage();
                    }
                    else if (_pressedKeys.Contains(Key.Left))
                    {
                        Left -= 10;
                    }
                    else if (_pressedKeys.Contains(Key.Right))
                    {
                        Left += 10;
                    }
                    else if (_pressedKeys.Contains(Key.Up))
                    {
                        Top -= 10;
                    }
                    else if (_pressedKeys.Contains(Key.Down))
                    {
                        Top += 10;
                    } 
                }
                else if (_pressedKeys.Contains(Key.LeftCtrl) || _pressedKeys.Contains(Key.RightCtrl))
                {
                    if (_pressedKeys.Contains(Key.D1))
                    {
                        Opacity = 0.1;
                    } 
                    else if (_pressedKeys.Contains(Key.D2))
                    {
                        Opacity = 0.2;
                    }
                    else if (_pressedKeys.Contains(Key.D3))
                    {
                        Opacity = 0.3;
                    } 
                    else if (_pressedKeys.Contains(Key.D4))
                    {
                        Opacity = 0.4;
                    } 
                    else if (_pressedKeys.Contains(Key.D5))
                    {
                        Opacity = 0.5;
                    } 
                    else if (_pressedKeys.Contains(Key.D6))
                    {
                        Opacity = 0.6;
                    } 
                    else if (_pressedKeys.Contains(Key.D7))
                    {
                        Opacity = 0.7;
                    }
                    else if (_pressedKeys.Contains(Key.D8))
                    {
                        Opacity = 0.8;
                    }
                    else if (_pressedKeys.Contains(Key.D9))
                    {
                        Opacity = 0.9;
                    }
                    else if (_pressedKeys.Contains(Key.D0))
                    {
                        Opacity = 1;
                    } 
                    else if (_pressedKeys.Contains(Key.Left))
                    {
                        rotation += 90;
                        updateImage();
                    } 
                    else if (_pressedKeys.Contains(Key.Right))
                    {
                        rotation -= 90;
                        updateImage();
                    } 
                }
            }

            _pressedKeys.Remove(e.Key);
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.Key))
                return;

            _pressedKeys.Add(e.Key);
            e.Handled = true;
        }
        #endregion

        #region drag and resize window
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!updatingImageInProgress && bitmapImage != null)
            {
                double aspectRatio = bitmapImage.Width / bitmapImage.Height;
                double verticalMargins = getVerticalMargins();
                double horisontalMargins = getHorisontalMargins();

                if (sizeInfo.WidthChanged)
                    this.Width = (sizeInfo.NewSize.Height - verticalMargins) * aspectRatio + horisontalMargins;
                else
                    this.Height = (sizeInfo.NewSize.Width - horisontalMargins) / aspectRatio + verticalMargins;
            } 

            base.OnRenderSizeChanged(sizeInfo);
        }
        #endregion
    }
}
