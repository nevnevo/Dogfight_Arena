using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Dogfight_Arena.Objects
{
    public abstract class GameObject
    {
        public double _x;
        public double _y;
        protected Image _objectImage;
        protected Canvas _field;

        public bool Colisional { get; set; } = true;

        // Constructor
        public GameObject(double x, double y, string fileName, Canvas field, double size)
        {
            _x = x;
            _y = y;
            _objectImage = new Image
            {
                Width = size
            };
            _field = field;

            // Schedule UI initialization on main thread
            _ = InitializeAsync(fileName);
        }

        // Thread-safe initialization
        private async Task InitializeAsync(string fileName)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _objectImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
                    _field.Children.Add(_objectImage);
                    Canvas.SetLeft(_objectImage, _x);
                    Canvas.SetTop(_objectImage, _y);
                });
        }

        // Rect with angle (can override)
        public virtual Rect Rect(int angle)
        {
            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);
        }

        // Default Rect
        public virtual Rect Rect()
        {
            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);
        }

        // Thread-safe render
        public virtual async Task RenderAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Canvas.SetLeft(_objectImage, _x);
                    Canvas.SetTop(_objectImage, _y);
                });
        }

        // Fire-and-forget render
        public virtual void Render()
        {
            _ = RenderAsync();
        }

        // Thread-safe remove
        public virtual void Remove()
        {
            _ = RemoveAsync();
        }

        private async Task RemoveAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _field.Children.Remove(_objectImage);
                });
        }

        // Override for collision behavior
        public virtual void Collide(GameObject otherObject) { }
    }
}