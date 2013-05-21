using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.MessageHandlers
{
    using System.ComponentModel.Composition;

    using AO.Core.Components;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.CoreClient;
    using ZoneEngine.PacketHandlers;

    [Export(typeof(IHandleMessage))]
    public class CharInPlayHandler : IHandleMessage<CharInPlayMessage>
    {
        #region Public Methods and Operators

        public void Handle(object sender, Message message)
        {
            var client = (Client)sender;
            var charInPlayMessage = (CharInPlayMessage)message.Body;

            CharacterInPlay.Read(client);
        }

        #endregion
    }
}