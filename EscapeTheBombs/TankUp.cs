using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable] public class TankUp : Tank
    {
        public TankUp(int x, int y, string imagePath) : base(x, y, "/Images/tankUp.png")
        {
        }
    }
}
