using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Collections.Generic;

namespace ServerData
{
    [Serializable]
    public class Packets
    {
        public List<string> GetData;
        public int packetInt;
        public bool packetBool;
        public string senderID;
        public PacketType PacketTypes;

        public Packets(PacketType type, string senderID)
        {
            GetData = new List<string>();
            this.senderID = senderID;
            this.PacketTypes = type;
        }

        public byte[] toBytes()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, this);
            byte[] dataMemory = memoryStream.ToArray();
            memoryStream.Close();
            return dataMemory;
        }

        // create a packet from the recieved bytes
        public Packets(byte[] packetBytes)
        {
            BinaryFormatter binaryFormatter2 = new BinaryFormatter();
            MemoryStream memoryStream2 = new MemoryStream(packetBytes);

            Packets p = (Packets)binaryFormatter2.Deserialize(memoryStream2);
            memoryStream2.Close();
            this.GetData = p.GetData;
            this.packetInt = p.packetInt;
            this.packetBool = p.packetBool;
            this.senderID = p.senderID;
            this.PacketTypes = p.PacketTypes;
        }

        public static string GetIPAddress()
        {
            IPAddress[] iPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress iP in iPs)
            {
                if (iP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return iP.ToString();
                }
                
            }

            return "127.0.0.1";
        }

        public enum PacketType
        {
            Registration, Chat
        }
    }
}
