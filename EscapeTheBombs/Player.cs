using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EscapeTheBombs
{
    [Serializable]
    public class Player
    {
        public int _x;
        public int _y;
        public string _imagePath;
        public int canvasWidth = 1200;
        public int canvasHeight = 650;
        public int speed;
        public Direction direction;

        [NonSerialized] public Image PlayerImage;

        public Player(int x, int y, string imagePath)
        {
            _x = x;
            _y = y;
            _imagePath = imagePath;

            PlayerImage = new Image();
            Uri uriSource = new Uri(imagePath, UriKind.Relative);
            PlayerImage.Source = new BitmapImage(uriSource);
            PlayerImage.Width = 50;
            PlayerImage.Height = 50;
        }

        public virtual int Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    _x -= 5;
                    CheckWhetherWithinTheCanvas();
                    return _x;
                case Direction.Right:
                    _x += 5;
                    CheckWhetherWithinTheCanvas();
                    return _x;
                case Direction.Up:
                    _y -= 5;
                    CheckWhetherWithinTheCanvas();
                    return _y;
                case Direction.Down:
                    _y += 5;
                    CheckWhetherWithinTheCanvas();
                    return _y;
                default:
                    return 0;

            }
        }

        public void CheckWhetherWithinTheCanvas()
        {
            if (_x < 5)
            {
                _x = 5;
            }
            if (_x > canvasWidth - (int)PlayerImage.Width)
            {
                _x = canvasWidth - (int)PlayerImage.Width;
            }
            if (_y < 5)
            {
                _y = 5;
            }
            if (_y > canvasHeight - (int)PlayerImage.Height)
            {
                _y = canvasHeight - (int)PlayerImage.Height;
            }
        }

        public virtual int Speed() => 0;


        public virtual int Escape(char coordinate) => 0;

    }
}
