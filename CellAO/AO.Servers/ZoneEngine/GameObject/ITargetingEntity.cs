using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject
{
    using SmokeLounge.AOtomation.Messaging.GameData;

    public interface ITargetingEntity
    {
        /// <summary>
        /// </summary>
        Identity FightingTarget { get; set; }

        /// <summary>
        /// </summary>
        Identity SelectedTarget { get; set; }

        bool SetTarget(Identity identity);

        bool SetFightingTarget(Identity identity);
    }
}
