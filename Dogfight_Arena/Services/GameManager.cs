using Dogfight_Arena.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Services
{
    public class GameManager
    {
        public static bool IsOnline { get; set; }
        public List<GameObject> _ObjectsList = new List<GameObject>();
        private DispatcherTimer _runTimer;
        public static Events GameEvents { get; private set; } = new Events();
        public GameManager(Canvas field) 
        {
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(1);
            _runTimer.Start();
            _runTimer.Tick += runTimer_Tick;
            _ObjectsList.Add(new LeftPlane(100,100,"Images/LeftPlayer.png",field,200));

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (GameEvents.OnKeyPress != null)
            {
                GameEvents.OnKeyPress(args.VirtualKey);
            }
        }

        private void runTimer_Tick(object sender, object e)
        {
            foreach(GameObject obj in _ObjectsList)
            {
                if(obj is GameMovingObject MovingObj)
                {
                    MovingObj.Render();
                }
            }
        }
    }
}
