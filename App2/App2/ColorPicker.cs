using NControl.Abstractions;
using NGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;

namespace NowMine
{
    class ColorPicker : NControlView, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double DrawScale { get; set; }
        public double Size { get; set; }
        public Xamarin.Forms.Color SelectedColor { get; set; }
        public BoxView RectSelectedColor { get; set; }

        public ColorPicker(double width = 300.0, double height = 375.0)
        {
            if (width > height)
                height += (width - height) + 75.0;
            this.HeightRequest = height;
            this.WidthRequest = width;
            Size = 120.0;
            var colorDrawing = new ColorDrawing(Size);
            var contentBox = new AbsoluteLayout ();
            contentBox.HeightRequest = height;
            contentBox.WidthRequest = width;
            
            //setting anchor to left up point for proper scaling
            colorDrawing.AnchorX = 0.0;
            colorDrawing.AnchorY = 0.0;

            DrawScale = width / Size;
            colorDrawing.Scale = DrawScale;
            //contentBox.BackgroundColor = Xamarin.Forms.Color.Blue;  --> touch nie działa
            contentBox.Children.Add(colorDrawing);

            RectSelectedColor = new BoxView();
            RectSelectedColor.BackgroundColor = Xamarin.Forms.Color.Red;
            RectSelectedColor.WidthRequest = width;
            RectSelectedColor.HeightRequest = height - width;
            RectSelectedColor.TranslationY = width;
            
            contentBox.Children.Add(RectSelectedColor);
            Content = contentBox;

            //this.BackgroundColor = Xamarin.Forms.Color.Yellow;
        }

        public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
        {
            //base.Draw(canvas, rect);
            //canvas.DrawLine(rect.Left, rect.Top, rect.Width, rect.Height, NGraphics.Colors.Blue);
            //canvas.DrawLine(rect.Width, rect.Top, rect.Left, rect.Height, NGraphics.Colors.Yellow);
        }

        public override bool TouchesBegan(IEnumerable<NGraphics.Point> points)
        {
            base.TouchesBegan(points);
            double x = points.First().X / DrawScale;
            double y = points.First().Y / DrawScale;
            double hue = (((x / Size) * 360.0) / 360.0);
            double satlat = y / Size;
            SelectedColor = Xamarin.Forms.Color.FromHsla(hue, satlat, satlat);
            RectSelectedColor.BackgroundColor = SelectedColor;
            OnPropertyChanged("SelectedColor");
            return true;
        }
    }

    class ColorDrawing : NControlView
    {
        private NGraphics.Color[,] ColorPickerColors { get; set; }

        public ColorDrawing(double size)
        {
            this.WidthRequest = size;
            this.HeightRequest = size;

            ColorPickerColors = new NGraphics.Color[120, 120];
            double lightness = 0.0;
            double saturation = 0.0;
            for (int y = 0; y < 120; y++)
            {
                for (int x = 0; x < 120; x++)
                {

                    double hue = ((((double)x / 120.0) * 360.0) / 360.0);
                    ColorPickerColors[x, y] = NGraphics.Color.FromHSL(hue, saturation, lightness);
                }
                lightness = ((double)y / 120.0);
                saturation = ((double)y / 120.0);
            }
        }

        public override void Draw(ICanvas canvas, Rect rect)
        {
            base.Draw(canvas, rect);
            //double lightness = 0.0;
            //double saturation = 0.0;
            //int rectBoundX = (int)rect.Width;
            //int rectBoundY = (int)rect.Height;
            int rectBoundX = 120;
            int rectBoundY = 120;
            for (int y = 0; y < rectBoundX; y++)
            {
                for (int x = 0; x < rectBoundY; x++)
                {
                    //double hue = ((((double)x / (double)rect.Width) * 360.0) / 360.0);
                    //var newColor = NGraphics.Color.FromHSL(hue, saturation, lightness);
                    //Debug.WriteLine("x {0} y {1} hue {2} sat {3} lgh {4}", x, y, hue, saturation, lightness);
                    var colRec = new Rect(x, y, 1, 1);
                    //canvas.FillRectangle(colRec, newColor);
                    canvas.FillRectangle(colRec, ColorPickerColors[x,y]);
                }
                //lightness = ((double)y / (double)rectBoundY);
                //saturation = ((double)y / (double)rectBoundY);
            }
        }
    }
}
