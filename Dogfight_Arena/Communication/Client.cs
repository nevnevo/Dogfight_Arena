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
using System.Numerics;
using Dogfight_Arena.Services;


namespace Dogfight_Arena.Communication
{
    public class Client
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private int _localPort;
        private bool isInitialized = false;
        private bool _initializationFailed = false;
        public Packet.PlayerSide _Side;
        private long _randomSeed;
        public long StartTime = 0;

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
            initPacket.Data.Add("proposedSide", Objects.Plane.PlaneTypes.LeftPlane);
            
            initPacket.Data.Add("randomSeed", (long)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            initPacket.Data.Add("playerName", "RightNowItsEmpty");//******** implement here ********
            new Thread(Listen).Start();
            SendData(initPacket);
            while(!isInitialized && !_initializationFailed)//waiting for the initialization to be complete
                continue;

            if (!_initializationFailed)
            {

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
                    if ((Packet.PlayerSide)recievedPacket.Data["proposedSide"] != Packet.PlayerSide.Right)
                        _Side = Packet.PlayerSide.Right;
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
                    if ((Packet.PlayerSide)recievedPacket.Data["acceptedSide"] != _Side)
                    {
                        _randomSeed = (long)recievedPacket.Data["randomSeed"];
                        Packet confirmHandshake = new Packet(Packet.PacketType.confirmHandshake);
                        confirmHandshake.Data.Add("setSide",_Side);
                        SendData(confirmHandshake);
                        
                    }
                    else
                        _initializationFailed = true;
                    break;
                case (Packet.PacketType.confirmHandshake):
                    if ((Packet.PlayerSide)recievedPacket.Data["acceptedSide"] != _Side && (long)recievedPacket.Data["randomSeed"] == _randomSeed)
                        isInitialized = true;
                    else
                        _initializationFailed = true;
                    break;
                case (Packet.PacketType.Ready):
                    StartTime = (long)recievedPacket.Data["startingTime"];
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
