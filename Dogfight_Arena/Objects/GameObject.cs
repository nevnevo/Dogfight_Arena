using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Dogfight_Arena.Objects
{
    public abstract class GameObject
    {
        protected double _x;
        protected double _y;
        protected Image _objectImage;
        protected Canvas _field;
        
        public bool Colisional { get; set; } = true;
        public GameObject(double x, double y, string fileName, Canvas field, double size)
        {
            _x = x;
            _y = y;
            _objectImage = new Image();
            _objectImage.Width = size; 
            _field = field;
            SetImage(fileName);
            _field.Children.Add(_objectImage);
            Render();
        }

        private void SetImage(string fileName)
        {
            _objectImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
        }
        public virtual void Render()
        {
            Canvas.SetLeft(_objectImage, _x);
            Canvas.SetTop(_objectImage, _y);
        }
    }
}
