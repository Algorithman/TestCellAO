using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.MessageHandlers
{
    using System.ComponentModel.Composition;

    using AO.Core.Components;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.SystemMessages;

    using ZoneEngine.CoreClient;
    using ZoneEngine.PacketHandlers;

    [Export(typeof(IHandleMessage))]
    public class ZoneLoginHandler : IHandleMessage<ZoneLoginMessage>
    {
        #region Public Methods and Operators

        public void Handle(object sender, Message message)
        {
            var client = (Client)sender;
            var zoneLoginMessage = (ZoneLoginMessage)message.Body;
            client.CreateCharacter(zoneLoginMessage.CharacterId);
            client.Character.Playfield = client.Playfield;

            client.Send(new InitiateCompressionMessage());

            ClientConnected tmpClientConnected = new ClientConnected();
            tmpClientConnected.Read(zoneLoginMessage.CharacterId, client);
        }

        #endregion
    }
}