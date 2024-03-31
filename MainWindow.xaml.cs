using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace MiniRambo
{
    public partial class MainWindow : Window
    {
        public int Enemy_Spawning_Rate = 0;

        public Canvas Main_Canvas { get; set; }
        public List<Enemy> AllEnemies { get; set; }
        public Player_Info Player;
        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            Main_Canvas = gameCanvas;

            Player = new Player_Info();
            AllEnemies = new List<Enemy>();
        }

        public async Task GameStart()
        {
            while (Player.Hp > 0)
            {
                Player.PlayerMove();

                if (Enemy_Spawning_Rate > 100)
                {
                    AllEnemies.Add(new Enemy());
                    Enemy_Spawning_Rate = 0;
                }

                Enemy_Spawning_Rate++;
                await Task.Delay(10);
            }
        }
        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await GameStart();
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
            Player.Shoot();
        }

    }
}