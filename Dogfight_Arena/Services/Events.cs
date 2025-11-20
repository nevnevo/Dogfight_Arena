using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using static Dogfight_Arena.Objects.Plane;

namespace Dogfight_Arena.Services
{
    public class Events
    {
        public Action<VirtualKey> OnKeyPress;
        public Action<VirtualKey> OnKeyLeave;
        public Action<Projectile> OnShoot;
        public Action<Projectile> OnProjectileDelete;
        public Action<PlaneTypes> TakeHit;
    }
}
