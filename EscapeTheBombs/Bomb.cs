using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeTheBombs
{
    [Serializable]
    public class Bomb : Player
    {
        Random randomSpeed = new Random();
        
        public Bomb(int x, int y, string imagePath) : base(x, y, "/Images/bomb.png")
        {
        }

        public override int Speed() => speed = 1;
    }
}