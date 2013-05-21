using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.InternalBus.InternalMessages
{
    using ZoneEngine.GameObject;

    public interface IIMAction
    {
        Dynel Initiator { get; set; }

        Dynel Target { get; set; }

    }
}
