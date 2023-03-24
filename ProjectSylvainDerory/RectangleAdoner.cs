using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectSylvainDerory
{
    partial class RectangleAdoner : Adorner
    {
        VisualCollection RectangleAdornerVisuals;
        Thumb thumbUL, thumbUM, thumbUR, thumbMR, thumbDR, thumbDM, thumbDL, thumbML;
        bool isVisible;
        Point myPoint;
        int minimumSize = 10;
        double minX, maxX, minY, maxY;
        Cursor activeCursor;

        public RectangleAdoner(UIElement adornedElement, Point startingPoint, Point imgPoint, double imgWidth, double imgHeight) : base(adornedElement)
        {
            //Initialize and add elements and their methods to manage the resize
            RectangleAdornerVisuals = new VisualCollection(this);
            SolidColorBrush transparent = Brushes.Transparent;
            thumbUL = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbUM = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbUR = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbMR = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbDR = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbDM = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbDL = new Thumb() { Background = transparent, Height = 15, Width = 15 };
            thumbML = new Thumb() { Background = transparent, Height = 15, Width = 15 };

            thumbUL.DragDelta += Move_Diagonal_Up_Left;
            thumbUM.DragDelta += Move_Vertical_Up;
            thumbUR.DragDelta += Move_Diagonal_Up_Right;
            thumbMR.DragDelta += Move_Horizontal_Right;
            thumbDR.DragDelta += Move_Diagonal_Down_Right;
            thumbDM.DragDelta += Move_Vertical_Down;
            thumbDL.DragDelta += Move_Diagonal_Down_Left;
            thumbML.DragDelta += Move_Horizontal_Left;

            RectangleAdornerVisuals.Add(thumbUL);
            RectangleAdornerVisuals.Add(thumbUM);
            RectangleAdornerVisuals.Add(thumbUR);
            RectangleAdornerVisuals.Add(thumbMR);
            RectangleAdornerVisuals.Add(thumbDR);
            RectangleAdornerVisuals.Add(thumbDM);
            RectangleAdornerVisuals.Add(thumbDL);
            RectangleAdornerVisuals.Add(thumbML);

            myPoint = startingPoint;
            minX = imgPoint.X;
            maxX = imgPoint.X + imgWidth;
            minY = imgPoint.Y;
            maxY = imgPoint.Y + imgHeight;
        }

        private bool Check_mouse_in_image(Point mouse)
        {
            if (mouse.X >= minX && mouse.X < maxX && mouse.Y >= minY && mouse.Y < maxY)
            {
                return true;
            }
            return false;
        }

        private bool Check_mouse_in_image_treshhold(Point mouse)
        {
            //Added a treshhold so that when you are at the treshold the elment is put back inside the image 
            //with a minimum value otherwise it will not be accesible anymore
            if (mouse.X >= minX - 1 && mouse.X < maxX + 1 && mouse.Y >= minY - 1 && mouse.Y < maxY + 1)
            {
                return true;
            }
            return false;
        }

        private void Move_Horizontal_Left(object sender, DragDeltaEventArgs e)
        {
            //manage the mouvement to the left and change the icon
            //cannot go further than the image and if it reaches the
            //other side it is 0 we do not manage negative values
            //store the cursor before the resize to go back to it afterwards
            if (Mouse.OverrideCursor != Cursors.SizeWE) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeWE;
            var ele = (FrameworkElement)AdornedElement;
            var newX = ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange;
            if (Check_mouse_in_image(new Point(newX,myPoint.Y)))
            {
                myPoint.X = newX;
                ele.Width = ele.Width - e.HorizontalChange < minimumSize ? minimumSize : ele.Width - e.HorizontalChange;
                Canvas.SetLeft(ele, myPoint.X);
            }
            else if (Check_mouse_in_image_treshhold(new Point(newX, myPoint.Y)))
            {
                myPoint.X = myPoint.X + minimumSize;
                ele.Width = ele.Width - minimumSize;
                Canvas.SetLeft(ele, myPoint.X);
            }
        }

        private void Move_Diagonal_Down_Left(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNESW) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNESW;
            var ele = (FrameworkElement)AdornedElement;
            var newX = ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange;
            var newY = (ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange) + ele.Height;
            if (Check_mouse_in_image(new Point(newX, newY)))
            {
                myPoint.X = newX;
                ele.Height = ele.Height + e.VerticalChange < minimumSize ? minimumSize : ele.Height + e.VerticalChange;
                ele.Width = ele.Width - e.HorizontalChange < minimumSize ? minimumSize : ele.Width - e.HorizontalChange;
                Canvas.SetLeft(ele, myPoint.X);
            }
            else if(Check_mouse_in_image_treshhold(new Point(newX, newY))){
                if (Check_mouse_in_image_treshhold(new Point(newX, myPoint.Y)))
                {
                    myPoint.X = myPoint.X + minimumSize;
                    ele.Width = ele.Width - minimumSize;
                    Canvas.SetLeft(ele, myPoint.X);
                }
                else
                {
                    ele.Height = ele.Height + minimumSize;
                }
            }
        }

        private void Move_Vertical_Down(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNS) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNS;
            var ele = (FrameworkElement)AdornedElement;
            var newY = (ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange) + ele.Height;
            if (Check_mouse_in_image(new Point(myPoint.X, newY)))
            {
                ele.Height = ele.Height + e.VerticalChange < minimumSize ? minimumSize : ele.Height + e.VerticalChange;
            }
            else if (Check_mouse_in_image_treshhold(new Point(myPoint.X, newY)))
            {
                ele.Height = ele.Height + minimumSize;
            }
        }

        private void Move_Diagonal_Down_Right(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNWSE) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNWSE;
            var ele = (FrameworkElement)AdornedElement;
            var newX = (ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange) + ele.Width;
            var newY = (ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange) + ele.Height;
            if (Check_mouse_in_image(new Point(newX, newY)))
            {
                ele.Height = ele.Height + e.VerticalChange < minimumSize ? minimumSize : ele.Height + e.VerticalChange;
                ele.Width = ele.Width + e.HorizontalChange < minimumSize ? minimumSize : ele.Width + e.HorizontalChange;
            }
            else if(Check_mouse_in_image_treshhold(new Point(newX, newY)))
            {
                if (Check_mouse_in_image_treshhold(new Point(myPoint.X, newY)))
                {
                    ele.Height = ele.Height + minimumSize;
                }
                else
                {
                    ele.Width = ele.Width - minimumSize;
                }
            }

        }

        private void Move_Horizontal_Right(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeWE) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeWE;
            var ele = (FrameworkElement)AdornedElement;
            var newX = (ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange) + ele.Width;
            if (Check_mouse_in_image(new Point(newX, myPoint.Y)))
            {
                ele.Width = ele.Width + e.HorizontalChange < minimumSize ? minimumSize : ele.Width + e.HorizontalChange;
            }
            else if (Check_mouse_in_image_treshhold(new Point(newX, myPoint.Y)))
            {
                ele.Width = ele.Width - minimumSize;
            }
        }

        private void Move_Diagonal_Up_Right(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNESW) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNESW;
            var ele = (FrameworkElement)AdornedElement;
            var newX = (ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange) + ele.Width;
            var newY = ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange;
            if (Check_mouse_in_image(new Point(newX, newY)))
            {
                ele.Height = ele.Height - e.VerticalChange < minimumSize ? minimumSize : ele.Height - e.VerticalChange;
                ele.Width = ele.Width + e.HorizontalChange < minimumSize ? minimumSize : ele.Width + e.HorizontalChange;
                myPoint.Y = newY;
                Canvas.SetTop(ele, myPoint.Y);
            }
            else if (Check_mouse_in_image_treshhold(new Point(newX, newY)))
            {
                if (Check_mouse_in_image_treshhold(new Point(myPoint.X, newY)))
                {
                    ele.Height = ele.Height + minimumSize;
                    myPoint.Y = myPoint.Y - minimumSize;
                    Canvas.SetTop(ele, myPoint.Y);
                }
                else
                {
                    ele.Width = ele.Width - minimumSize;
                }
            }
        }

        private void Move_Vertical_Up(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNS) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNS;
            var ele = (FrameworkElement)AdornedElement;
            var newY = ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange;
            if (Check_mouse_in_image(new Point(myPoint.X, newY)))
            {
                ele.Height = ele.Height - e.VerticalChange < minimumSize ? minimumSize : ele.Height - e.VerticalChange;
                myPoint.Y = newY;
                Canvas.SetTop(ele, myPoint.Y);
            }
            else if (Check_mouse_in_image_treshhold(new Point(myPoint.X, newY)))
            {
                ele.Height = ele.Height - minimumSize;
                myPoint.Y = myPoint.Y + minimumSize;
                Canvas.SetTop(ele, myPoint.Y);
            }
        }

        private void Move_Diagonal_Up_Left(object sender, DragDeltaEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.SizeNWSE) activeCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.SizeNWSE;
            var ele = (FrameworkElement)AdornedElement;
            var newX = ele.Width - e.HorizontalChange < minimumSize ? myPoint.X : myPoint.X + e.HorizontalChange;
            var newY = ele.Height - e.VerticalChange < minimumSize ? myPoint.Y : myPoint.Y + e.VerticalChange;
            if (Check_mouse_in_image(new Point(newX, newY)))
            {
                ele.Height = ele.Height - e.VerticalChange < minimumSize ? minimumSize : ele.Height - e.VerticalChange;
                ele.Width = ele.Width - e.HorizontalChange < minimumSize ? minimumSize : ele.Width - e.HorizontalChange;
                myPoint.X = newX;
                myPoint.Y = newY;
                Canvas.SetLeft(ele, myPoint.X);
                Canvas.SetTop(ele, myPoint.Y);
            }
            else if (Check_mouse_in_image_treshhold(new Point(myPoint.X, newY)))
            {
                ele.Height = ele.Height + minimumSize;
                myPoint.Y = myPoint.Y - minimumSize;
                Canvas.SetTop(ele, myPoint.Y);
            }
            else if (Check_mouse_in_image_treshhold(new Point(newX, myPoint.Y)))
            {
                myPoint.X = myPoint.X + minimumSize;
                ele.Width = ele.Width - minimumSize;
                Canvas.SetLeft(ele, myPoint.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            isVisible = true;
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Mouse.OverrideCursor = activeCursor;
            isVisible = false;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if(isVisible)
            {
                Pen rectPen = new Pen(Brushes.Gray, 2);
                rectPen.DashStyle = DashStyles.Dash;
                drawingContext.DrawRectangle(null, rectPen, new Rect(-2.5, -2.5, AdornedElement.DesiredSize.Width + 5, AdornedElement.DesiredSize.Height + 5));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1),new Rect(-5, -5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect((AdornedElement.DesiredSize.Width / 2) - 5, -5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect(AdornedElement.DesiredSize.Width - 5, -5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect(-5, (AdornedElement.DesiredSize.Height / 2) - 5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect(AdornedElement.DesiredSize.Width - 5, (AdornedElement.DesiredSize.Height / 2) - 5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect(-5, AdornedElement.DesiredSize.Height - 5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect((AdornedElement.DesiredSize.Width / 2) - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));
                drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1), new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));

            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return RectangleAdornerVisuals[index];
        }

        protected override int VisualChildrenCount => (RectangleAdornerVisuals != null) ? RectangleAdornerVisuals.Count : 0;

        protected override Size ArrangeOverride(Size finalSize)
        {
            thumbUL.Arrange(new Rect(-5, -5, 15, 15));
            thumbUM.Arrange(new Rect((AdornedElement.DesiredSize.Width / 2) - 5, -5, 15, 15));
            thumbUR.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, -5, 15, 15));
            thumbML.Arrange(new Rect(-5, (AdornedElement.DesiredSize.Height / 2) - 5, 15, 15));
            thumbMR.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, (AdornedElement.DesiredSize.Height / 2) - 5, 15, 15));
            thumbDL.Arrange(new Rect(-5, AdornedElement.DesiredSize.Height - 5, 15, 15));
            thumbDM.Arrange(new Rect((AdornedElement.DesiredSize.Width / 2) - 5, AdornedElement.DesiredSize.Height - 5, 15, 15));
            thumbDR.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 15, 15));

            return base.ArrangeOverride(finalSize);
        }
    }
}
