using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Services;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Dogfight_Arena.Objects
{
    public class Plane : GameMovingObject
    {
        public Plane(double x, double y,string Image, Canvas field,int size) : base(x, y,Image, field, size)
        {
            GameManager.GameEvents.OnKeyPress += Move;
            _speed = Constants.MinSpeed;
        }
        protected virtual void Move(VirtualKey key)
        {

        }
        protected void Accelerate()
        {
            if (_speed < Constants.MaxSpeed)
            {
                _acceleration += 0.3;
            }
        }

        protected void Decelerate()
        {
            if (_speed > Constants.MinSpeed)
            {
                _acceleration -= 0.3;
            }
        }

        protected void Turn(int direction)//direction>0 clockwise || direction<0 counter clockwise
        {
            if (direction > 0)
                _angle = Constants.RotationConstant * Math.PI * 180;
            else
                _angle = (Constants.RotationConstant * Math.PI * 180);
            Rotate();
        }
        private (double, double) CalculateCenterPoint()
        {
            return (
                _x + _objectImage.Width / 2,
                _y + _objectImage.Height / 2
            );
        }
        private void Rotate()
        {
            double centerX, centerY;
            (centerX, centerY) = CalculateCenterPoint();

            var rotation = new RotateTransform
            {
                Angle = _angle, // already in degrees now
                CenterX = centerX,
                CenterY = centerY
            };

            _objectImage.RenderTransform = rotation;
            base.Render();
        }

    }
}
