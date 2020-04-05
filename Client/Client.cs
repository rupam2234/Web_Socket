using System;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static ServerData.Packets;
//using static ServerData.Packets;

namespace Client 
{
    class Client
    {
        public static Socket master;
        public static string name;
        public static string id;

        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            name = Console.ReadLine();

            Attribute: Console.Clear();

            Console.Write("Enter Host IP address: ");
            string ip = Console.ReadLine();

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 8080);

            try
            {
                master.Connect(iPEndPoint);

                Console.WriteLine(master.LocalEndPoint.ToString());
            }

            catch
            {
                Console.WriteLine("Couldn't connect to the host");
                Thread.Sleep(1000);
                goto Attribute;
            }

            Thread thread = new Thread(DATA_in);
            thread.Start();

            for(; ; )
            {
                Console.Write("::>");
                string input = Console.ReadLine();
                Packets packets = new Packets(PacketType.Chat, id);
                
                // possible error 
                packets.GetData.Add(name);
                packets.GetData.Add(input);
                //

                //track the list
                //Console.WriteLine(packets.GetData[0].ToString());
                //Console.WriteLine(packets.GetData[1].ToString());
                
                master.Send(packets.toBytes());
            }

        }

        public static void DATA_in()
        {
            byte[] buffer;
            int readBytes;

            for (; ; )
            {

                try
                {
                    buffer = new byte[master.SendBufferSize];
                    readBytes = master.Receive(buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packets(buffer));
                    }
                }

                catch
                {
                    Console.WriteLine("Server Disconnected...");
                }
                
            }
        }

        // client get a response from the server
        public static void DataManager( Packets packets)
        {
            switch(packets.PacketTypes)
            {
                case PacketType.Registration:
                    //Console.WriteLine("Recived a packet for registration! Responding...");
                    id = packets.GetData[0];
                    break;
                case PacketType.Chat:
                    //ConsoleColor color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(packets.GetData[0] + ": " + packets.GetData[1]);
                    //Console.ForegroundColor = color;
                    break;
            }
        }
    }
}
