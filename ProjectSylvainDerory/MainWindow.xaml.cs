using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ProjectSylvainDerory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point startingPoint;
        Point offset;
        Rectangle activeRectangle;
        Rectangle movingRectangle;
        String state_Cursor = "Add Rectangle";

        public MainWindow()
        {
            InitializeComponent();
        }

        // function used to check if the mouse is inside the image
        // we use the function TranslatePoint to get the point relative to the Canvas Container
        private Boolean Check_mouse_in_image(Point mouse)
        {
            if (imagePictures.Source != null)
            {
                Point imgPoint = imagePictures.TranslatePoint(new Point(0, 0), canvasImage);
                if (mouse.X >= imgPoint.X && mouse.X < imgPoint.X + imagePictures.ActualWidth && mouse.Y >= 0 && mouse.Y < imgPoint.Y + imagePictures.ActualHeight)
                {
                    return true;
                }
            }
            return false;
        }

        //Function used to convert the Color from the Rectangle
        //to the type of color used by Color Picker
        private System.Drawing.Color Convert_Color_to_Brush(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        //Function used to convert the Color from the ColorPicker
        //to the type of color used by the Rectangle
        private SolidColorBrush Convert_Brush_to_Color(System.Drawing.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Rectangle)
            {
                if(state_Cursor == "Change Rectangle Color")
                {
                    // if the user wants to modify the color
                    // when clicking on a Rectangle we display
                    // a Color Palette for him to choose a new one
                    // the color selected by default is the color of the Rectangle
                    Rectangle myRectangle = (e.OriginalSource as Rectangle);
                    Color rectColor = (myRectangle.Fill as SolidColorBrush).Color;
                    System.Drawing.Color dialogColor = Convert_Color_to_Brush(rectColor);
                    ColorDialog colorDialog = new ColorDialog
                    {
                        Color = dialogColor,
                    };
                    try
                    {
                        if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            myRectangle.Fill = Convert_Brush_to_Color(colorDialog.Color);
                        }
                    }
                    catch
                    {

                        System.Windows.MessageBox.Show("Error occured While Changing the color", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (state_Cursor == "Move Rectangle")
                {
                    // If the user selected the move Cursor
                    // He can drag and drop the Rectangle on the image
                    // the offset is necessary to know the difference between
                    // where the user clicked on the Rectangle and the position
                    // of the Rectangle.
                    movingRectangle = (e.OriginalSource as Rectangle);
                    // Get Offset to move the element
                    offset = e.GetPosition(canvasImage);
                    offset.X -= Canvas.GetLeft(movingRectangle);
                    offset.Y -= Canvas.GetTop(movingRectangle);
                }
                else if (state_Cursor == "Delete Rectangle")
                {
                    // If the user selected the delete Cursor
                    // when Clicking on a Rectangle, it will be deleted
                    canvasImage.Children.Remove(e.OriginalSource as Rectangle);
                }
            }
            else if (state_Cursor == "Add Rectangle")
            {
                // If the user selected the Add Cursor
                // when Clickingon the image a rectangle is created
                // the user can then drag the mouse to draw the rectangle
                startingPoint = e.GetPosition(canvasImage);
                if (Check_mouse_in_image(startingPoint))
                {
                    activeRectangle = new Rectangle
                    {
                        Fill = System.Windows.Media.Brushes.Teal,
                    };

                    Canvas.SetLeft(activeRectangle, startingPoint.X);
                    Canvas.SetTop(activeRectangle, startingPoint.Y);

                    canvasImage.Children.Add(activeRectangle);
                }
            }
        }

        private void CanvasImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // make sure that we are resizing a rectangle
            if (activeRectangle != null)
            {
                //get current point and test that we are still in the image
                Point currentPosition = e.GetPosition(canvasImage);
                if (Check_mouse_in_image(currentPosition))
                {
                    //calculate the width and height of the resizing rectangle
                    double width = Math.Abs(currentPosition.X - startingPoint.X);
                    double height = Math.Abs(currentPosition.Y - startingPoint.Y);

                    //apply the new dimention to the active rectangle
                    activeRectangle.Width = width;
                    activeRectangle.Height = height;

                    //set the position of the resized rectangle
                    Canvas.SetLeft(activeRectangle, Math.Min(startingPoint.X, currentPosition.X));
                    Canvas.SetTop(activeRectangle, Math.Min(startingPoint.Y, currentPosition.Y));
                }
            }
            // make sure that we are moving a rectangle
            if (movingRectangle != null)
            {
                //get current point and test that we are still in the image
                Point currentPosition = e.GetPosition(canvasImage);
                currentPosition.X -= offset.X;
                currentPosition.Y -= offset.Y;
                //Check that the Recatngle is still inside the Image
                if(Check_mouse_in_image(currentPosition) &&
                    Check_mouse_in_image(new Point(currentPosition.X, currentPosition.Y + movingRectangle.ActualHeight)) &&
                    Check_mouse_in_image(new Point(currentPosition.X + movingRectangle.ActualWidth, currentPosition.Y)) &&
                    Check_mouse_in_image(new Point(currentPosition.X + movingRectangle.ActualWidth, currentPosition.Y + movingRectangle.ActualHeight)))
                {
                    //set the position of the resized rectangle
                    Canvas.SetLeft(movingRectangle, currentPosition.X);
                    Canvas.SetTop(movingRectangle, currentPosition.Y);
                }
            }
        }

        private void CanvasImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Add the Adorner to the Rectangle
            Point imgPoint = imagePictures.TranslatePoint(new Point(0, 0), canvasImage);
            if(activeRectangle != null) AdornerLayer.GetAdornerLayer(canvasImage).Add(new RectangleAdoner(activeRectangle, startingPoint, imgPoint, imagePictures.ActualWidth, imagePictures.ActualHeight));
            // Reset the current active Rectangle
            activeRectangle = null;
            // Reset the moving Rectangle
            movingRectangle = null;
        }

        private void state_Checked(object sender, RoutedEventArgs e)
        {
            // Change the cursor in terms of what action the user wants to do
            System.Windows.Controls.RadioButton radio = sender as System.Windows.Controls.RadioButton;
            state_Cursor = radio.Content != null ? radio.Content.ToString() : "Add Rectangle";
            switch (state_Cursor)
            {
                case "Delete Rectangle":
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.No;
                    break;
                case "Move Rectangle":
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.SizeAll;
                    break;
                case "Change Rectangle Color":
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Pen;
                    break;
                default:
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    break;
            }

        }

        private void Upload_Img_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                // Filter what file can be uploaded
                openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.img;*.png";
                openFileDialog.FilterIndex = 1;
                // Show Dialog to choose wich file to upload
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // upload Image to Canvas and stretch to the dimension of the canvas
                    imagePictures.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    imagePictures.Stretch = Stretch.Uniform;
                    imagePictures.Width = canvasImage.Width;
                    imagePictures.Height = canvasImage.Height;
                }
                //Once an Image has been uploaded we can enabled the download button and radio buttons
                Download_Img.IsEnabled = true;
                moveRadio.IsEnabled = true;
                changeColorRadio.IsEnabled = true;
                deleteRadio.IsEnabled = true;
            }
            catch
            {

                System.Windows.MessageBox.Show("Error occured While Uploading an Image", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Download_Img_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            //Render Canvas as a Bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)canvasImage.ActualWidth,
                (int)canvasImage.ActualHeight,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvasImage);
            //Create PNG encoder to save Bitmap to a file
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            try
            {
                //Use Open File Dialog to indicate the name and location of the file we cant to create
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //check if file name has an extension otherwise add .png
                    if(System.IO.Path.HasExtension(saveFileDialog.FileName))
                    {
                        fileName = saveFileDialog.FileName;
                    } else
                    {
                        fileName = string.Concat(saveFileDialog.FileName,".png");
                    }
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        pngEncoder.Save(fileStream);
                    }
                }
            }
            catch
            {

                System.Windows.MessageBox.Show("Error occured While Downloading an Image", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
