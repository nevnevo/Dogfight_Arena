using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public abstract class GameMovingObject : GameObject
    {
        protected double _speedX;
        protected double _speedY;
        protected double _accelerationX;
        protected double _accelerationY;

        protected GameMovingObject(double x, double y, string fileName, Canvas field, double size) : base(x, y, fileName, field, size)
        {
            Stop();
        }
        public override void Render()
        {
            _x += _speedX;
            _y += _speedY;
            _speedX += _accelerationX;
            _speedY += _accelerationY;

            base.Render();
        }
        private void Stop()
        {
            _speedX = 0;
            _speedY = 0;
            _accelerationX = 0;
            _accelerationY = 0;

        }
    }
}
