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

namespace Dogfight_Arena.Communication
{
    public class Client
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private int _localPort;
        private bool isInitialized = false;
        private bool _initializationFailed = false;
        public Plane.PlaneTypes _Side;
        public long _randomSeed;
        public long StartTime = 0;
        private DispatcherTimer UpdateTimer;

        public Client(int localPort)
        {
            _localPort = localPort;
            

            
           
        }
        public void InitializeConnection(IPAddress targetIp,int targetPort)
        {
            
            _udpClient = new UdpClient(_localPort);
            _endPoint = new IPEndPoint(targetIp, targetPort);
            _udpClient.Client.ReceiveTimeout = 30000; // 10 seconds

            Packet initPacket = new Packet(Packet.PacketType.initGame);
            initPacket.Data.Add("proposedSide", Plane.PlaneTypes.LeftPlane);
            
            initPacket.Data.Add("randomSeed", (long)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            initPacket.Data.Add("playerName", "RightPlaneNowItsEmpty");//******** implement here player's name******** 
            new Thread(Listen).Start();
            SendData(initPacket);
            while(!isInitialized && !_initializationFailed)//waiting for the initialization to be complete
                continue;

            if (!_initializationFailed)
            {
                UpdateTimer = new DispatcherTimer();
                UpdateTimer.Interval = TimeSpan.FromMilliseconds(25);
                UpdateTimer.Tick += SendUpdatePkt;
                UpdateTimer.Start();
            }

            




        }

        private void SendUpdatePkt(object sender, object e)
        {

           
            if(GameManager._ObjectsList[0]!=null && GameManager._ObjectsList[0] is Plane)
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
            string jsonData = JsonConvert.SerializeObject(packet);
            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            _udpClient.Send(data, data.Length, _endPoint);
        }
        private void Listen()
        {
            while(true)
            {
                try
                {
                    byte[] data = _udpClient.Receive(ref _endPoint);
                    string recievedData = Encoding.UTF8.GetString(data);

                    Task.Run(() =>
                    {
                        Packet recievedPacket = JsonConvert.DeserializeObject<Packet>(recievedData);
                        ProccessPacket(recievedPacket);
                    });
                }
                catch (SocketException)
                {
                    // Timeout or network error
                }
            }

        }

        private void ProccessPacket(Packet recievedPacket)
        {
            switch (recievedPacket.Type)
            {
                case (Packet.PacketType.initGame):
                    int a = Convert.ToInt32(recievedPacket.Data["proposedSide"]);
                    if ((Plane.PlaneTypes)a != Plane.PlaneTypes.RightPlane)
                        _Side = Plane.PlaneTypes.RightPlane;
                    else
                        _initializationFailed = true;
                    _randomSeed = (long)recievedPacket.Data["randomSeed"];
                    //implement popup for player about the opponents name
                    Packet HandshakeAck = new Packet(Packet.PacketType.initiated);
                    HandshakeAck.Data.Add("acceptedSide", _Side);
                    HandshakeAck.Data.Add("randomSeed", _randomSeed);
                    SendData(HandshakeAck);
                    break;
                case (Packet.PacketType.initiated):
                    if (Convert.ToInt32(recievedPacket.Data["acceptedSide"]) != ((int)_Side))
                    {
                        _randomSeed = (long)recievedPacket.Data["randomSeed"];
                        Packet confirmHandshake = new Packet(Packet.PacketType.confirmHandshake);
                        confirmHandshake.Data.Add("setSide", _Side);
                        confirmHandshake.Data.Add("randomSeed", _randomSeed);

                        SendData(confirmHandshake);

                    }
                    else
                        _initializationFailed = true;
                    break;
                case (Packet.PacketType.confirmHandshake):
                    if ((Plane.PlaneTypes)Convert.ToInt32(recievedPacket.Data["setSide"]) != _Side && (long)recievedPacket.Data["randomSeed"] == _randomSeed)
                    {
                        isInitialized = true;
                        Packet confirmHandshake2 = new Packet(Packet.PacketType.confirmHandshake);
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
                    if (GameManager.GameEvents.PacketRecieved != null)
                        GameManager.GameEvents.PacketRecieved(recievedPacket);
                    break;


                //case default:
                //    if (GameManager.GameEvents.PacketRecieved != null)
                //    {
                //        GameManager.GameEvents.PacketRecieved(recievedPacket);
                //    }
                //    break;


            }
        }
    }
}
