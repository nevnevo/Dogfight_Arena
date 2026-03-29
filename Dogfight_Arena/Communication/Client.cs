using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Windows.ApplicationModel.Chat;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ComponentModel;
using Newtonsoft.Json;
using System.IO;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.Media.Protection.PlayReady;

using Dogfight_Arena.Services;
using Dogfight_Arena.Objects;
using System.Diagnostics;

namespace Dogfight_Arena.Communication
{
    public class Client
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private int _localPort;

        private bool isInitialized = false;
        private bool _isRunning = false;
        private Thread _listenThread;

        public Plane.PlaneTypes _Side;

        private long _mySeed;
        public long _randomSeed;

        public long StartTime = 0;

        private DispatcherTimer UpdateTimer;

        public static bool playAgainRequested = false;
        public static bool playAgainRequestedFromOther = false;

        public Client(int localPort)
        {
            _localPort = localPort;
        }

        public void InitializeConnection(IPAddress targetIp, int targetPort)
        {
            Debug.WriteLine($"Initializing connection to {targetIp}:{targetPort} from local port {_localPort}");

            _udpClient = new UdpClient(_localPort);
            _endPoint = new IPEndPoint(targetIp, targetPort);

            _isRunning = true;
            _listenThread = new Thread(Listen);
            _listenThread.Start();

            _mySeed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Packet initPacket = new Packet(Packet.PacketType.initGame);
            initPacket.Data.Add("randomSeed", _mySeed);
            initPacket.Data.Add("playerName", "RightPlaneNowItsEmpty");

            SendData(initPacket);

            while (!isInitialized)
                Thread.Sleep(10);

            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = TimeSpan.FromMilliseconds(16);
            UpdateTimer.Tick += SendUpdatePkt;
            UpdateTimer.Start();
        }

        private void SendUpdatePkt(object sender, object e)
        {
            if (!_isRunning)
                return;

            if (GameManager._ObjectsList[0] != null && GameManager._ObjectsList[0] is Plane)
            {
                Plane localPlane = (Plane)GameManager._ObjectsList[0];

                Packet UpdatePacket = new Packet(Packet.PacketType.Update);

                UpdatePacket.Data["X"] = localPlane._x;
                UpdatePacket.Data["Y"] = localPlane._y;
                UpdatePacket.Data["speed"] = localPlane._speed;
                UpdatePacket.Data["acceleration"] = localPlane._acceleration;
                UpdatePacket.Data["angle"] = localPlane._angle;

                SendData(UpdatePacket);
            }
        }

        public void SendData(Packet packet)
        {
            if (!_isRunning || _udpClient == null)
                return;

            string jsonData = JsonConvert.SerializeObject(packet);
            byte[] data = Encoding.UTF8.GetBytes(jsonData);

            try
            {
                _udpClient.Send(data, data.Length, _endPoint);
            }
            catch
            {
            }
        }

        private void Listen()
        {
            while (_isRunning)
            {
                try
                {
                    IPEndPoint remote = null;
                    byte[] data = _udpClient.Receive(ref remote);

                    string recievedData = Encoding.UTF8.GetString(data);

                    Task.Run(() =>
                    {
                        Packet recievedPacket =
                            JsonConvert.DeserializeObject<Packet>(recievedData);

                        ProccessPacket(recievedPacket);
                    });
                }
                catch (SocketException)
                {
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private async void ProccessPacket(Packet recievedPacket)
        {
            switch (recievedPacket.Type)
            {
                case (Packet.PacketType.initGame):

                    long remoteSeed = (long)recievedPacket.Data["randomSeed"];

                    _randomSeed = Math.Min(_mySeed, remoteSeed);

                    if (_mySeed < remoteSeed)
                        _Side = Plane.PlaneTypes.LeftPlane;
                    else
                        _Side = Plane.PlaneTypes.RightPlane;

                    isInitialized = true;

                    break;

                case (Packet.PacketType.Ready):

                    StartTime = (long)recievedPacket.Data["startingTime"];
                    break;

                case (Packet.PacketType.Update):

                    GameManager.GameEvents.PacketRecieved?.Invoke(recievedPacket);

                    break;

                case (Packet.PacketType.PlayAgain):

                    if (playAgainRequested)
                    {
                        var pkt = new Packet(Packet.PacketType.Time);

                        pkt.Data["startingTime"] =
                            (long)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5000;

                        SendData(pkt);
                    }

                    playAgainRequestedFromOther = true;

                    break;

                case (Packet.PacketType.Time):

                    StartTime = (long)recievedPacket.Data["startingTime"];

                    break;

                case (Packet.PacketType.OnShoot):

                    string[] keyNames =
                    {
                        "X",
                        "Y",
                        "angle",
                        "side",
                        "image"
                    };

                    foreach (string key in keyNames)
                    {
                        try
                        {
                            if (recievedPacket.Data[key] == null)
                                return;
                        }
                        catch
                        {
                            return;
                        }
                    }

                    if (Convert.ToString(recievedPacket.Data["image"]) == "Images/Bullet.png")
                    {
                        await Windows.ApplicationModel.Core.CoreApplication
                            .MainView.CoreWindow.Dispatcher
                            .RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () =>
                                {
                                    var proj =
                                        new Bullet(
                                            Convert.ToInt32(recievedPacket.Data["X"]),
                                            Convert.ToInt32(recievedPacket.Data["Y"]),
                                            Convert.ToString(recievedPacket.Data["image"]),
                                            GameManager._field,
                                            5,
                                            Convert.ToDouble(recievedPacket.Data["angle"]),
                                            (Plane.PlaneTypes)Convert.ToInt32(recievedPacket.Data["side"])
                                        );

                                    GameManager.GameEvents.OnShoot?.Invoke(proj);
                                });
                    }

                    break;
            }
        }

        public void Delete()
        {
            _isRunning = false;

            try
            {
                UpdateTimer?.Stop();
            }
            catch { }

            try
            {
                _udpClient?.Close();
            }
            catch { }

            try
            {
                _listenThread?.Join();
            }
            catch { }
        }
    }
}