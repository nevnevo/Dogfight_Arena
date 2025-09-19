using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Objects;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena
{
    
    public class Projectile : GameMovingObject
    {
        public Plane.PlaneTypes _ShootingPlayer;

        public Projectile(double x, double y, string fileName, Canvas field, double size,double angle,Plane.PlaneTypes ShootingPlayer): base(x, y, fileName, field, size)
        {
            _angle = angle;
            _ShootingPlayer = ShootingPlayer;
        }
        public void Remove()
        {
            _field.Children.Remove(_objectImage);
        }

    }
}
