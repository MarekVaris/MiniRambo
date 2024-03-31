using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows;

namespace MiniRambo
{
    public class Player_Info
    {
        public int Hp = 5;
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public Canvas Main_Canvas { get; set; }
        public Rect Player_Hitbox { get; set; }

        private Ellipse Player_Ellipse { get; set; }
        private List<double> CurrentMove = new List<double> { 0, 0 };
        private double Speed { get; set; } = 2;
        private double MouseX, MouseY;
        private bool PlayerHitCooldow = true;

        public Player_Info()
        {
            if (MainWindow.Instance != null)
            {
                Main_Canvas = MainWindow.Instance.Main_Canvas;
                X = Main_Canvas.Width / 2;
                Y = Main_Canvas.Height / 2;
            }
            else
                throw new InvalidOperationException();

            Player_Ellipse = CreatePlayer();
        }

        public Ellipse CreatePlayer()
        {
            Ellipse playerEllipse = new Ellipse();
            playerEllipse.Width = 25;
            playerEllipse.Height = 25;
            playerEllipse.Stroke = Brushes.Black;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("../../../Img/Rambo.png", UriKind.Relative));
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
            double currentLeft = Canvas.GetLeft(Player_Ellipse);
            double currentTop = Canvas.GetTop(Player_Ellipse);

            if (currentLeft < 0) currentLeft = 1 * Speed;
            else if (currentLeft > Main_Canvas.Width - Player_Ellipse.Width) currentLeft += -1 * Speed;
            else if (currentTop < 0) currentTop += 1 * Speed;
            else if (currentTop > Main_Canvas.Height - Player_Ellipse.Height) currentTop += -1 * Speed;
            else
            {
                currentTop += Speed * CurrentMove[0];
                currentLeft += Speed * CurrentMove[1];
            }

            Canvas.SetLeft(Player_Ellipse, currentLeft);
            Canvas.SetTop(Player_Ellipse, currentTop);
            MoveAngle(MouseX, MouseY);

            X = Canvas.GetLeft(Player_Ellipse);
            Y = Canvas.GetTop(Player_Ellipse);

            Player_Hitbox = new Rect(currentLeft, currentTop, Player_Ellipse.Width, Player_Ellipse.Height);
        }

        public void MoveAngle(double x, double y)
        {
            MouseX = x;
            MouseY = y;
            double angle = Math.Atan2(y - Canvas.GetTop(Player_Ellipse), x - Canvas.GetLeft(Player_Ellipse)) * (180 / Math.PI);
            RotateTransform? rotateTransform = Player_Ellipse.RenderTransform as RotateTransform;
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

        public async void PlayerHit(int dmg)
        {
            if (PlayerHitCooldow)
            {
                PlayerHitCooldow = false;
                Hp -= dmg;
                for (int i = 0; i < 3; i++)
                {
                    Player_Ellipse.Opacity = 0.3;
                    await Task.Delay(500); 

                    Player_Ellipse.Opacity = 1.0;
                    await Task.Delay(500);
                }
                PlayerHitCooldow = true;
            }
        }

        public void Shoot()
        {
            _ = new Projectile(false);
        }
    }
}
