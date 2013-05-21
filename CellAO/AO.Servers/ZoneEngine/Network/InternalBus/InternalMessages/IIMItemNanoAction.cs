using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.InternalBus.InternalMessages
{
    using ZoneEngine.GameObject;
    using ZoneEngine.GameObject.Items;

    interface IIMItemNanoAction
    {
        Dynel User { get; set; }

        AOActions Action { get; set; }
    }
}
