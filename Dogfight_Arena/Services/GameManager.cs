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
        private readonly object _objectListLock = new object();
        public List<GameObject> _ObjectsList = new List<GameObject>();

        private DispatcherTimer _runTimer;
        private DispatcherTimer _spawnHealthCratesTimer;
        private HashSet<VirtualKey> ActiveKeys = new HashSet<VirtualKey>(); //Hashset is used to avoid duplicatekeys,because it can contain only unique values
        private Plane LocalPlayer;
        private Plane SecondPlayer;
        private Canvas _field;
        private int PlayerWidth = 100;
        private bool _isLastHealthCrateOn = false;
        public static Events GameEvents { get; private set; } = new Events();
        public GameManager(Canvas field) 
        {
            _field = field;
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(1);
            _runTimer.Start();
            _runTimer.Tick += runTimer_Tick;
            _spawnHealthCratesTimer = new DispatcherTimer();
            _spawnHealthCratesTimer.Interval = TimeSpan.FromMilliseconds(10000);
            
            if (!IsOnline)
            {//If the player chose to play offline
                LocalPlayer = new LeftPlane(100, 100, "Images/LeftPlayer.png", field, PlayerWidth);
                SecondPlayer = new RightPlane(field.ActualWidth - 100 - 200, 100, "Images/RightPlayer.png", field, PlayerWidth);
                _ObjectsList.Add(LocalPlayer);
                _ObjectsList.Add(SecondPlayer);
                _spawnHealthCratesTimer.Start();
                _spawnHealthCratesTimer.Tick += _spawnHealthCratesTimer_Tick;
            }
            

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            GameEvents.OnShoot += OnShoot;
            GameEvents.OnDelete += DeleteObject;
        }

        private void _spawnHealthCratesTimer_Tick(object sender, object e)
        {
            if (!_isLastHealthCrateOn)
            {
                Random rnd = new Random();
                _ObjectsList.Add(new Crate(rnd.Next(0,(int)_field.ActualWidth), rnd.Next(0, (int)_field.ActualHeight), "Images/healthCrate.png",_field,50,Crate.CrateTypes.HealthCrate));
            }
        }

        public void DeleteObject(GameObject go)
        {
            if(go is Bullet bullet)
            {
                if (_ObjectsList[0] is LeftPlane lplane)
                {
                    if (bullet._ShootingPlayer == Plane.PlaneTypes.LeftPlane)
                        lplane.BulletsLeft++;
                    else
                    {
                        if (_ObjectsList[1] is RightPlane rplane)
                        {
                            rplane.BulletsLeft++;
                        }
                    }
                }
                else if (_ObjectsList[0] is RightPlane rplane)
                {
                    if (bullet._ShootingPlayer == Plane.PlaneTypes.RightPlane)
                        rplane.BulletsLeft++;
                    else
                    {
                        if (_ObjectsList[1] is LeftPlane lplane2)
                        {
                            lplane2.BulletsLeft++;
                        }
                    }
                }
                lock (_objectListLock)
                {
                    _ObjectsList.Remove(bullet);
                }
                
                bullet.Remove();
                
            }
            if(go is Crate hc)
            {
                lock (_objectListLock)
                {
                    _ObjectsList.Remove(hc);
                }

                hc.Remove();
            }

        }
        

        private void OnShoot(Projectile projectile)
        {
            if(projectile is Bullet bullet)
            {
                lock (_objectListLock)
                {
                    _ObjectsList.Add(bullet);
                }
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (GameEvents.OnKeyPress != null)
            {
                if (args.VirtualKey == Keys.ShootBulletLeftPlayer || args.VirtualKey==Keys.ShootBulletRightPlayer)
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
            List<GameObject> currentObjects;

           
            lock (_objectListLock)
            {
                currentObjects = _ObjectsList.ToList();
            }

            
            foreach (GameObject obj in currentObjects)
            {
                if (obj is GameMovingObject)
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
                        )
                        if(!RectHelper.Intersect(_ObjectsList[i].Rect(), _ObjectsList[j].Rect()).IsEmpty)
                        {
                            if ((_ObjectsList[i] is Plane && _ObjectsList[j] is Bullet) || (_ObjectsList[i] is Bullet && _ObjectsList[j] is Plane) || (_ObjectsList[i] is Plane && _ObjectsList[j] is Crate))
                            {
                                _ObjectsList[i].Collide(_ObjectsList[j]);
                                break;
                            }
                        }
                    
                    
                }
            }
        }
        public void UnsubscribeAllEvents()
        {
            GameEvents.OnShoot -= OnShoot;
            Window.Current.CoreWindow.KeyDown-= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp-= CoreWindow_KeyUp;
            GameEvents.OnDelete -= DeleteObject;
            LocalPlayer.UnsubscribeEvents();
            SecondPlayer.UnsubscribeEvents();

        }


    }
}
