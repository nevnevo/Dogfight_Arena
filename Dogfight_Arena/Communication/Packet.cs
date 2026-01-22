using Dogfight_Arena.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dogfight_Arena.Communication
{
    public class Packet
    {
        public enum PacketType
        {
            initGame,
            initiated,
            confirmHandshake,
            Ready,
            Update,
            OnShoot,
            PlayAgain,
            Time
        }
        public enum PlayerSide
        {
            Left,
            Right
        }
        public PacketType Type { get; set; }
        public Plane.PlaneTypes SenderSide { get; set; }

        public long Timestamp { get; set; }  

        
        public Dictionary<string, object> Data { get; set; }

        public Packet(PacketType type)
        {
            Type = type;
            
            Data = new Dictionary<string, object>();
        }
    }
}
