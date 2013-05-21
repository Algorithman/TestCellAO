using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.InternalBus.InternalMessages
{
    using ZoneEngine.GameObject;
    using ZoneEngine.GameObject.Items;

    interface IIMItemNanoEvent
    {
        Dynel Target { get; set; }

        Dynel Self { get; set; }

        AOEvents Event { get; set; }
    }
}
