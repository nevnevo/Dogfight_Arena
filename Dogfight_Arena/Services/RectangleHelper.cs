using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Dogfight_Arena.Services
{
    public static class RectangleHelper
    {
        /// <summary>
        /// הפעולה מציירת מלבן במקום ובגודל ובצבע שקובעים
        /// </summary>
        /// <param name="scene">במה</param>
        /// <param name="x">מקום אופקי של הפינה העליונה השמאלית של המלבן</param>
        /// <param name="y">מיקום אנכי של הפינה השמאלית העליונה של המלבן</param>
        /// <param name="width">רוחב המלבן</param>
        /// <param name="height">גובה המלבן</param>
        /// <param name="color">צבע המלבן</param>
        public static void DrawRectangle(Canvas scene, double x, double y, double width, double height, Color color)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = (int)width,
                Height = (int)height,
                Fill = new SolidColorBrush(color),
            };
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);

            var rect = scene.Children.FirstOrDefault(r => r is Rectangle &&
                                        Math.Abs(Canvas.GetLeft(r) - x) < 50 && Math.Abs(Canvas.GetTop(r) - y) < 50);
            if (rect != null)
            {
                scene.Children.Remove(rect);
            }
            //scene.Children.RemoveAt(scene.Children.Count - 1);

            scene.Children.Add(rectangle);
        }
    }
}
