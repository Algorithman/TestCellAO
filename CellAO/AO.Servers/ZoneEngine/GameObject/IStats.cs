using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoneEngine.GameObject.Stats;

namespace ZoneEngine.GameObject
{
    public interface IStats
    {
        DynelStats Stats { get; }
    }
}
