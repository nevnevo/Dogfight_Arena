using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Services;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public class Plane : GameMovingObject
    {
        public Plane(double x, double y,string Image, Canvas field,int size) : base(x, y,Image, field, size)
        {

        }
        private void Accelerate()
        {
            if (_accelerationX < Constants.MaxAcceleration)
                _accelerationX += 0.3;
            if (_accelerationY < Constants.MaxAcceleration)
            {
                _accelerationY += 0.3;
            }
        }
        private void Decelerate()
        {
            if (_accelerationX > Constants.MinAcceleration)
                _accelerationX -= 0.3;
            if (_accelerationY > Constants.MinAcceleration)
            {
                _accelerationY -= 0.3;
            }
        }
    }
}
