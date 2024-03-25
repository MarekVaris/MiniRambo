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
        public Canvas Main_Window { get; set; }
        private Player_Info Player;

        public MainWindow()
        {
            InitializeComponent();
            Main_Window = gameCanvas;
            Player = new Player_Info(400, 300, Main_Window);
        }

        public async Task GameSpeed()
        {
            while (true)
            {
                await Task.Delay(10);
                Player.PlayerMove();
            }
        }
        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await GameSpeed();
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