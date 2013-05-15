using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine
{
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.Messages;
    [ContractClass(typeof(IZoneClientContract))]
    public interface IZoneClient : IClient
    {
        void SendCompressed(MessageBody messageBody, int receiver, bool announceToPlayfield);
    }


    [ContractClassFor(typeof(IZoneClient))]
    abstract class IZoneClientContract : IZoneClient
    {
        public void SendCompressed(MessageBody messageBody, int receiver, bool announceToPlayfield)
        {
            Contract.Ensures(messageBody != null);
            Contract.Ensures(receiver != 0);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ServerBase Server { get; private set; }

        public IPAddress ClientAddress { get; private set; }

        public int Port { get; private set; }

        public IPEndPoint UdpEndpoint { get; set; }

        public Socket TcpSocket { get; set; }

        public bool IsConnected { get; private set; }

        public void BeginReceive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] packet)
        {
            throw new NotImplementedException();
        }

        public void SendCopy(byte[] packet)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] packet, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public void Send(BufferSegment segment, int length)
        {
            throw new NotImplementedException();
        }

        public void Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        public void Connect(IPAddress addr, int port)
        {
            throw new NotImplementedException();
        }
    }
}
