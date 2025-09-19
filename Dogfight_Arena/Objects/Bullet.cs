using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Dogfight_Arena.Objects
{
    public class Bullet : Projectile
    {
        public double ttl = Constants.ProjectileTimeToLive; 
        public Bullet(double x, double y, string fileName, Canvas field, double size, double angle, Plane.PlaneTypes ShootingPlayer) : base(x, y, fileName, field, size, angle, ShootingPlayer)
        {
        }
        public override void Render()
        {
            if (ttl < 0)
            {
                Remove();
                if (GameManager.GameEvents.OnProjectileDelete != null)
                    GameManager.GameEvents.OnProjectileDelete(this);
                return;
            }
                
            base.Render();
            ttl -= 1;
            _speed = Constants.BulletSpeed;
        }

    }

}
