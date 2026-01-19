using Dogfight_Arena.Communication;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Dogfight_Arena.Services
{
    public class GameManager
    {
        public static bool IsOnline { get; set; }
        private readonly object _objectListLock = new object();
        public static List<GameObject>  _ObjectsList = new List<GameObject>();

        private DispatcherTimer _runTimer;
        private DispatcherTimer _spawnHealthCratesTimer;
        private HashSet<VirtualKey> ActiveKeys = new HashSet<VirtualKey>(); //Hashset is used to avoid duplicatekeys,because it can contain only unique values
        public static Plane LocalPlayer;
        private Plane SecondPlayer;
        public static Canvas _field;
        private int PlayerWidth = 100;
        private bool _isLastHealthCrateOn = false;
        private bool _isLastMissileCrateOn = false;
        public static Events GameEvents { get; private set; } = new Events();
        public static Client client = new Client(42069);
        public static Plane.PlaneTypes LocalPlayerType;
        //implement method to get other players ip
        private string targetIp = "192.168.2.6";
        private int targetPort = 42069;
        public static int UIthread;
        private int seed = 0;
        
        public GameManager(Canvas field) 
        {
            
            UIthread = Thread.CurrentThread.ManagedThreadId;
            _field = field;
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(1);
            
            _runTimer.Tick += runTimer_Tick;
            _spawnHealthCratesTimer = new DispatcherTimer();
            _spawnHealthCratesTimer.Interval = TimeSpan.FromMilliseconds(10000);

            if (!IsOnline)
            {//If the player chose to play offline
                LocalPlayer = new LeftPlane(100, 150, "Images/LeftPlayer.png", field, PlayerWidth);
                SecondPlayer = new RightPlane(field.ActualWidth - 100 - 200, 150, "Images/RightPlayer.png", field, PlayerWidth);
                _ObjectsList.Add(LocalPlayer);
                _ObjectsList.Add(SecondPlayer);
                _spawnHealthCratesTimer.Start();
                _spawnHealthCratesTimer.Tick += _spawnCratesTimer_Tick;
                _runTimer.Start();
            }
            else 
            {
                client.InitializeConnection(IPAddress.Parse(targetIp), targetPort);
                LocalPlayerType = client._Side;
                if (client._Side == Plane.PlaneTypes.LeftPlane)
                {
                    LocalPlayer = new LeftPlane(100, 150, "Images/LeftPlayer.png", field, PlayerWidth);
                    SecondPlayer = new RightPlane(field.ActualWidth - 100 - 200, 150, "Images/RightPlayer.png", field, PlayerWidth);
                    
                }
                else
                {
                    SecondPlayer = new LeftPlane(100, 150, "Images/LeftPlayer.png", field, PlayerWidth);
                    LocalPlayer = new RightPlane(field.ActualWidth - 100 - 200, 150, "Images/RightPlayer.png", field, PlayerWidth);
                }
                _ObjectsList.Add(LocalPlayer);
                _ObjectsList.Add(SecondPlayer);
                
                _spawnHealthCratesTimer.Tick += _spawnCratesTimer_Tick;
                if (client._Side == Plane.PlaneTypes.LeftPlane)
                {
                    Packet pkt = new Packet(Packet.PacketType.Ready);
                    pkt.Data.Add("startingTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+5000);
                    client.SendData(pkt);
                    client.StartTime = (long)pkt.Data["startingTime"];
                    if (client.StartTime != 0)
                    {
                        long curTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        while (curTime != client.StartTime)
                        {
                            curTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        }
                        _runTimer.Start();
                        _spawnHealthCratesTimer.Start();

                    }
                }
                else
                {
                    while(client.StartTime == 0)
                    {
                        continue;
                    }
                    if (client.StartTime != 0)
                    {
                        long curTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        while (curTime < client.StartTime)
                        {
                            curTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        }
                        _runTimer.Start();
                        _spawnHealthCratesTimer.Start();

                    }
                }
                    

            }

            

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            GameEvents.OnShoot += OnShoot;
            GameEvents.OnDelete += DeleteObject;
            GameEvents.CreateMissile += CreateMissile;
            GameEvents.PacketRecieved += PacketRecieved;
        }

        private void PacketRecieved(Packet packet)
        {
            if (packet != null)
            {
                SecondPlayer.SetNewData(packet);
            }
            

        }

        private void CreateMissile(Plane.PlaneTypes ShootingPlayer)
        {
            Plane enemyPlane = null;
            Plane thisPlane = null;
            foreach(GameObject obj in _ObjectsList)
            {
                if (obj is Plane plane)
                {

                
                    if (plane.PlaneType == ShootingPlayer)
                        thisPlane = plane;
                    else
                        enemyPlane = plane;
                }

            }

            Missile missile = new Missile(thisPlane._x,thisPlane._y,"Images/missile.png",_field,50,thisPlane._angle,ShootingPlayer,enemyPlane);
            lock (_objectListLock)
            {
                _ObjectsList.Add(missile);
            }

        }

       

        private void _spawnCratesTimer_Tick(object sender, object e)
        {
            if (!IsOnline)
            {
                if (!_isLastHealthCrateOn)
                {
                    Random rnd = new Random();
                    _ObjectsList.Add(new HealthCrate(rnd.Next(0, (int)_field.ActualWidth), rnd.Next(0, (int)_field.ActualHeight), "Images/healthCrate.png", _field, 50, HealthCrate.CrateTypes.HealthCrate));
                    _isLastHealthCrateOn = true;
                }
                if (!_isLastMissileCrateOn)
                {
                    Random rnd = new Random();
                    _ObjectsList.Add(new HealthCrate(rnd.Next(0, (int)_field.ActualWidth), rnd.Next(0, (int)_field.ActualHeight), "Images/MissileCrate.png", _field, 50, HealthCrate.CrateTypes.MissileCrate));
                    _isLastMissileCrateOn = true;
                }
            }
            else
            {
                if (client._randomSeed == 0 && seed == 0)
                    return;
                if (seed == 0)
                    seed = (int)client._randomSeed;
                Random rnd = new Random((int)seed);
                if (!_isLastHealthCrateOn)
                {
                    
                    _ObjectsList.Add(new HealthCrate(rnd.Next(0, Constants.WINDOWSIZE), rnd.Next(0, Constants.WINDOWSIZE), "Images/healthCrate.png", _field, 50, HealthCrate.CrateTypes.HealthCrate));
                    _isLastHealthCrateOn = true;
                }
                if (!_isLastMissileCrateOn)
                {
                   
                    _ObjectsList.Add(new HealthCrate(rnd.Next(0, Constants.WINDOWSIZE), rnd.Next(0, Constants.WINDOWSIZE), "Images/MissileCrate.png", _field, 50, HealthCrate.CrateTypes.MissileCrate));
                    _isLastMissileCrateOn = true;
                }
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
            if(go is HealthCrate hc)
            {
                lock (_objectListLock)
                {
                    _ObjectsList.Remove(hc);
                    if(hc.CrateType==HealthCrate.CrateTypes.MissileCrate)
                        _isLastMissileCrateOn=false;
                    else
                        _isLastMissileCrateOn = true;
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
                if (args.VirtualKey == Keys.ShootBulletLeftPlayer || args.VirtualKey==Keys.ShootBulletRightPlayer || args.VirtualKey == Keys.ShootMissileLeftPlayer || args.VirtualKey == Keys.ShootMissileRightPlayer)
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
                            if ((_ObjectsList[i] is Plane && _ObjectsList[j] is Bullet) || (_ObjectsList[i] is Bullet && _ObjectsList[j] is Plane) || (_ObjectsList[i] is Plane && _ObjectsList[j] is HealthCrate) || (_ObjectsList[i] is Plane && _ObjectsList[j] is Missile))
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
            
            GameEvents.CreateMissile -= CreateMissile;
            LocalPlayer.UnsubscribeEvents();
            SecondPlayer.UnsubscribeEvents();


        }


    }
}
