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
        private bool _initializationFailed = false;

        private bool _isRunning = false;
        private Thread _listenThread;

        public Plane.PlaneTypes _Side;
        private Plane.PlaneTypes _proposedSide;

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
            _udpClient.Client.ReceiveTimeout = 30000;

            _isRunning = true;
            _listenThread = new Thread(Listen);
            _listenThread.Start();

            _proposedSide = Plane.PlaneTypes.LeftPlane;

            Packet initPacket = new Packet(Packet.PacketType.initGame);
            initPacket.Data.Add("proposedSide", _proposedSide);
            initPacket.Data.Add("randomSeed", (long)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            initPacket.Data.Add("playerName", "RightPlaneNowItsEmpty");

            SendData(initPacket);

            while (!isInitialized && !_initializationFailed)
                Thread.Sleep(10);

            if (!_initializationFailed)
            {
                UpdateTimer = new DispatcherTimer();
                UpdateTimer.Interval = TimeSpan.FromMilliseconds(16);
                UpdateTimer.Tick += SendUpdatePkt;
                UpdateTimer.Start();
            }
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

                    Plane.PlaneTypes remoteSide =
                        (Plane.PlaneTypes)Convert.ToInt32(recievedPacket.Data["proposedSide"]);

                    _Side = remoteSide == Plane.PlaneTypes.LeftPlane
                        ? Plane.PlaneTypes.RightPlane
                        : Plane.PlaneTypes.LeftPlane;

                    _randomSeed = (long)recievedPacket.Data["randomSeed"];

                    Packet HandshakeAck =
                        new Packet(Packet.PacketType.initiated);

                    HandshakeAck.Data.Add("acceptedSide", _Side);
                    HandshakeAck.Data.Add("randomSeed", _randomSeed);

                    SendData(HandshakeAck);

                    break;

                case (Packet.PacketType.initiated):

                    if (Convert.ToInt32(recievedPacket.Data["acceptedSide"]) != ((int)_Side))
                    {
                        _randomSeed = (long)recievedPacket.Data["randomSeed"];

                        Packet confirmHandshake =
                            new Packet(Packet.PacketType.confirmHandshake);

                        confirmHandshake.Data.Add("setSide", _Side);
                        confirmHandshake.Data.Add("randomSeed", _randomSeed);

                        SendData(confirmHandshake);
                    }
                    else
                        _initializationFailed = true;

                    break;

                case (Packet.PacketType.confirmHandshake):

                    if ((Plane.PlaneTypes)Convert.ToInt32(recievedPacket.Data["setSide"]) != _Side &&
                        (long)recievedPacket.Data["randomSeed"] == _randomSeed)
                    {
                        isInitialized = true;

                        Packet confirmHandshake2 =
                            new Packet(Packet.PacketType.confirmHandshake);

                        confirmHandshake2.Data.Add("setSide", _Side);
                        confirmHandshake2.Data.Add("randomSeed", _randomSeed);

                        SendData(confirmHandshake2);
                    }
                    else
                        _initializationFailed = true;

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