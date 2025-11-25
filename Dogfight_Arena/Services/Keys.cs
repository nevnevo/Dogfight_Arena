using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Dogfight_Arena.Services
{
    public abstract class Keys
    {
        //LeftPlayer
        public static VirtualKey AccelerateLeftPlayer = VirtualKey.W;
        public static VirtualKey DeccelerateleftPlayer = VirtualKey.S;
        public static VirtualKey RotateClockWiseLeftPlayer = VirtualKey.D;
        public static VirtualKey RotateCounterClockWiseLeftPlayer = VirtualKey.A;

        //RightPlayer
        public static VirtualKey AccelerateRightPlayer = VirtualKey.Up;
        public static VirtualKey DeccelerateRightPlayer = VirtualKey.Down;
        public static VirtualKey RotateClockWiseLRightPlayer = VirtualKey.Right;
        public static VirtualKey RotateCounterClockWiseRightPlayer = VirtualKey.Left;

        public static VirtualKey ShootBulletLeftPlayer = VirtualKey.E;
        public static VirtualKey ShootBulletRightPlayer = VirtualKey.Space;

        public static VirtualKey ShootMissileLeftPlayer = VirtualKey.Q;
        public static VirtualKey ShootMissileRightPlayer = VirtualKey.M;

    }
}
