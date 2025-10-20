
using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Services
{
    public class Manager
    {
        public static bool IsOnline { get; set; }
        public List<GObject> _ObjectsList = new List<GObject>();
        private DispatcherTimer _runTimer;
        private HashSet<VirtualKey> ActiveKeys = new HashSet<VirtualKey>(); //Hashset is used to avoid duplicatekeys,because it can contain only unique values
        //private Plane LocalPlayer;
        public static Events GameEvents { get; private set; } = new Events();
        public GameManager(Canvas field) 
        {
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(1);
            _runTimer.Start();
            _runTimer.Tick += runTimer_Tick;
            LocalPlayer = new LeftPlane(100, 100, "Images/LeftPlayer.png", field, 200);
            _ObjectsList.Add(LocalPlayer);

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            GameEvents.OnShoot += OnShoot;
            GameEvents.OnProjectileDelete += DeleteProjectile;
        }

        private void DeleteProjectile(Projectile projectile)
        {
            if(projectile is Bullet bullet)
            {
                _ObjectsList.Remove(bullet);
            }
        }

        private void OnShoot(Projectile projectile)
        {
            if(projectile is Bullet bullet)
            {
                    _ObjectsList.Add(bullet);
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (GameEvents.OnKeyPress != null)
            {
                ActiveKeys.Remove(args.VirtualKey);
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (GameEvents.OnKeyPress != null)
            {
                ActiveKeys.Add(args.VirtualKey);
                
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
            foreach (VirtualKey key in ActiveKeys)
            {
                GameEvents.OnKeyPress(key);
            }
        }
    }
}
