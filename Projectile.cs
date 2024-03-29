using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace MiniRambo
{
    class Projectile
    {
        public int Speed = 5;
        private double Angle { get; set; }

        private Ellipse Bullet;
        private Canvas Main_Canvas;
        private Player_Info Player;

        public Projectile(Player_Info parent, Canvas main_canvas)
        {
            Main_Canvas = main_canvas;
            Player = parent;
            Angle = Player.Angle;
            Bullet = CreateProjectile();
            _ = MoveBullet();
        }

        private Ellipse CreateProjectile()
        {
            Ellipse bulletEllipse = new Ellipse();
            bulletEllipse.Width = 25;
            bulletEllipse.Height = 10;
            bulletEllipse.Stroke = Brushes.Black;
            bulletEllipse.Fill = Brushes.Black;

            Canvas.SetLeft(bulletEllipse, Player.X);
            Canvas.SetTop(bulletEllipse, Player.Y);

            Main_Canvas.Children.Add(bulletEllipse);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = Angle;
            rotateTransform.CenterX = bulletEllipse.Width / 2;
            rotateTransform.CenterY = bulletEllipse.Height / 2;
            bulletEllipse.RenderTransform = rotateTransform;

            return bulletEllipse; 
        }


        private async Task MoveBullet()
        {
            while (true)
            {
                double angle = Angle;

                double deltaX = Math.Cos(angle * Math.PI / 180) * Speed;
                double deltaY = Math.Sin(angle * Math.PI / 180) * Speed;

                Canvas.SetLeft(Bullet, Canvas.GetLeft(Bullet) + deltaX);
                Canvas.SetTop(Bullet, Canvas.GetTop(Bullet) + deltaY);

                if (Canvas.GetLeft(Bullet) < 0 || Canvas.GetLeft(Bullet) > Main_Canvas.Width || Canvas.GetTop(Bullet) < 0 || Canvas.GetTop(Bullet) > Main_Canvas.Height)
                {
                    Main_Canvas.Children.Remove(Bullet);
                    break;
                }

                await Task.Delay(10);
            }
        }

    }
}
