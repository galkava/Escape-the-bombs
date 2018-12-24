using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable]
    public class Tank : Player
    {
        Random randomEscape = new Random();

        public Tank(int x, int y, string imagePath) : base(x, y, imagePath)
        {
        }
        
        public override int Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    _x -= 10;
                    CheckWhetherWithinTheCanvas();
                    return _x;
                case Direction.Right:
                    _x += 10;
                    CheckWhetherWithinTheCanvas();
                    return _x;
                case Direction.Up:
                    _y -= 10;
                    CheckWhetherWithinTheCanvas();
                    return _y;
                case Direction.Down:
                    _y += 10;
                    CheckWhetherWithinTheCanvas();
                    return _y;
                default:
                    return 0;
            }
        }

        public override int Escape(char coordinate)
        {
            switch (coordinate)
            {
                case 'x':
                    _x = randomEscape.Next(-980, 980);
                    if (_x < 5)
                        _x = 5;
                    if (_x > 980 - 55)
                        _x = 980 - 55;
                    return _x;

                case 'y':
                    _y = randomEscape.Next(-550, 550);
                    if (_y < 5)
                        _y = 5;
                    if (_y > 550 - 55)
                        _y = 550 - 55;
                    return _y;

                default:
                    return 0;
            }
        }
    }
}
