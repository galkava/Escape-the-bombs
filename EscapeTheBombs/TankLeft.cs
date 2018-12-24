using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable]
    public class TankLeft : Tank
    {
        public TankLeft(int x, int y, string imagePath) : base(x, y, "/Images/tankLeft.png")
        {
        }
    }
}
