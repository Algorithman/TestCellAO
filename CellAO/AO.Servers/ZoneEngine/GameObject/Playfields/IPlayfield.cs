using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Playfields
{
    using System.Diagnostics.Contracts;

    using AO.Core.Components;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Items;

    public interface IPlayfield
    {
        IBus PlayfieldBus { get; set; }

        Identity Identity { get; set; }

        HashSet<IInstancedEntity> Entities { get; }
        
        int NumberOfPlayers();

        int NumberOfDynels();

        List<AOFunctions> EnvironmentFunctions { get; }

        bool IsInstancedPlayfield();


    }
}
