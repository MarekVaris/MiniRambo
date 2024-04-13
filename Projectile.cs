using System;
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
        private double _Speed = 5;
        private bool _Is_Enemy = false;
        private double _Angle { get; set; }

        private double _Top {  get; set; }
        private double _Left { get; set; }
        private List<Enemy> _All_Enemies { get; set; }
        private Player_Info _Player;
        private Canvas _Game_Canvas;
        private Canvas _Stop_Canvas;
        private Ellipse _Bullet;
        private SolidColorBrush _Bullet_Color;

        public Projectile(Enemy? enemy = null)
        {
            if (MainWindow.Instance != null && MainWindow.Instance.Player != null)
            {
                _All_Enemies = MainWindow.Instance.All_Enemies;
                _Game_Canvas = MainWindow.Instance.Game_Canvas;
                _Stop_Canvas = MainWindow.Instance.Stop_Canvas;
                _Player = MainWindow.Instance.Player;
                if(enemy == null)
                {
                    _Angle = _Player.Angle;
                    _Top = Canvas.GetTop(_Player.Player_Ellipse);
                    _Left = Canvas.GetLeft(_Player.Player_Ellipse);
                    _Bullet_Color = Brushes.Black;
                }
            }
            else
                throw new InvalidOperationException();
            if (enemy != null)
            {
                _Angle = enemy.Angle;
                _Is_Enemy = true;
                _Speed = 2;
                _Top = Canvas.GetTop(enemy.Enemy_Ellipse);
                _Left = Canvas.GetLeft(enemy.Enemy_Ellipse);
                _Bullet_Color = Brushes.Red;
            }

            _Bullet = CreateProjectile();
            _ = MoveBullet();
        }

        private Ellipse CreateProjectile()
        {
            Ellipse bulletEllipse = new Ellipse();
            bulletEllipse.Width = 25;
            bulletEllipse.Height = 10;
            bulletEllipse.Stroke = Brushes.Black;
            bulletEllipse.Fill = _Bullet_Color;

            Canvas.SetLeft(bulletEllipse, _Left);
            Canvas.SetTop(bulletEllipse, _Top);

            _Game_Canvas.Children.Add(bulletEllipse);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = _Angle;
            rotateTransform.CenterX = bulletEllipse.Width / 2;
            rotateTransform.CenterY = bulletEllipse.Height / 2;
            bulletEllipse.RenderTransform = rotateTransform;

            return bulletEllipse; 
        }

        private bool ProjectileTouched()
        {
            Rect projectile = new Rect(Canvas.GetLeft(_Bullet), Canvas.GetTop(_Bullet), _Bullet.Width, _Bullet.Height);
            if (_Is_Enemy)
            {
                if (projectile.IntersectsWith(_Player.Player_Hitbox))
                {
                    _Player.PlayerHit(1);
                    return true;
                }
            }
            else
            {
                foreach (Enemy enemy in _All_Enemies)
                {
                    if (projectile.IntersectsWith(enemy.Enemy_Hitbox) && enemy.Allive)
                    {
                        enemy.EnemyHit(1);
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task MoveBullet()
        {
            while(true)
            {   
                if (_Stop_Canvas.Visibility != Visibility.Visible || _Player.Hp <= 0)
                {
                    double angle = _Angle;

                    double deltaX = Math.Cos(angle * Math.PI / 180) * _Speed;
                    double deltaY = Math.Sin(angle * Math.PI / 180) * _Speed;

                    Canvas.SetLeft(_Bullet, Canvas.GetLeft(_Bullet) + deltaX);
                    Canvas.SetTop(_Bullet, Canvas.GetTop(_Bullet) + deltaY);

                    if (Canvas.GetLeft(_Bullet) < 0 || Canvas.GetLeft(_Bullet) > _Game_Canvas.Width || Canvas.GetTop(_Bullet) < 0 || Canvas.GetTop(_Bullet) > _Game_Canvas.Height)
                    {
                        _Game_Canvas.Children.Remove(_Bullet);
                        break;
                    }
                    if (ProjectileTouched())
                        break;
                }
                await Task.Delay(10);
            }
            _Game_Canvas.Children.Remove(_Bullet);
        }

    }
}
