using Dogfight_Arena.Services;
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
        protected double _speed;
        protected double _accelerationX;
        protected double _acceleration;
        protected double _accelerationY;
        protected double _angle;    

        protected GameMovingObject(double x, double y, string fileName, Canvas field, double size) : base(x, y, fileName, field, size)
        {
            Stop();
        }
        public override void Render()
        {
            // Update position based on angle and speed
            _x += _speed * Math.Cos(_angle);
            _y += _speed * Math.Sin(_angle);

            // Apply acceleration
            _speed += _acceleration;

            // Clamp speed
            if (_speed < Constants.MinSpeed)
                _speed = Constants.MinSpeed;
            if (_speed > Constants.MaxSpeed)
                _speed = Constants.MaxSpeed;

            // Optional: clamp acceleration
            if (_acceleration > Constants.MaxAcceleration)
                _acceleration = Constants.MaxAcceleration;
            if (_acceleration < Constants.MinAcceleration)
                _acceleration = Constants.MinAcceleration;

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
