using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniRambo
{
    public class Gun
    {
        public double Parent_Top;
        public double Parent_Left;
        public bool Reloading = false;

        private int _Reload_Speed { get; set; }
        private int _Max_Ammo { get; set; }
        private int _Current_Ammo { get; set; }
        private TextBlock? _Ammo_Text { get; set; }

        public Gun(int maxAmmo, int reloadSpeed, bool ui = false)
        {
            _Max_Ammo = maxAmmo;
            _Current_Ammo = _Max_Ammo;
            _Reload_Speed = reloadSpeed;
            if (ui)
            {
                if (MainWindow.Instance != null &&
                    MainWindow.Instance.ammoText != null)
                {
                    _Ammo_Text = MainWindow.Instance.ammoText;
                    _Ammo_Text.Text = $"{_Max_Ammo}/{_Current_Ammo}";
                }
                else
                    throw new InvalidOperationException();
            }
        }

        public async void Reload()
        {
            if (!Reloading) 
            { 
                Reloading = true;
                _Current_Ammo = 0;
                await Task.Delay(_Reload_Speed);
                _Current_Ammo = _Max_Ammo;
                if (_Ammo_Text != null)
                    _Ammo_Text.Text = $"{_Max_Ammo}/{_Current_Ammo}";
                Reloading = false;
            }
        }

        public void Shoot(double top, double left)
        {
            if (_Current_Ammo > 0)
            {
                Parent_Top = top;
                Parent_Left = left;
                _Current_Ammo--;
                if (_Ammo_Text != null)
                    _Ammo_Text.Text = $"{_Max_Ammo}/{_Current_Ammo}";
                _ = new Projectile(this, false);
            }
            else
                Reload();
        }

    }
}
