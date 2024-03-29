using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniRambo
{
    public partial class MainWindow : Window
    {
        public int Enemy_Spawning_Rate = 0;

        public Canvas Main_Canvas { get; set; }
        private Player_Info Player;
        private List<Enemy> AllEnemies;

        public MainWindow()
        {
            InitializeComponent();
            Main_Canvas = gameCanvas;
            AllEnemies = new List<Enemy>();
            Player = new Player_Info(Main_Canvas);
        }

        public async Task GameStart()
        {
            while (true)
            {
                Player.PlayerMove();

                if(Enemy_Spawning_Rate > 100)
                {
                    AllEnemies.Add(new Enemy(Main_Canvas, Player));
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