using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable]
    public class TankDown : Tank
    {
        public TankDown(int x, int y, string imagePath) : base(x, y, "/Images/tankDown.png")
        {
        }
    }
}
