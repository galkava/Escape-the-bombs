using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable]
    public class TankRight : Tank
    {
        public TankRight(int x, int y, string imagePath) : base(x, y, "/Images/tankRight.png")
        {
        }
    }
}
