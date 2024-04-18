using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MiniRambo
{
    public class Shop
    {
        public List<int> Main_Shop { get; set; } = [0, 0, 0, 0, 0, 0];

        public Shop()
        {

        }

        public void UpgradeStat(string stat)
        {
            if (MainWindow.Instance != null)
            {
                switch (stat)
                {
                    case "Hp":
                        SetStat("HP:+", MainWindow.Instance.shopHp, 0);
                        MainWindow.Instance.Player.UpdateHealthUi(Main_Shop[0]);
                        break;
                    case "Dmg":
                        SetStat("DMG:+", MainWindow.Instance.shopDmg, 1);
                        break;
                    case "MSpd":
                        SetStat("MSPD:+", MainWindow.Instance.shopMSpd, 2);
                        break;
                    case "ASpd":
                        SetStat("ASPD:+", MainWindow.Instance.shopASpd, 3);
                        break;
                    case "Ammo":
                        SetStat("AMMO:+", MainWindow.Instance.shopAmmo, 4);
                        MainWindow.Instance.ammoText.Text = $"{12 + Main_Shop[4]}/{12 + Main_Shop[4]}";
                        break;
                    case "Spread":
                        SetStat("SPREAD:+", MainWindow.Instance.shopSpread, 5);
                        break;
                }
            }
            else
                throw new InvalidOperationException();

        }

        private async void SetStat(string textStat, StackPanel stat_SP, int shopIndex)
        {
            if (stat_SP.Children.Count > 2 && MainWindow.Instance != null)
            {
                if ((10 + (Main_Shop[shopIndex] * 5)) <= MainWindow.Instance.Coins)
                {
                    TextBlock textName = (TextBlock)stat_SP.Children[0];
                    TextBlock textPrice = (TextBlock)stat_SP.Children[1];

                    MainWindow.Instance.Coins -= 10 + (Main_Shop[shopIndex] * 5);

                    Main_Shop[shopIndex]++;
                    textName.Text = $"{textStat}{Main_Shop[shopIndex]}";
                    textPrice.Text = $"{10 + (Main_Shop[shopIndex] * 5)}";

                    MainWindow.Instance.UpdatePoints();
                }
                else
                {
                    Button currentButton = (Button)stat_SP.Children[2];
                    currentButton.IsEnabled = false;
                    for (int i = 0; i < 2; i++)
                    {
                        currentButton.Opacity = 0.3;
                        await Task.Delay(250);

                        currentButton.Opacity = 1;
                        await Task.Delay(250);
                    }
                    currentButton.IsEnabled = true;
                }
            }
        }
    }
}
