using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiniRambo
{
    public class Gun
    {
        public double Parent_Top;
        public double Parent_Left;
        public bool Reloading = false;

        private int Reload_Speed { get; set; }
        private int Max_Ammo { get; set; }
        private int Current_Ammo { get; set; }
        private TextBlock? Ammo_Text { get; set; }

        public Gun(int maxAmmo, int reloadSpeed, bool ui = false)
        {
            Max_Ammo = maxAmmo;
            Current_Ammo = Max_Ammo;
            Reload_Speed = reloadSpeed;
            if (ui)
                if (MainWindow.Instance != null &&
                    MainWindow.Instance.ammoText != null &&
                    MainWindow.Instance.healthText != null)
                {
                    Ammo_Text = MainWindow.Instance.ammoText;
                    Ammo_Text.Text = $"{Max_Ammo}/{Current_Ammo}";
                }
                else
                    throw new InvalidOperationException();
        }

        public async void Reload()
        {
            if (!Reloading) 
            { 
                Reloading = true;
                Current_Ammo = 0;
                await Task.Delay(Reload_Speed);
                Current_Ammo = Max_Ammo;
                if (Ammo_Text != null)
                    Ammo_Text.Text = $"{Max_Ammo}/{Current_Ammo}";
                Reloading = false;
            }
        }

        public void Shoot(double top, double left)
        {
            if (Current_Ammo > 0)
            {
                Parent_Top = top;
                Parent_Left = left;
                Current_Ammo--;
                if (Ammo_Text != null)
                    Ammo_Text.Text = $"{Max_Ammo}/{Current_Ammo}";
                _ = new Projectile(this, false);
            }
            else
                Reload();
        }

    }
}
