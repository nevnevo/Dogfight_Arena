using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public class RightPlane : Plane
    {
        public RightPlane(double x, double y, string Image, Canvas field, int size) : base(x, y, Image, field, size)
        {
            PlaneType = PlaneTypes.RightPlane;
            _planeTypeConstant = -1;
        }
        public override Rect Rect()
        {
            Rect rct = base.Rect();
            
            return base.Rect();
        }
        protected override void Shoot(VirtualKey key)
        {
            if (key == Keys.ShootBulletRightPlayer && BulletsLeft > 0)
            {
                BulletsLeft--;
                ShootBullet();

            }
            else if (key == Keys.ShootMissileRightPlayer && MissileLeft > 0 && canShootMissile)
            {
                MissileLeft--;
                ShootMissile();

            }

        }
        protected override void Move(VirtualKey key)
        {
            if (key == Keys.AccelerateRightPlayer)
                base.Accelerate();
            if (key == Keys.DeccelerateRightPlayer)
                base.Decelerate();
            if (key == Keys.RotateClockWiseLRightPlayer)
                base.Rotate(-1);
            if (key == Keys.RotateCounterClockWiseRightPlayer)
                base.Rotate(1);
        }

    }
}

