using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniRambo
{
    class Enemy
    {
        private Canvas Main_Canvas;
        private Ellipse CurrentEnemy { get; set; }

        public Enemy(Canvas mainCanvas)
        {
            Main_Canvas = mainCanvas;
            CurrentEnemy = CreateEnemy();
        }


        private Ellipse CreateEnemy()
        {
            Ellipse enemyEllipse = new Ellipse();
            enemyEllipse.Width = 20;
            enemyEllipse.Height = 20;
            enemyEllipse.Stroke = Brushes.Black;
            enemyEllipse.Fill = Brushes.Red;

            Random random = new Random();

            

            Canvas.SetLeft(enemyEllipse, Main_Canvas.Width - 20);
            Canvas.SetTop(enemyEllipse, 0);

            Main_Canvas.Children.Add(enemyEllipse);

            return enemyEllipse;
        }
    }

}
