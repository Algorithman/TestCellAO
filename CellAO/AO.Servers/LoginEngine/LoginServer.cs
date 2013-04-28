using System;

namespace LoginEngine.CoreServer
{
    using System.ComponentModel.Composition;
    using System.Net;

    using Cell.Core;

    using LoginEngine.Component;

    using NLog;


    [Export]
    public sealed class LoginServer : ServerBase
    {
        private readonly ClientFactory clientFactory;

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

        [ImportingConstructor]
        public LoginServer(ClientFactory clientfactory)
        {
            this.clientFactory = clientfactory;
        }
        public static readonly DateTime StartTime;

        public static TimeSpan RunTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        static LoginServer()
        {
            StartTime = DateTime.Now;

        }

        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected LoginServer()
        {
            Log.Debug("Server is starting at " + StartTime.ToString());
        }
    }
}
