﻿
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelOthello
{
    internal class TileButton : Button
    {
        int owner,x,y;
        MainWindow game;
        bool isPlayable;

        public static SolidColorBrush[] BRUSHES = { Brushes.LightGray, Brushes.White, Brushes.Black };

        public int Owner
        {
            get { return owner; }
            set {
                if (value == 1)
                    B();
                else if (value == 0)
                    W();
            }
        }


        public bool IsPlayable
        {
            set {
                isPlayable = value;
                if (isPlayable)
                    Background = Brushes.Pink;
                else
                    Background = BRUSHES[owner+1];
            }
        }

        public TileButton(int x, int y, MainWindow parent)
        {
            this.x = x;
            this.y = y;
            this.game = parent;

            Name = $"Button{x}{y}";
            //IsEnabled = false;

            Grid.SetColumn(this, x);
            Grid.SetRow(this, y);

            owner = -1;
        }

        public void B()
        {
            owner = 1;
            Background = Brushes.Black;
        }

        public void W()
        {
            owner = 0;
            Background = Brushes.White;
        }
        
        protected override void OnClick()
        {
            if(owner == -1) 
                game.play(x,y);
        }
    }
}