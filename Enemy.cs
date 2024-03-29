using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniRambo
{
    class Enemy
    {
        private double Speed = 0;
        private Canvas Main_Canvas;
        private Ellipse Current_Enemy { get; set; }
        private Player_Info Player { get; set; }

        public Enemy(Canvas mainCanvas, Player_Info player)
        {
            Main_Canvas = mainCanvas;
            Player = player;
            Current_Enemy = CreateEnemy();
            Task.Delay(100);
            _ = EnemyMove();
        }


        private Ellipse CreateEnemy()
        {
            Ellipse enemyEllipse = new Ellipse();
            enemyEllipse.Width = 20;
            enemyEllipse.Height = 20;
            enemyEllipse.Stroke = Brushes.Black;
            enemyEllipse.Fill = Brushes.Red;

            Random random = new Random();
            Speed = random.Next(40, 70) / 100.0;

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

        private async Task EnemyMove()
        {

            while (true)
            {
                double angle = Math.Atan2(Player.Y - Canvas.GetTop(Current_Enemy), Player.X - Canvas.GetLeft(Current_Enemy)) * (180 / Math.PI);
                double deltaX = Math.Cos(angle * Math.PI / 180) * Speed;
                double deltaY = Math.Sin(angle * Math.PI / 180) * Speed;

                Canvas.SetLeft(Current_Enemy, Canvas.GetLeft(Current_Enemy) + deltaX);
                Canvas.SetTop(Current_Enemy, Canvas.GetTop(Current_Enemy) + deltaY);
                
                await Task.Delay(10);
            }
        }
    }

}
