using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using System;
using System.Threading.Tasks;

public class GameObject
{
    public double _x;
    public double _y;
    public Image _objectImage;
    private Canvas _field;

    public GameObject(double x, double y, string fileName, Canvas field, double size)
    {
        _x = x;
        _y = y;
        _field = field;

        // Schedule construction of _objectImage on the UI thread
        _ = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
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


private void SetImage(string fileName)
        {
            _objectImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
        }
        public virtual Rect Rect(int angle)
        {

            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);

        }
        public virtual Rect Rect()
        {
            
            return new Rect(_x, _y, _objectImage.Width - 15, _objectImage.Height - 15);

        }
        public virtual void Remove()
        {
            _field.Children.Remove(_objectImage);
        }
        public virtual async Task RenderAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication
                .MainView.CoreWindow.Dispatcher
                .RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Canvas.SetLeft(_objectImage, _x);
                    Canvas.SetTop(_objectImage, _y);
                });
        }
        public virtual void Render()
        {
            _ = RenderAsync();

        }
        public virtual void Collide(GameObject otherObject) { }
    }
}
