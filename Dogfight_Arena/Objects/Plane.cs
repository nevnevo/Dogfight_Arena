using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dogfight_Arena.Services;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace Dogfight_Arena.Objects
{
    public class Plane : GameMovingObject
    {
        protected PlaneTypes PlaneType;
        protected int HealthPoints = Constants.StartingHealthPoints;
        public int BulletsLeft = 5;
        

        public enum PlaneTypes
        {
            LeftPlane,RightPlane
        }
        public Plane(double x, double y,string Image, Canvas field,int size) : base(x, y,Image, field, size)
        {
            GameManager.GameEvents.OnKeyPress += Move;
            GameManager.GameEvents.OnKeyLeave += Shoot;
            _speed = Constants.MinSpeed;
            _objectImage.Height = _objectImage.Width * 0.932;
        }

        protected virtual void Shoot(VirtualKey key)
        {
            
        }
        public void UnsubscribeEvents()
        {
            GameManager.GameEvents.OnKeyPress -= Move;
            GameManager.GameEvents.OnKeyLeave -= Shoot;
        }

        protected virtual void Move(VirtualKey key)
        {

        }
        protected virtual void ShootBullet()
        {
            var (centerX, centerY) = CalculateCenterPointProjectile();
            var projectile = new Bullet(centerX, centerY, "Images/Bullet.png", _field, 5, _angle, PlaneType);
            if(GameManager.GameEvents.OnShoot != null)
                GameManager.GameEvents.OnShoot(projectile);
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
        public override Rect Rect()
        {
            
            return RotateRectAroundCenter(base.Rect(), this._angle);
        }


        protected void Rotate(int SpinDirection)// SpinDirection with the clock gotta be a num>0 else num<0
        { 
            
            if (SpinDirection > 0)
            {
                var rotateTransform = new RotateTransform
                {
                    Angle = _angle, // The rotation angle in degrees (set initial angle to 0)
                    CenterX = _objectImage.ActualWidth / 2, // Rotate around the center of the image
                    CenterY = _objectImage.ActualHeight / 2 // Rotate around the center of the image
                };

        // Apply the RotateTransform to the RenderTransform of the image
        _objectImage.RenderTransform = rotateTransform;

                // Update the rotation on every update (e.g., when button is clicked, etc.)
                _angle -= 3; // Increase the angle for continuous rotation
                rotateTransform.Angle = _angle;
               
                

            }
            else
            {


                // Create a RotateTransform to apply rotation
                var rotateTransform = new RotateTransform
                {
                    Angle = _angle, // The rotation angle in degrees (set initial angle to 0)
                    CenterX = _objectImage.ActualWidth / 2, // Rotate around the center of the image
                    CenterY = _objectImage.ActualHeight / 2 // Rotate around the center of the image
                };

    // Apply the RotateTransform to the RenderTransform of the image
    _objectImage.RenderTransform = rotateTransform;

                // Update the rotation on every update (e.g., when button is clicked, etc.)
                _angle += 3; // Increase the angle for continuous rotation
                rotateTransform.Angle = _angle;
                

            }
        }

        
        private (double, double) CalculateCenterPointProjectile()
        {
            return (
                _x + _objectImage.Width / 2,
                _y + _objectImage.Width / 2
            );
        }
        private static Vector2 RotatePointsAroundAxis(Vector2 point, double centerX, double centerY, double angleInRadians)
        {
            float translatedX = point.X - (float)centerX;
            float translatedY = point.Y - (float)centerY;

            // Rotate the point using the 2D rotation matrix
            float rotatedX = (float)(translatedX * Math.Cos(angleInRadians) - translatedY * Math.Sin(angleInRadians));
            float rotatedY = (float)(translatedX * Math.Sin(angleInRadians) + translatedY * Math.Cos(angleInRadians));

            // Translate back to the original position
            return new Vector2(rotatedX + (float)centerX, rotatedY + (float)centerY);


        }
        public static Rect RotateRectAroundCenter(Rect rect, double angleInDegrees)
        {
            // Convert angle from degrees to radians
            double angleInRadians = angleInDegrees * (Math.PI / 180);

            // Get the center of the rectangle
            double centerX = rect.X + rect.Width / 2;
            double centerY = rect.Y + rect.Height / 2;

            // Define the four corners of the rectangle
            var topLeft = new Vector2((float)rect.X, (float)rect.Y);
            var topRight = new Vector2((float)(rect.X + rect.Width), (float)rect.Y);
            var bottomLeft = new Vector2((float)rect.X, (float)(rect.Y + rect.Height));
            var bottomRight = new Vector2((float)(rect.X + rect.Width), (float)(rect.Y + rect.Height));

            // Rotate each corner around the center
            topLeft = RotatePointsAroundAxis(topLeft, centerX, centerY, angleInRadians);
            topRight = RotatePointsAroundAxis(topRight, centerX, centerY, angleInRadians);
            bottomLeft = RotatePointsAroundAxis(bottomLeft, centerX, centerY, angleInRadians);
            bottomRight = RotatePointsAroundAxis(bottomRight, centerX, centerY, angleInRadians);

            // Now we need to compute the bounding box that contains all the rotated corners
            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X))+17;
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y))+17;
            float maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X))-17;
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y))-17;

            // Return the new Rect that fits the rotated corners
            
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }


        public override void Collide(GameObject otherObject)
        {
            if(otherObject is Projectile projectile && projectile._ShootingPlayer!= PlaneType)
            {
                GameManager.GameEvents.OnProjectileDelete(projectile);
                projectile.Remove();

                if (GameManager.GameEvents.TakeHit != null)
                {
                    GameManager.GameEvents.TakeHit(PlaneType);
                }

            }
        }
    }
}
