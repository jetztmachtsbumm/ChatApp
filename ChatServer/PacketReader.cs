using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class PacketReader : BinaryReader
    {

        NetworkStream ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            this.ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            int length = ReadInt32();
            msgBuffer = new byte[length];
            ns.Read(msgBuffer, 0, length);

            string msg = Encoding.UTF8.GetString(msgBuffer);

            return msg;
        }

    }
}
