using System;
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
        public bool Allive { get; set; } = true;
        public double Angle { get; set; }
        public Ellipse Enemy_Ellipse { get; set; }
        public Rect Enemy_Hitbox { get; set; }

        private int _Hp { get; set; } = 2;
        private double _Speed { get; set; } = 0;
        private int _Type { get; set; } = 1;
        private int _Current_Lvl { get; set; }
        private Canvas _Game_Canvas { get; set; }
        private Canvas _Stop_Canvas { get; set; }
        private Player_Info _Player { get; set; }
        private Gun? _Enemy_Gun { get; set; }

        public Enemy()
        {
            if (MainWindow.Instance != null && MainWindow.Instance.Player != null)
            {
                _Player = MainWindow.Instance.Player;
                _Current_Lvl = MainWindow.Instance.Lvl;
                _Game_Canvas = MainWindow.Instance.Game_Canvas;
                _Stop_Canvas = MainWindow.Instance.Stop_Canvas;
            }
            else
                throw new InvalidOperationException();

            Enemy_Ellipse = CreateEnemy();
            _ = EnemyMove();
        }

        public void EnemyHit(int dmg)
        {
            _Hp -= dmg;
            if (_Hp <= 0)
            {
                Allive = false;
            }
        }

        private Ellipse CreateEnemy()
        {
            Ellipse enemyEllipse = new Ellipse();
            enemyEllipse.Width = 40;
            enemyEllipse.Height = 40;
            enemyEllipse.Stroke = Brushes.Black;

            Random random = new Random();
            if (_Current_Lvl>1 && random.Next(0, 4) == 1)
            {
                _Type = 2;
                enemyEllipse.Fill = MainWindow.Instance?.LoadImg("Enemy2.png");
                _Enemy_Gun = new Gun();
            }
            else
            {
                enemyEllipse.Fill = MainWindow.Instance?.LoadImg("Enemy.png");
            }
            _Hp = _Current_Lvl + _Type;

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;
            rotateTransform.CenterX = enemyEllipse.Width / 2;
            rotateTransform.CenterY = enemyEllipse.Height / 2;
            enemyEllipse.RenderTransform = rotateTransform;

            _Speed = random.Next(70, 100) / 100.0;

            int widthMax = (int)_Game_Canvas.Width - 20;
            int heightMax = (int)_Game_Canvas.Height - 20;
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
          

            _Game_Canvas.Children.Add(enemyEllipse);

            return enemyEllipse;
        }

        private void EnemyTouchPlayer()
        {
            Enemy_Hitbox = new Rect(Canvas.GetLeft(Enemy_Ellipse), Canvas.GetTop(Enemy_Ellipse), Enemy_Ellipse.Width, Enemy_Ellipse.Height);
            if (Enemy_Hitbox.IntersectsWith(_Player.Player_Hitbox))
            {
                _Player.PlayerHit(1);
            }
        }

        private async Task EnemyMove()
        {
            int fireCoolDown = 0;

            while (Allive)
            {
                if (_Stop_Canvas.Visibility != Visibility.Visible)
                {

                    double angle = Math.Atan2(_Player.Y - Canvas.GetTop(Enemy_Ellipse), _Player.X - Canvas.GetLeft(Enemy_Ellipse)) * (180 / Math.PI);
                    double deltaX = Math.Cos(angle * Math.PI / 180) * _Speed;
                    double deltaY = Math.Sin(angle * Math.PI / 180) * _Speed;
                    RotateTransform? rotateTransform = Enemy_Ellipse.RenderTransform as RotateTransform;
                    if (rotateTransform != null)
                    {
                        rotateTransform.Angle = angle;
                        Angle = angle;
                    }


                    Canvas.SetLeft(Enemy_Ellipse, Canvas.GetLeft(Enemy_Ellipse) + deltaX);
                    Canvas.SetTop(Enemy_Ellipse, Canvas.GetTop(Enemy_Ellipse) + deltaY);

                    EnemyTouchPlayer();

                    if (_Enemy_Gun != null)
                    {
                        if(fireCoolDown > 100)
                        {
                            _Enemy_Gun.Shoot(this);
                            fireCoolDown = 0;
                        }
                        else
                            fireCoolDown++;
                    }
                }
                await Task.Delay(10);
            }
            if (_Hp <= 0)
            {
                AddPoints();
                Enemy_Ellipse.Opacity = 0.4;        
            }
        }

        private void AddPoints()
        {
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.Score += 100 * _Current_Lvl * _Type;
                MainWindow.Instance.Coins += 5 * _Current_Lvl * _Type;

                MainWindow.Instance.nextLvlBar.Value += 100 / (10 * _Current_Lvl);
                MainWindow.Instance.UpdatePoints();
            }
        }
    }
}
