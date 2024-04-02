﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiniRambo
{
    public class Enemy
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Rect Enemy_Hitbox { get; set; }
        public Ellipse Enemy_Ellipse { get; set; }
        public bool Allive { get; set; } = true;
        private int Hp { get; set; } = 2;
        private double Speed = 0;
        private Canvas Main_Canvas { get; set; }
        private Player_Info Player { get; set; }

        public Enemy()
        {
            if (MainWindow.Instance != null && MainWindow.Instance.Player != null)
            {
                Player = MainWindow.Instance.Player;
                Main_Canvas = Player.Main_Canvas;
            }
            else
                throw new InvalidOperationException();

            Enemy_Ellipse = CreateEnemy();
            _ = EnemyMove();
        }

        private Ellipse CreateEnemy()
        {
            Ellipse enemyEllipse = new Ellipse();
            enemyEllipse.Width = 40;
            enemyEllipse.Height = 40;
            enemyEllipse.Stroke = Brushes.Black;
            
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("../../../Img/Enemy.png", UriKind.Relative));
            enemyEllipse.Fill = imageBrush;

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;
            rotateTransform.CenterX = enemyEllipse.Width / 2;
            rotateTransform.CenterY = enemyEllipse.Height / 2;
            enemyEllipse.RenderTransform = rotateTransform;


            Random random = new Random();
            Speed = random.Next(70, 100) / 100.0;

            int widthMax = (int)Main_Canvas.Width - 20;
            int heightMax = (int)Main_Canvas.Height - 20;
            if (random.Next(0,2) == 1)
            {
                if (random.Next(0, 2) == 1)
                    Canvas.SetTop(enemyEllipse, heightMax);
                else
                    Canvas.SetTop(enemyEllipse, 0);
                Canvas.SetLeft(enemyEllipse, random.Next(0, widthMax));
            }
            else
            {
                if (random.Next(0, 2) == 1)
                    Canvas.SetLeft(enemyEllipse, 0);
                else
                    Canvas.SetLeft(enemyEllipse, widthMax);
                Canvas.SetTop(enemyEllipse, random.Next(0, heightMax));
            }
          

            Main_Canvas.Children.Add(enemyEllipse);

            return enemyEllipse;
        }

        public void EnemyHit(int dmg)
        {
            Hp -= dmg;
        }

        private void EnemyTouchPlayer()
        {
            Enemy_Hitbox = new Rect(Canvas.GetLeft(Enemy_Ellipse), Canvas.GetTop(Enemy_Ellipse), Enemy_Ellipse.Width, Enemy_Ellipse.Height);
            if (Enemy_Hitbox.IntersectsWith(Player.Player_Hitbox))
            {
                Player.PlayerHit(1);
            }
        }

        private async Task EnemyMove()
        {
            while (Hp > 0)
            {
                
                double angle = Math.Atan2(Player.Y - Canvas.GetTop(Enemy_Ellipse), Player.X - Canvas.GetLeft(Enemy_Ellipse)) * (180 / Math.PI);
                double deltaX = Math.Cos(angle * Math.PI / 180) * Speed;
                double deltaY = Math.Sin(angle * Math.PI / 180) * Speed;
                RotateTransform? rotateTransform = Enemy_Ellipse.RenderTransform as RotateTransform;
                if (rotateTransform != null) rotateTransform.Angle = angle;

                Canvas.SetLeft(Enemy_Ellipse, Canvas.GetLeft(Enemy_Ellipse) + deltaX);
                Canvas.SetTop(Enemy_Ellipse, Canvas.GetTop(Enemy_Ellipse) + deltaY);

                EnemyTouchPlayer();

                await Task.Delay(10);
            }
            Allive = false;
            Enemy_Ellipse.Opacity = 0.4;        
        }
    }

}
