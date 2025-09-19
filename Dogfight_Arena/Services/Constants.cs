using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogfight_Arena.Services
{
    public static class Constants
    {
        public static readonly double MaxAcceleration = 1.5;//the maximum unreachable acceleration
        public static readonly double MinAcceleration= -1.5;//the minimum unreachable acceleration
        public static readonly double RotationConstant = 5;
        public static readonly double MinSpeed = 0.5;
        public static readonly double MaxSpeed = 5;

        public static double ProjectileTimeToLive = 10000;//time to live in milliseconds ==> 10 seconds

        public static double BulletSpeed = 50;
    }
}
