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


                scene.Children.Add(rectangle);

            }
            public static void DrawTankRectangle(Canvas scene, double x, double y, double width, double height, Color color, int angle)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = (int)width,
                    Height = (int)height,
                    Fill = new SolidColorBrush(color),




                };

                rectangle.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);//sets the rotaton center  point


                RotateTransform rotateTransform = new RotateTransform();//creates an instance of a rotate transform
                rotateTransform.Angle = angle; //sets the roatation angle acording to param

                rectangle.RenderTransform = rotateTransform;// sets the instance to render the transform



                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, y);

                var rect = scene.Children.FirstOrDefault(r => r is Rectangle &&
                                            Math.Abs(Canvas.GetLeft(r) - x) < 50 && Math.Abs(Canvas.GetTop(r) - y) < 50);
                if (rect != null)
                {
                    scene.Children.Remove(rect);
                }


                scene.Children.Add(rectangle);
            }
            public static void DeleteProjectile(Canvas scene, double x, double y, double width, double height)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = (int)width,
                    Height = (int)height,
                    Fill = new SolidColorBrush(Colors.White),

                };
                Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, y);

                var rect = scene.Children.FirstOrDefault(r => r is Rectangle &&
                                            Math.Abs(Canvas.GetLeft(r) - x) < 50 && Math.Abs(Canvas.GetTop(r) - y) < 50);
                if (rect != null)
                {
                    scene.Children.Remove(rect);
                }

                scene.Children.Remove(rectangle);

            }
            public static void DrawWall(Canvas scene, double x, double y, double width, double height, Color color)
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



                scene.Children.Add(rectangle);

            }
        
    }
}
