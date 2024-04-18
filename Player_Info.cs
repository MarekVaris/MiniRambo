using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Numerics;
using System.Reflection.Metadata;

namespace MiniRambo
{
    public class Player_Info
    {
        public int Hp { get; set; }
        public double Speed { get; set; }
        public double Proj_Spread { get; set; } = 0;
        public double A_Speed { get; set; } = 1;
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public Ellipse Player_Ellipse { get; set; }
        public Gun Player_Gun { get; set; }
        public Rect Player_Hitbox { get; set; }

        private List<double> _Current_Move { get; set; } = new List<double> { 0, 0 };
        private Canvas _Game_Canvas { get; set; }
        private Canvas _Stop_Canvas { get; set; }
        private StackPanel Health_Ui { get; set; }
        private bool Player_Hit_Cooldow = true;
        private double MouseX, MouseY;

        public Player_Info(int hp, double speed)
        {
            
            Speed = speed;
            if (MainWindow.Instance != null)
            {
                Hp = hp;
                Health_Ui = MainWindow.Instance.healthUi;
                UpdateHealthUi();

                _Game_Canvas = MainWindow.Instance.Game_Canvas;
                _Stop_Canvas = MainWindow.Instance.Stop_Canvas;

                X = _Game_Canvas.Width / 2;
                Y = _Game_Canvas.Height / 2;
            }
            else
                throw new InvalidOperationException();
            Player_Ellipse = CreatePlayer();

            Player_Gun = new Gun(12 + MainWindow.Instance.Shop_Game.Main_Shop[4], true);
        }

        public void UpdateHealthUi(int bonus = 0)
        {
            while (Health_Ui.Children.Count > 0)
            {
                Health_Ui.Children.RemoveAt(0);
            }

            for (int i = Health_Ui.Children.Count; i < Hp + bonus; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Fill = MainWindow.Instance?.LoadImg("Heart.png");
                rect.Height = 40;
                rect.Width = 40;
                rect.Margin = new Thickness(5);
                Health_Ui.Children.Add(rect);
            }
        }

        public void PlayerInput(KeyEventArgs e, bool idle = false)
        {
            switch (e.Key)
            {
                case Key.A:
                    if (_Current_Move[1] == 1) break;
                    _Current_Move[1] = idle ? 0 : -1;
                    break;
                case Key.D:
                    if (_Current_Move[1] == -1) break;
                    _Current_Move[1] = idle ? 0 : 1;
                    break;
                case Key.W:
                    if (_Current_Move[0] == 1) break;
                    _Current_Move[0] = idle ? 0 : -1;
                    break;
                case Key.S:
                    if (_Current_Move[0] == -1) break;
                    _Current_Move[0] = idle ? 0 : 1;
                    break;
                case Key.R:
                    if (idle && _Stop_Canvas.Visibility != Visibility.Visible)
                        Player_Gun.Reload();
                    break;
                case Key.Space:
                    if (idle && Hp > 0 
                        && _Stop_Canvas.Visibility != Visibility.Visible 
                        && _Game_Canvas.Visibility == Visibility.Visible 
                        && MainWindow.Instance?.Shop_Canvas.Visibility != Visibility.Visible)
                            Player_Gun.Shoot();
                    break;
            }
        }
        public void PlayerMove()
        {
            double currentLeft = Canvas.GetLeft(Player_Ellipse);
            double currentTop = Canvas.GetTop(Player_Ellipse);

            if (currentLeft < 0) currentLeft = 1 * Speed;
            else if (currentLeft > _Game_Canvas.Width - Player_Ellipse.Width) currentLeft += -1 * Speed;
            else if (currentTop < 0) currentTop += 1 * Speed;
            else if (currentTop > _Game_Canvas.Height - Player_Ellipse.Height) currentTop += -1 * Speed;
            else
            {
                currentTop += Speed * _Current_Move[0];
                currentLeft += Speed * _Current_Move[1];
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
                rotateTransform.Angle = angle;
                Angle = rotateTransform.Angle;
            }
        }


        public async void PlayerHit(int dmg)
        {
            if (Player_Hit_Cooldow && Hp > 0)
            {
                Player_Hit_Cooldow = false;
                Hp -= dmg;
                if (Health_Ui.Children.Count > 0)
                {
                    Health_Ui.Children.RemoveAt(Health_Ui.Children.Count - 1);
                }
                if (Hp <= 0)
                {
                    MainWindow.Instance?.StopGame();
                    Player_Ellipse.Opacity = 0.5;
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Player_Ellipse.Opacity = 0.3;
                        await Task.Delay(500); 

                        Player_Ellipse.Opacity = 1.0;
                        await Task.Delay(500);
                    }
                    Player_Hit_Cooldow = true;
                }
            }
        }


        private Ellipse CreatePlayer()
        {
            Ellipse playerEllipse = new Ellipse();
            playerEllipse.Width = 25;
            playerEllipse.Height = 25;
            playerEllipse.Stroke = Brushes.Black;
            playerEllipse.Opacity = 0;
            playerEllipse.Fill = MainWindow.Instance?.LoadImg("Rambo.png");
            Panel.SetZIndex(playerEllipse, 1);

            Canvas.SetLeft(playerEllipse, X);
            Canvas.SetTop(playerEllipse, Y);
            _Game_Canvas.Children.Add(playerEllipse);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;
            rotateTransform.CenterX = 12.5;
            rotateTransform.CenterY = 12.5;
            playerEllipse.RenderTransform = rotateTransform;

            return playerEllipse;
        }

    }
}
