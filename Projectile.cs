﻿using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace MiniRambo
{
    public class Projectile
    {
        private double Speed = 5;
        private bool From_Enemy { get; set; }
        private double Angle { get; set; }
        private List<Enemy> All_Enemies { get; set; }
        private Player_Info Player;
        private Canvas Main_Canvas;
        private Ellipse Bullet;
        private Gun Gun_Using;

        public Projectile(Gun gunUsing, bool isEnemy)
        {
            if (MainWindow.Instance != null && MainWindow.Instance.Player != null)
            {
                All_Enemies = MainWindow.Instance.All_Enemies;
                Player = MainWindow.Instance.Player;
                Angle = Player.Angle;
                Main_Canvas = Player.Main_Canvas;
            }
            else
                throw new InvalidOperationException();
            Gun_Using = gunUsing;
            From_Enemy = isEnemy;
            if (isEnemy) Speed = 1;
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

            Canvas.SetLeft(bulletEllipse, Gun_Using.Parent_Top);
            Canvas.SetTop(bulletEllipse, Gun_Using.Parent_Left);

            Main_Canvas.Children.Add(bulletEllipse);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = Angle;
            rotateTransform.CenterX = bulletEllipse.Width / 2;
            rotateTransform.CenterY = bulletEllipse.Height / 2;
            bulletEllipse.RenderTransform = rotateTransform;

            return bulletEllipse; 
        }

        private bool ProjectileTouched()
        {
            Rect projectile = new Rect(Canvas.GetLeft(Bullet), Canvas.GetTop(Bullet), Bullet.Width, Bullet.Height);
            if (From_Enemy)
            {
                if (projectile.IntersectsWith(Player.Player_Hitbox))
                {
                    Player.PlayerHit(1);
                    return true;
                }
            }
            else
            {
                foreach (Enemy enemy in All_Enemies)
                {
                    if (projectile.IntersectsWith(enemy.Enemy_Hitbox) && enemy.Allive)
                    {
                        enemy.EnemyHit(1);
                        Main_Canvas.Children.Remove(Bullet);
                        return true;
                    }
                }
            }
            return false;
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
                if (ProjectileTouched())
                    break;

                await Task.Delay(10);
            }
        }

    }
}
