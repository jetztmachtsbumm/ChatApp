using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatApp
{
    public class Server
    {
        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;

        PacketReader packetReader;

        TcpClient client;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("192.168.2.220", 25565);

                packetReader = new PacketReader(client.GetStream());

                PacketBuilder connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteString(username);

                client.Client.Send(connectPacket.GetPacketBytes());

                ReadPackets();
            }
        }

        public void SendMessage(string message)
        {
            PacketBuilder messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteString(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                    }
                }
            });
        }

        public PacketReader GetPacketReader()
        {
            return packetReader;
        }

    }
}
