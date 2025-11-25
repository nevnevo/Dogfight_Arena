using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dogfight_Arena.Objects;
using Windows.Foundation;
using Windows.UI.Xaml;
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
            this.Colisional = false;

        }
        public override Rect Rect()
        {

            return new Rect(_x, _y, _objectImage.Width, _objectImage.Height);

        }


    }
}
