using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniRambo
{
    internal class Game_Border
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public Rectangle Border = new Rectangle();
        private Canvas Main_Canvas { get; set; }

        public Game_Border(double height, double width, Canvas patent) 
        {
            Height = height;
            Width = width;
            Main_Canvas = patent;
            Border = CreateBorder();
        }

        private Rectangle CreateBorder()
        {
            Rectangle borderRect = new Rectangle();
            borderRect.Width = Width;
            borderRect.Height = Height;
            borderRect.Stroke = Brushes.Black;
            Main_Canvas.Children.Add(borderRect);

            return borderRect;
        }
    }
}
