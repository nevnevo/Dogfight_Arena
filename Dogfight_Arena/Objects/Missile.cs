using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Services;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Dogfight_Arena.Objects
{
    public class Missile : Projectile
    {
        private double ttl = Constants.ProjectileTimeToLive;
        private Plane _enemyPlane;
       
        public Missile(double x, double y, string fileName, Canvas field, double size, double angle, Plane.PlaneTypes ShootingPlayer,Plane enemyPlane) : base(x, y, fileName, field, size, angle, ShootingPlayer)
        {
            _enemyPlane = enemyPlane;
            _objectImage.Height = _objectImage.Width * 0.245;
        }
        public override void Render()
        {
            if (ttl < 0)
            {
                Remove();
                if (GameManager.GameEvents.OnDelete != null)
                    GameManager.GameEvents.OnDelete(this);
                return;
            }


            ttl -= 1;
            _speed = Constants.BulletSpeed;

            if (_ShootingPlayer == Plane.PlaneTypes.RightPlane)
                _planeTypeConstant = -1;
            else
                _planeTypeConstant = 1;
            GotoEnemy();
            base.Render();
        }
        public void GotoEnemy()
        {
            if (_enemyPlane != null)
            {
                Rect enemyRect = _enemyPlane.Rect();

                double dx = enemyRect.X - _x;
                double dy = enemyRect.Y - _y;

                double targetAngle = Math.Atan2(dy, dx) * 180 / Math.PI;

                double currentAngle = ((_angle % 360) + 360) % 360;
                if (currentAngle > 180) currentAngle -= 360;

                double angleDiff = targetAngle - currentAngle;
                angleDiff = ((angleDiff + 180) % 360 + 360) % 360 - 180;

                if (angleDiff > 0)
                    Rotate(1);   // CCW
                else
                    Rotate(-1);    // CW
            }
        }

        public void Rotate(int SpinDirection)
        {
            var rotateTransform = new RotateTransform
            {
                Angle = _angle,
                CenterX = _objectImage.ActualWidth / 2,
                CenterY = _objectImage.ActualHeight / 2
            };

            _objectImage.RenderTransform = rotateTransform;

            if (SpinDirection > 0)
            {
                // CLOCKWISE → angle increases
                _angle += 4;
            }
            else
            {
                // COUNTER-CLOCKWISE → angle decreases
                _angle -= 4;
            }

            rotateTransform.Angle = _angle;
        }
        public override Rect Rect()
        {
            Rect rect = Plane.RotateRectAroundCenter2(base.Rect(), _angle);
            RectangleHelper.DrawTankRectangle(_field, rect.X, rect.Y, _objectImage.Width, _objectImage.Height, Colors.Black, (int)_angle);
            return rect;
        }


    }
}
