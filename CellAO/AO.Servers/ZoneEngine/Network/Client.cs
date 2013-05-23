#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace ZoneEngine.CoreClient
{
    #region Usings ...

    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Net.Sockets;

    using AO.Core.Components;
    using AO.Core.Events;
    using AO.Core.Logger;

    using Cell.Core;

    using ComponentAce.Compression.Libs.zlib;

    using Database;

    using NiceHexOutput;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.GameObject;
    using ZoneEngine.GameObject.Playfields;
    using ZoneEngine.Network;

    #endregion

    /// <summary>
    /// </summary>
    public class Client : ClientBase, IZoneClient
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly IBus bus;

        /// <summary>
        /// </summary>
        private Character character = new Character();

        /// <summary>
        /// </summary>
        private readonly IMessageSerializer messageSerializer;

        /// <summary>
        /// </summary>
        private string accountName = string.Empty;

        /// <summary>
        /// </summary>
        private bool zStreamSetup;

        /// <summary>
        /// </summary>
        private NetworkStream netStream;

        /// <summary>
        /// </summary>
        private ZOutputStream zStream;

        /// <summary>
        /// </summary>
        private string clientVersion = string.Empty;

        /// <summary>
        /// </summary>
        private ushort packetNumber = 1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="server">
        /// </param>
        /// <param name="messageSerializer">
        /// </param>
        /// <param name="bus">
        /// </param>
        public Client(ServerBase server, IMessageSerializer messageSerializer, IBus bus)
            : base(server)
        {
            this.messageSerializer = messageSerializer;
            this.bus = bus;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
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

        /// <summary>
        /// </summary>
        public Character Character
        {
            get
            {
                return this.character;
            }
            set
            {
                this.character = value;
            }
        }

        /// <summary>
        /// </summary>
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

        /// <summary>
        /// </summary>
        public IPlayfield Playfield;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private ZoneServer server
        {
            get
            {
                return (ZoneServer)this._server;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        public void Send(MessageBody messageBody)
        {
            // TODO: Investigate if reciever is a timestamp
            Contract.Requires(messageBody != null);
            var message = new Message
                              {
                                  Body = messageBody,
                                  Header =
                                      new Header
                                          {
                                              MessageId = this.packetNumber,
                                              PacketType = messageBody.PacketType,
                                              Unknown = 0x0001,
                                              Sender = 0x00000001,
                                              Receiver = this.character.Identity.Instance
                                          }
                              };
            byte[] buffer = this.messageSerializer.Serialize(message);

#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(NiceHexOutput.Output(buffer));
            Console.ResetColor();
#endif

            this.Send(buffer);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void SendCompressed(byte[] packet)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(NiceHexOutput.Output(packet));
            Console.ResetColor();
            LogUtil.Debug("Sending Compressed:\r\n" + NiceHexOutput.Output(packet));
#endif
            Contract.Requires(packet != null);
            Contract.Requires(1 < packet.Length);
            int tries = 0;
            bool done = false;

            // 18.1 Fix
            byte[] pn = BitConverter.GetBytes(this.packetNumber);
            packet[0] = pn[1];
            packet[1] = pn[0];
            this.packetNumber++;
            while ((!done) && (tries < 3))
            {
                try
                {
                    done = true;
                    if (!this.zStreamSetup)
                    {
                        // Create the zStream
                        this.netStream = new NetworkStream(this.TcpSocket);
                        this.zStream = new ZOutputStream(this.netStream, zlibConst.Z_BEST_COMPRESSION);
                        this.zStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
                        this.zStreamSetup = true;
                    }

                    this.zStream.Write(packet, 0, packet.Length);
                    this.zStream.Flush();
                }
                catch (Exception)
                {
                    tries++;
                    done = false;
                    this.Server.DisconnectClient(this);
                    return;
                }
            }
        }

        // <summary>
        // </summary>
        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void SendChatText(string text)
        {
            ChatTextMessage chatTextMessage = new ChatTextMessage();
            chatTextMessage.Text = text;
            this.Send(chatTextMessage);

            throw new NotImplementedException("SendChatText not implemented yet");
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="segment">
        /// </param>
        /// <returns>
        /// </returns>
        protected uint GetMessageNumber(BufferSegment segment)
        {
            Contract.Requires(segment != null);
            Contract.Requires(19 < ((BufferSegment)segment).SegmentData.Length);
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment.SegmentData[16];
            messageNumberArray[2] = segment.SegmentData[17];
            messageNumberArray[1] = segment.SegmentData[18];
            messageNumberArray[0] = segment.SegmentData[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        /// <summary>
        /// </summary>
        /// <param name="segment">
        /// </param>
        /// <returns>
        /// </returns>
        protected uint GetMessageNumber(byte[] segment)
        {
            Contract.Requires(segment != null);
            Contract.Requires(19 < segment.Length);
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment[16];
            messageNumberArray[2] = segment[17];
            messageNumberArray[1] = segment[18];
            messageNumberArray[0] = segment[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer">
        /// </param>
        /// <returns>
        /// </returns>
        protected override bool OnReceive(BufferSegment buffer)
        {
            Message message = null;

            var packet = new byte[this._remainingLength];
            Array.Copy(buffer.SegmentData, packet, this._remainingLength);

#if DEBUG
            Console.WriteLine("Receiving");
            Console.WriteLine("Offset: " + buffer.Offset.ToString() + " -- RemainingLength: " + this._remainingLength);
            Console.WriteLine(NiceHexOutput.Output(packet));
            LogUtil.Debug("\r\nReceived: \r\n" + NiceHexOutput.Output(packet));
#endif
            this._remainingLength = 0;
            try
            {
                message = this.messageSerializer.Deserialize(packet);
            }
            catch (Exception)
            {
                uint messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent malformed message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
                return false;
            }

            buffer.IncrementUsage();

            if (message == null)
            {
                uint messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent unknown message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
                return false;
            }

            this.bus.Publish(new MessageReceivedEvent(this, message));

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        public void SendCompressed(MessageBody messageBody)
        {
#if DEBUG
            Console.WriteLine("Sending Message: " + messageBody.GetType().ToString());
#endif
            var message = new Message
                              {
                                  Body = messageBody,
                                  Header =
                                      new Header
                                          {
                                              MessageId = this.packetNumber,
                                              PacketType = messageBody.PacketType,
                                              Unknown = 0x0001,
                                              Sender = 0x00000001,
                                              Receiver = this.character.Identity.Instance
                                          }
                              };
            byte[] buffer = this.messageSerializer.Serialize(message);
            if ((buffer == null) || (buffer.Length < 1))
            {
                throw new NullReferenceException("Serializer failure? (" + typeof(MessageBody).FullName + ")");
            }

            this.SendCompressed(buffer);
        }

        public void CreateCharacter(int charId)
        {
            this.character = new Character(new Identity { Type = IdentityType.CanbeAffected, Instance = charId }, this);
            var daochar = new CharacterDao().GetById(charId);
            if (daochar.Count() == 0)
            {
                throw new Exception("Character " + charId.ToString() + " not found.");
            }
            if (daochar.Count() > 1)
            {
                throw new Exception(daochar.Count().ToString() + " Characters with id " + charId.ToString() + " found??? Check Database setup!");
            }
            DBCharacter character = daochar.First();
            this.character.Name = character.Name;
            this.character.LastName = character.LastName;
            this.character.FirstName = character.FirstName;
            this.character.Coordinates = new Vector3();
            this.character.Coordinates.X = character.X;
            this.character.Coordinates.Y = character.Y;
            this.character.Coordinates.Z = character.Z;
            this.character.Heading = new Quaternion();
            this.character.Heading.X = character.HeadingX;
            this.character.Heading.Y = character.HeadingY;
            this.character.Heading.Z = character.HeadingZ;
            this.character.Heading.W = character.HeadingW;
            this.character.Playfield = server.PlayfieldById(character.Playfield);
            this.Playfield = this.character.Playfield;
            this.Playfield.Entities.Add(this.character);
            this.character.Stats.ReadStatsfromSql();

        }

        public void SendInitiateCompressionMessage(MessageBody messageBody)
        {

            // TODO: Investigate if reciever is a timestamp
            Contract.Requires(messageBody != null);
            var message = new Message
            {
                Body = messageBody,
                Header =
                    new Header
                    {
                        MessageId = 0xdfdf,
                        PacketType = messageBody.PacketType,
                        Unknown = 0x0001,
                        Sender = 0x03000000,
                        Receiver = this.character.Identity.Instance
                    }
            };
            byte[] buffer = this.messageSerializer.Serialize(message);

#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(NiceHexOutput.Output(buffer));
            Console.ResetColor();
            LogUtil.Debug(NiceHexOutput.Output(buffer));
#endif
            this.packetNumber = 1;

            this.Send(buffer);
        }
    }
}