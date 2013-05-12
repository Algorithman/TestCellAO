using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SmokeLounge.AOtomation.Messaging.GameData;

namespace ZoneEngine.GameObject
{
    public interface IEntity
    {
        Identity Identity
        {
            get;
        }
    }
}