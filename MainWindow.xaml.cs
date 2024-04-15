using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace MiniRambo
{
    public partial class MainWindow : Window
    {
        public int Lvl { get; set; } = 1;
        public int Score { get; set; } = 0;
        public int Coins { get; set; } = 0;
        public List<int> Shop { get; set; } = [0,0,0];

        private bool _Bar_Ready { get; set; } = false;
        private int _Enemy_Spawning_Rate { get; set; } = 0;
        public Canvas Game_Canvas { get; set; }
        public Canvas Menu_Canvas { get; set; }
        public Canvas Stop_Canvas {  get; set; }
        public List<Enemy> All_Enemies { get; set; }
        public Player_Info Player {  get; set; }
        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            Game_Canvas = gameCanvas;
            Menu_Canvas = mainMenu;
            Stop_Canvas = stopCanvas;

            Player = new Player_Info(3, 2);
            All_Enemies = new List<Enemy>();
        }


        private async Task GameStart()
        {
            Player.Player_Ellipse.Opacity = 1;
            Game_Canvas.Visibility = Visibility.Visible;

            while (Player.Hp > 0)
            {
                if (Stop_Canvas.Visibility == Visibility.Hidden)
                {
                    Player.PlayerMove();
                    if (Player.Player_Gun.Ready_To_Shoot < 100)
                        Player.Player_Gun.Ready_To_Shoot += Player.A_Speed;
                    if (Player.Proj_Spread > 0)
                        Player.Proj_Spread -= 0.1;
                    else
                        Player.Proj_Spread = 0;


                    if (_Enemy_Spawning_Rate > 100)
                    {
                        All_Enemies.Add(new Enemy());
                        _Enemy_Spawning_Rate = 0;
                    }

                    _Enemy_Spawning_Rate += 1 + (Lvl / 10);
                }
                await Task.Delay(10);
            }
        }

        public ImageBrush LoadImg(string file)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri($"../../../Img/{file}", UriKind.Relative));
            return imageBrush;
        }

        public void StopGame()
        {
            if (Player.Hp > 0)
                textStopCanvas.Text = "Pause";
            else
                textStopCanvas.Text = "Game Over";

            if (Stop_Canvas.Visibility == Visibility.Visible)
                Stop_Canvas.Visibility = Visibility.Hidden;
            else
                Stop_Canvas.Visibility = Visibility.Visible;
        }
        public void UpdatePoints()
        {
            scoreText.Text = Score.ToString();
            coinsText.Text = Coins.ToString();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            mainMenu.Background = LoadImg("rambo.jpeg");
            gameCanvas.Background = LoadImg("Map.png");
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Player.Hp > 0)
                StopGame();
            else if (e.Key == Key.E && Player.Hp > 0 && _Bar_Ready)
            {
                nextLvlBar.Value = 0;
                Lvl += 1;
            }
            else
                Player.PlayerInput(e);
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            Player.PlayerInput(e, true);
        }

        private void WinMouseMove(object sender, MouseEventArgs e)
        {
            if (Player.Hp > 0)
            {
                Point cursorPosition = e.GetPosition(gameCanvas);
                Player.MoveAngle(cursorPosition.X - 15, cursorPosition.Y - 10);
            }
        }

        private void WinMouseClick(object sender, MouseEventArgs e)
        {
            if (Player.Hp > 0 && Stop_Canvas.Visibility != Visibility.Visible && Game_Canvas.Visibility == Visibility.Visible)
                    Player.Player_Gun.Shoot();
        }

        private void StartGameClick(object sender, RoutedEventArgs e)
        {
            Menu_Canvas.Visibility = Visibility.Hidden;
            _ = GameStart();
        }

        private void LeaveGame(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {

        }

        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            Player.Hp = 0;
            await Task.Delay(100);
            Game_Canvas.Children.Remove(Player.Player_Ellipse);
            foreach (Enemy enemy in All_Enemies)
            {
                enemy.Allive = false;
                Game_Canvas.Children.Remove(enemy.Enemy_Ellipse);
            }

            Coins = 0;
            Score = 0;
            Lvl = 1;
            UpdatePoints();
            nextLvlBar.Value = 0;
            Player = new Player_Info(3, 2);
            All_Enemies = new List<Enemy>();
            Stop_Canvas.Visibility = Visibility.Hidden;
            _ = GameStart();
        }


        private void UpdateStatusBar(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (nextLvlBar.Value >= 100)
            {
                statusBarReady.Visibility = Visibility.Visible;
                _Bar_Ready = true;
            }
            else
            {
                statusBarReady.Visibility = Visibility.Hidden;
                _Bar_Ready = false;
            }
        }

        private void StatsBuy(object sender, RoutedEventArgs e)
        {

        }

        private void GunStatsBuy(object sender, RoutedEventArgs e)
        {

        }

    }
}