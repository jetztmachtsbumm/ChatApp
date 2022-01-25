using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {

        static List<Client> users;
        static TcpListener listener;

        static void Main(string[] args)
        {
            users = new List<Client>();

            listener = new TcpListener(IPAddress.Any, 25565);
            listener.Start();

            Console.WriteLine("Server started successfully!");

            while (true)
            {
                Client client = new Client(listener.AcceptTcpClient());
                users.Add(client);

                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach(Client user in users)
            {
                foreach(Client usr in users)
                {
                    PacketBuilder broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.GetUsername());
                    broadcastPacket.WriteMessage(usr.GetUID().ToString());
                    user.GetClient().Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string msg)
        {
            foreach(Client user in users)
            {
                PacketBuilder msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(msg);
                user.GetClient().Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            Client disconnectedUser = users.Where(x => x.GetUID().ToString() == uid).FirstOrDefault();
            users.Remove(disconnectedUser);

            foreach (Client user in users)
            {
                PacketBuilder packet = new PacketBuilder();
                packet.WriteOpCode(10);
                packet.WriteMessage(uid);
                user.GetClient().Client.Send(packet.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.GetUsername()}] Disconnected!");
        }

    }
}
