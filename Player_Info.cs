using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;

namespace MiniRambo
{
    internal class Player_Info
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }

        private Ellipse Player { get; set; }
        private Canvas Main_Canvas { get; set; }
        private List<double> CurrentMove = new List<double> { 0, 0 };
        private double Speed { get; set; } = 2;
        private double MouseX, MouseY;

        public Player_Info(Canvas mainCanvas)
        {
            X = mainCanvas.Width / 2;
            Y = mainCanvas.Height / 2;
            Main_Canvas = mainCanvas;
            Player = CreatePlayer();
        }

        public Ellipse CreatePlayer()
        {
            Ellipse playerEllipse = new Ellipse();
            playerEllipse.Width = 25;
            playerEllipse.Height = 25;
            playerEllipse.Stroke = Brushes.Black;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("../../../Img/rambo.jpeg", UriKind.Relative));
            playerEllipse.Fill = imageBrush;

            Canvas.SetLeft(playerEllipse, X);
            Canvas.SetTop(playerEllipse, Y);
            Main_Canvas.Children.Add(playerEllipse);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;
            rotateTransform.CenterX = 12.5;
            rotateTransform.CenterY = 12.5;
            playerEllipse.RenderTransform = rotateTransform;

            return playerEllipse;
        }

        public void PlayerMove()
        {

            double currentLeft = Canvas.GetLeft(Player);
            double currentTop = Canvas.GetTop(Player);

            if (currentLeft < 0) currentLeft = 1 * Speed;
            else if (currentLeft > Main_Canvas.Width - Player.Width) currentLeft += -1 * Speed;
            else if (currentTop < 0) currentTop += 1 * Speed;
            else if (currentTop > Main_Canvas.Height - Player.Height) currentTop += -1 * Speed;
            else
            {
                currentTop += Speed * CurrentMove[0];
                currentLeft += Speed * CurrentMove[1];
            }

            Canvas.SetLeft(Player, currentLeft);
            Canvas.SetTop(Player, currentTop);
            MoveAngle(MouseX, MouseY);

            X = Canvas.GetLeft(Player);
            Y = Canvas.GetTop(Player);
        }

        public void MoveAngle(double x, double y)
        {
            MouseX = x;
            MouseY = y;
            double angle = Math.Atan2(y - Canvas.GetTop(Player), x - Canvas.GetLeft(Player)) * (180 / Math.PI);
            RotateTransform? rotateTransform = Player.RenderTransform as RotateTransform;
            if (rotateTransform != null)
            {
                Angle = rotateTransform.Angle;
                rotateTransform.Angle = angle;
            }
        }

        public void PlayerInput(KeyEventArgs e, bool idle = false)
        {
            switch (e.Key)
            {
                case Key.A:
                    CurrentMove[1] = idle ? 0 : -1;
                    break;
                case Key.D:
                    CurrentMove[1] = idle ? 0 : 1;
                    break;
                case Key.W:
                    CurrentMove[0] = idle ? 0 : -1;
                    break;
                case Key.S:
                    CurrentMove[0] = idle ? 0 : 1;
                    break;
            }
        }

        public void Shoot()
        {
            _ = new Projectile(this, Main_Canvas);
        }
    }
}
