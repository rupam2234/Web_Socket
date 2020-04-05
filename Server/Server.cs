using System;
using System.Collections.Generic;
using System.Text;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using static ServerData.Packets;

namespace Server
{
    public class Server
    {
        public static Socket listnerSocket;
        public static List<ClientData> clientDatas;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on: " + Packets.GetIPAddress());
            Server server = new Server();
            listnerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientDatas = new List<ClientData>();

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Packets.GetIPAddress()), 8080);
            listnerSocket.Bind(endPoint);

            Thread listenThread = new Thread(ListenerThread);
            listenThread.Start();
        }

        // Listener = listens for clients trying to connect
        public static void ListenerThread()
        {
            for (; ; )
            {
                listnerSocket.Listen(0);
                clientDatas.Add(new ClientData(listnerSocket.Accept()));
                //clientDatas.Add(new ClientData(listnerSocket.Connected()));
            }
        }

        // read data from client
        public static void Data_in(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] buffer;
            int readBytes;

            
            for (; ; )
            {

                try
                {
                    buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(buffer);

                    if (readBytes > 0)
                    {
                        Packets packets = new Packets(buffer);
                        DataManager(packets);
                    }
                }

                // client disconnected ....
                catch
                {
                    Console.WriteLine("a socket disconnected!");
                }
                
            }

        }

        public static void DataManager(Packets packets)
        {
            switch(packets.PacketTypes)
            {
                case PacketType.Chat:
                    Server server = new Server();
                    foreach(ClientData clientData in clientDatas)
                    {
                        clientData.ClientSocket.Send(packets.toBytes());
                    }
                    break;
            }
        }

        // ClientData Thread = recieves data from each client individually        
    }

    public class ClientData
    {
        public Socket ClientSocket;
        public Thread clientThread;
        public string Clientid;

        public ClientData()
        {
            Clientid = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_in);
            clientThread.Start(ClientSocket);
            SendRegistrationPacket();
        }

        public ClientData(Socket clientSocket)
        {
            this.ClientSocket = clientSocket;
            Clientid = Guid.NewGuid().ToString();
            SendRegistrationPacket();
        }

        // send packet to clients
        public void SendRegistrationPacket()
        {
            Packets packets = new Packets(PacketType.Registration, "server");
            packets.GetData.Add(Clientid);
            ClientSocket.Send(packets.toBytes());
        }
    }
}
