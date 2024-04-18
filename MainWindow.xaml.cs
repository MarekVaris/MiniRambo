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
using static System.Formats.Asn1.AsnWriter;
using System.IO;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Xml;

namespace MiniRambo
{
    public partial class MainWindow : Window
    {
        public int Lvl { get; set; } = 1;
        public int Current_Score { get; set; } = 0;
        public int Coins { get; set; } = 0;
        public Shop Shop_Game { get; set; }

        public Canvas Game_Canvas { get; set; }
        public Canvas Menu_Canvas { get; set; }
        public Canvas Stop_Canvas {  get; set; }
        public Canvas Shop_Canvas { get; set; }
        public List<Enemy> All_Enemies { get; set; }
        public Player_Info Player {  get; set; }
        public static MainWindow? Instance { get; private set; }
        private bool _Bar_Ready { get; set; } = false;
        private string _Player_Name { get; set; } = "Player001";
        

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            Game_Canvas = gameCanvas;
            Menu_Canvas = mainMenu;
            Stop_Canvas = stopCanvas;
            Shop_Canvas = shopCanvas;

            Shop_Game = new Shop();
            Player = new Player_Info(3, 2);
            All_Enemies = new List<Enemy>();
        }


        private async Task GameStart()
        {
            lvlText.Text = Lvl.ToString();
            Player.Player_Ellipse.Opacity = 1;
            Game_Canvas.Visibility = Visibility.Visible;

            int enemySpawningRate = 0;
            while (Player.Hp > 0 && Shop_Canvas.Visibility != Visibility.Visible)
            {
                if (Stop_Canvas.Visibility == Visibility.Hidden)
                {
                    Player.PlayerMove();
                    if (Player.Player_Gun.Ready_To_Shoot < 50 && MainWindow.Instance != null)
                        Player.Player_Gun.Ready_To_Shoot += Player.A_Speed + (MainWindow.Instance.Shop_Game.Main_Shop[3] /2);
                    if (Player.Proj_Spread > 0)
                        Player.Proj_Spread -= 0.1;
                    else
                        Player.Proj_Spread = 0;


                    if (enemySpawningRate > 100)
                    {
                        All_Enemies.Add(new Enemy());
                        enemySpawningRate = 0;
                    }

                    enemySpawningRate += 1 + (Lvl / 5);
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
            {
                textStopCanvas.Text = "Game Over";
                AddNewScore();
            }

            if (Stop_Canvas.Visibility == Visibility.Visible)
                Stop_Canvas.Visibility = Visibility.Hidden;
            else
                Stop_Canvas.Visibility = Visibility.Visible;
        }
        public void UpdatePoints()
        {
            scoreText.Text = Current_Score.ToString();
            coinsText.Text = Coins.ToString();
        }



        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            mainMenu.Background = LoadImg("rambo.jpeg");
            gameCanvas.Background = LoadImg("Map.png");

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
            if (Player.Hp > 0 
                && Stop_Canvas.Visibility != Visibility.Visible 
                && Game_Canvas.Visibility == Visibility.Visible 
                && Shop_Canvas.Visibility != Visibility.Visible)
                    Player.Player_Gun.Shoot();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Player.Hp > 0)
                StopGame();
            else if (e.Key == Key.E && Player.Hp > 0 && _Bar_Ready)
            {
                ClearCanvas();
                Shop_Canvas.Visibility = Visibility.Visible;
                nextLvlBar.Value = 0;
                Lvl += 1;
                lvlText.Text = Lvl.ToString();
                gameCanvas.Background = LoadImg("Shop.png");
            }
            else
                Player.PlayerInput(e);
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            Player.PlayerInput(e, true);
        }

        private void ClearCanvas()
        {
            Game_Canvas.Children.Remove(Player.Player_Ellipse);
            foreach (Enemy enemy in All_Enemies)
            {
                enemy.Allive = false;
                Game_Canvas.Children.Remove(enemy.Enemy_Ellipse);
            }
            All_Enemies = new List<Enemy>();
        }

        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            Player.Hp = 0;
            await Task.Delay(100);
            ClearCanvas();
            Shop_Game.Main_Shop = [0, 0, 0, 0, 0, 0];
            Coins = 0;
            Current_Score = 0;
            Lvl = 1;
            lvlText.Text = Lvl.ToString();
            nextLvlBar.Value = 0;
            UpdatePoints();
            Player = new Player_Info(3, 2);
            Stop_Canvas.Visibility = Visibility.Hidden;
            _ = GameStart();
        }
        private void StartNextRound(object sender, RoutedEventArgs e)
        {
            gameCanvas.Background = LoadImg("Map.png");
            Shop_Canvas.Visibility = Visibility.Hidden;
            Player = new Player_Info(Player.Hp + Shop_Game.Main_Shop[0], 2 + Shop_Game.Main_Shop[2] * 0.2);
            _ = GameStart();
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
            if (sender is Button button)
            {
                string? parameter = button.CommandParameter as string;
                if (parameter != null) 
                    Shop_Game.UpgradeStat(parameter);
            }
            
        }
        private void LeaderBoard(object sender, RoutedEventArgs e)
        {
            string filePath = "../../../Scores.xml";
            if (File.Exists(filePath))
            {
                XDocument doc = XDocument.Load(filePath);

                List<Score_Item> scores = new List<Score_Item>();

                if(doc.Root != null)
                    foreach (var scoreElement in doc.Root.Elements("Score"))
                    {
                        if(scoreElement != null)
                        {
                            string playerName = scoreElement.Element("PlayerName")?.Value ?? "Unknown";
                            int scoreValue = int.Parse(scoreElement.Element("ScoreValue")?.Value ?? "Unknown");

                            scores.Add(new Score_Item
                            {
                                PlayerName = playerName,
                                Score = scoreValue
                            });
                        }

                    }
                scores = scores.OrderByDescending(score => score.Score).ToList();
                for (int i = 0; i < scores.Count; i++)
                {
                    scores[i].Rank = i + 1;
                }
                scoreListView.ItemsSource = scores;
            }
            scoreCanvas.Visibility = Visibility.Visible;

        }

        private void AddNewScore()
        {
            string filePath = $"../../../Scores.xml";

            if (!File.Exists(filePath))
            {
                XDocument xmlDoc = new XDocument(new XElement("Scores"));
                xmlDoc.Save(filePath);
            }
            XDocument doc = XDocument.Load(filePath);
            XElement newScore = new XElement("Score",
                                new XElement("PlayerName", _Player_Name),
                                new XElement("ScoreValue", Current_Score));

            doc.Root?.Add(newScore);
            doc.Save(filePath);
        }

        private void CloseLeaderboard(object sender, RoutedEventArgs e)
        {
            scoreCanvas.Visibility = Visibility.Hidden;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            _Player_Name = textBox.Text;
        }
    }
}