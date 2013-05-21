using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items
{
    public interface IItemNanoActions
    {
        List<AOActions> Actions { get; set; }
    }
}
