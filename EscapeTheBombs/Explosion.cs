using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    public class Explosion : Player
    {
        public Explosion(int x, int y, string imagePath) : base(x, y, "/Images/explosion.png")
        {
        }
    }
}
