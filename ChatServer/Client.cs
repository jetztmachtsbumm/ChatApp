using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        string username;
        Guid uid;
        TcpClient client;
        PacketReader packetReader;

        public Client(TcpClient client)
        {
            this.client = client;
            uid = Guid.NewGuid();
            packetReader = new PacketReader(client.GetStream());

            byte opcode = packetReader.ReadByte();
            username = packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    byte opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            string msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{username}]: {msg}");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{uid.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(uid.ToString());
                    client.Close();
                    break;
                }
            }
        }

        public string GetUsername()
        {
            return username;
        }

        public Guid GetUID()
        {
            return uid;
        }

        public TcpClient GetClient()
        {
            return client;
        }

    }
}
