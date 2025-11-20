using Dogfight_Arena.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
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
        private HashSet<VirtualKey> ActiveKeys = new HashSet<VirtualKey>(); //Hashset is used to avoid duplicatekeys,because it can contain only unique values
        private Plane LocalPlayer;
        private Plane SecondPlayer;
        private Canvas _field;
        public static Events GameEvents { get; private set; } = new Events();
        public GameManager(Canvas field) 
        {
            _field = field;
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(1);
            _runTimer.Start();
            _runTimer.Tick += runTimer_Tick;

            if (!IsOnline)
            {//If the player chose to play offline
                LocalPlayer = new LeftPlane(100, 100, "Images/LeftPlayer.png", field, 200);
                SecondPlayer = new RightPlane(field.ActualWidth - 100 - 200, 100, "Images/RightPlayer.png", field, 200);
                _ObjectsList.Add(LocalPlayer);
                _ObjectsList.Add(SecondPlayer);
            }
            

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            GameEvents.OnShoot += OnShoot;
            GameEvents.OnProjectileDelete += DeleteProjectile;
        }

        public void DeleteProjectile(Projectile projectile)
        {
            if(projectile is Bullet bullet)
            {
                _ObjectsList.Remove(bullet);
                bullet.Remove();
                
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
                if (args.VirtualKey == Keys.ShootBulletLeftPlayer)
                {
                    GameEvents.OnKeyLeave(args.VirtualKey);
                }
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

                if(obj is GameMovingObject)
                {
                    
                    obj.Render();
                }
            }
            CheckCollisional();
            foreach (VirtualKey key in ActiveKeys)
            {
                GameEvents.OnKeyPress(key);
            }

        }
        private void CheckCollisional()
        {
            for (int i = 0; i < _ObjectsList.Count; i++)
            {
                for (int j = 0; j < _ObjectsList.Count; j++)
                {
                    if (i != j && _ObjectsList[i].Colisional && _ObjectsList[j].Colisional
                        && !RectHelper.Intersect(_ObjectsList[i].Rect(), _ObjectsList[j].Rect()).IsEmpty)
                    {
                        if ((_ObjectsList[i] is Plane && _ObjectsList[j] is Bullet) || (_ObjectsList[i] is Bullet && _ObjectsList[j] is Plane))
                        {
                            _ObjectsList[i].Collide(_ObjectsList[j]);
                            break;
                        }
                        

                    }
                    
                }
            }
        }
    }
}
