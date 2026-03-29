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

        /// <summary>
        /// Thread-safe constructor: all UI elements are created on the UI thread.
        /// </summary>
        public GameObject(double x, double y, string fileName, Canvas field, double size)
        {
            _x = x;
            _y = y;
            _field = field;

            // Schedule full UI initialization on UI thread
            _ = InitializeAsync(fileName, size);
        }

        /// <summary>
        /// Initialize Image and add to Canvas safely on UI thread.
        /// </summary>
        private async Task InitializeAsync(string fileName, double size)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Create Image on UI thread
                    _objectImage = new Image
                    {
                        Width = size,
                        Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"))
                    };

                    _field.Children.Add(_objectImage);
                    Canvas.SetLeft(_objectImage, _x);
                    Canvas.SetTop(_objectImage, _y);
                });
        }

        /// <summary>
        /// Get Rect with angle (can be overridden in subclasses)
        /// </summary>
        public virtual Rect Rect(int angle)
        {
            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);
        }

        /// <summary>
        /// Get Rect without angle
        /// </summary>
        public virtual Rect Rect()
        {
            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);
        }

        /// <summary>
        /// Thread-safe render: moves object to _x/_y on UI thread
        /// </summary>
        public virtual async Task RenderAsync()
        {
            if (_objectImage == null) return;

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Canvas.SetLeft(_objectImage, _x);
                    Canvas.SetTop(_objectImage, _y);
                });
        }

        /// <summary>
        /// Fire-and-forget render
        /// </summary>
        public virtual void Render()
        {
            _ = RenderAsync();
        }

        /// <summary>
        /// Thread-safe remove from canvas
        /// </summary>
        public virtual void Remove()
        {
            _ = RemoveAsync();
        }

        private async Task RemoveAsync()
        {
            if (_objectImage == null) return;

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _field.Children.Remove(_objectImage);
                });
        }

        /// <summary>
        /// Override for collision behavior
        /// </summary>
        public virtual void Collide(GameObject otherObject) { }
    }
}