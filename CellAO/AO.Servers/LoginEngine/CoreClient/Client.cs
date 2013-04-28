using System;

namespace LoginEngine.CoreClient
{
    using System.Globalization;

    using AO.Core.Components;
    using AO.Core.Events;

    using Cell.Core;
    using NiceHexOutput;

    using SmokeLounge.AOtomation.Messaging.Messages;

    public class Client : ClientBase
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly IBus bus;

        private string accountName = string.Empty;

        private string clientVersion = string.Empty;

        private ushort packetNumber = 1;

        private string serverSalt = string.Empty;

        #region Public Properties

        public string AccountName
        {
            get
            {
                return this.accountName;
            }

            set
            {
                this.accountName = value;
            }
        }

        public string ClientVersion
        {
            get
            {
                return this.clientVersion;
            }

            set
            {
                this.clientVersion = value;
            }
        }

        public string ServerSalt
        {
            get
            {
                return this.serverSalt;
            }

            set
            {
                this.serverSalt = value;
            }
        }

        #endregion


        public Client(ServerBase server, IMessageSerializer messageSerializer, IBus bus)
            : base(server)
        {
            this.messageSerializer = messageSerializer;
            this.bus = bus;
        }

        protected uint GetMessageNumber(BufferSegment segment)
        {
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment.SegmentData[16];
            messageNumberArray[2] = segment.SegmentData[17];
            messageNumberArray[1] = segment.SegmentData[18];
            messageNumberArray[0] = segment.SegmentData[19];
            var reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        protected uint GetMessageNumber(byte[] segment)
        {
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment[16];
            messageNumberArray[2] = segment[17];
            messageNumberArray[1] = segment[18];
            messageNumberArray[0] = segment[19];
            var reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }


        protected override bool OnReceive(BufferSegment buffer)
        {
            Message message = null;

            var packet = new byte[_remainingLength];
            Array.Copy(buffer.SegmentData, packet, _remainingLength);
            /* Uncomment for Incoming Messages
             */
            Console.WriteLine("Offset: " + buffer.Offset.ToString() + " -- RemainingLength: " + _remainingLength);
            Console.WriteLine(NiceHexOutput.Output(packet));
             
            _remainingLength = 0;
            try
            {

                message = this.messageSerializer.Deserialize(packet);
            }
            catch (Exception)
            {
                var messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent malformed message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
                return false;
            }
            buffer.IncrementUsage();

            if (message == null)
            {
                var messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent unknown message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
                return false;
            }
            this.bus.Publish(new MessageReceivedEvent(this, message));

            return true;
        }
        public void Send(int receiver, MessageBody messageBody)
        {
            // TODO: Investigate if reciever is a timestamp
            var message = new Message
            {
                Body = messageBody,
                Header =
                    new Header
                    {
                        MessageId = BitConverter.ToInt16(new byte[] { 0xDF, 0xDF }, 0),
                        PacketType = messageBody.PacketType,
                        Unknown = 0x0001,
                        Sender = 0x00000001,
                        Receiver = receiver
                    }
            };
            var buffer = this.messageSerializer.Serialize(message);

            buffer[0] = BitConverter.GetBytes(packetNumber)[1];
            buffer[1] = BitConverter.GetBytes(packetNumber)[0];
            packetNumber++;


            /* Uncomment for Debug outgoing Messages
             */
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(NiceHexOutput.Output(buffer));
            Console.ResetColor();
             

            if (buffer.Length % 4 > 0)
            {
                Array.Resize(ref buffer, buffer.Length + (4 - (buffer.Length % 4)));
            }

            this.Send(buffer);
        }
    }
}
