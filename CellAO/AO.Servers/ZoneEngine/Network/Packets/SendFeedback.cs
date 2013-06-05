using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.Packets
{
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class SendFeedback
    {
        public static bool Send(Client client, int MsgCategory, int MsgNum)
        {
            var message = new FeedbackMessage
            {
                Identity = client.Character.Identity,
                Unknown = 0x01,
                Unknown1 = 0x00000000,
                CategoryId = MsgCategory,
                MessageId = MsgNum
            };
            client.SendCompressed(message);
            return true;
        }

    }
}
