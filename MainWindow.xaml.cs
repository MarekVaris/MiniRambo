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

namespace MiniRambo
{
    public partial class MainWindow : Window
    {
        public int Enemy_Spawning_Rate = 0;

        public Canvas Main_Canvas { get; set; }
        public List<Enemy> All_Enemies { get; set; }
        public Player_Info Player {  get; set; }
        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            Main_Canvas = gameCanvas;
            Player = new Player_Info();
            All_Enemies = new List<Enemy>();
        }


        private async Task GameStart()
        {
            Player.PlayerVisable(1);
            ammoText.Opacity = 0.5;
            while (Player.Hp >= 0)
            {
                Player.PlayerMove();

                if (Enemy_Spawning_Rate > 100)
                {
                    All_Enemies.Add(new Enemy());
                    Enemy_Spawning_Rate = 0;
                }

                Enemy_Spawning_Rate++;
                await Task.Delay(10);
            }
            ResetEnemyList();
            ammoText.Opacity = 0;
        }
        private void ResetEnemyList()
        {
            foreach (Enemy enemy in All_Enemies)
            {
                Main_Canvas.Children.Remove(enemy.Enemy_Ellipse);
            }
        }
        private ImageBrush LoadBacground(string file)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri($"../../../Img/{file}", UriKind.Relative));
            return imageBrush;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            mainMenu.Background = LoadBacground("rambo.jpeg");
            gameCanvas.Background = LoadBacground("Map.png");
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            Player.PlayerInput(e);
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            Player.PlayerInput(e, true);
        }

        private void WinMouseMove(object sender, MouseEventArgs e)
        {
            Point cursorPosition = e.GetPosition(gameCanvas);
            Player.MoveAngle(cursorPosition.X - 15, cursorPosition.Y - 10);
        }

        private void WinMouseClick(object sender, MouseEventArgs e)
        {
            Player.Player_Gun.Shoot(Player.X, Player.Y);
        }

        private async void StartGameClick(object sender, RoutedEventArgs e)
        {
            mainMenu.Visibility = Visibility.Hidden;
            await GameStart();
        }

        private void LeaveGame(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {

        }

    }
}