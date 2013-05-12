using System;

namespace ZoneEngine.CoreServer
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using Cell.Core;
    using NLog;
    using ZoneEngine.Component;

    [Export]
    public sealed class ZoneServer : ServerBase
    {
        public static readonly DateTime StartTime;

        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ClientFactory clientFactory;

        [ImportingConstructor]
        public ZoneServer(ClientFactory clientfactory)
        {
            this.clientFactory = clientfactory;
        }

        protected ZoneServer()
        {
            Log.Debug("Server is starting at " + StartTime.ToString());
        }

        static ZoneServer()
        {
            StartTime = DateTime.Now;
        }

        public static TimeSpan RunTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        public bool Running
        {
            get
            {
                return this._running;
            }
        }

        public HashSet<IClient> Clients
        {
            get
            {
                return base._clients;
            }
        }

        protected override IClient CreateClient()
        {
            return this.clientFactory.Create(this);
        }

        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
            Console.WriteLine("Sending to " + clientIP.Address);
        }
    }
}