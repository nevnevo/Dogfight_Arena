using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public class LeftPlane : Plane
    {
        public LeftPlane(double x, double y, string Image, Canvas field, int size) : base(x, y, Image, field, size)
        {
            PlaneType = PlaneTypes.LeftPlane;
            _planeTypeConstant = 1;
        }
        protected override void Shoot(VirtualKey key)
        {
            if (key == Keys.ShootBulletLeftPlayer && BulletsLeft > 0)
            {
                BulletsLeft--;
                ShootBullet();
                
            }
             if (key == Keys.ShootMissileLeftPlayer && MissileLeft>0 && canShootMissile)
            {
                MissileLeft--;
                ShootMissile();

            }
            
        }

       

        protected override void Move(VirtualKey key)
        {
            if (key == Keys.AccelerateLeftPlayer)
                base.Accelerate();
            if (key == Keys.DeccelerateleftPlayer)
                base.Decelerate();
            if (key == Keys.RotateClockWiseLeftPlayer)
                base.Rotate(-1);
            if (key == Keys.RotateCounterClockWiseLeftPlayer)
                base.Rotate(1);
        }
        

    }
}
