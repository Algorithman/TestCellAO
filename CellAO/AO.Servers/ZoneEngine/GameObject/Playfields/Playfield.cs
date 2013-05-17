using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Playfields
{
    using AO.Core.Components;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Items;

    public class Playfield : IPlayfield
    {
        public IBus PlayfieldBus { get; set; }

        public Identity Identity { get; set; }

        private readonly ServerBase server;

        public HashSet<IInstancedEntity> Entities { get; private set; }

        public int NumberOfPlayers()
        {
            int count = 0;
            foreach (IInstancedEntity instancedEntity in Entities)
            {
                if ((instancedEntity as IPacketReceivingEntity) != null)
                {
                    count++;
                }
            }
            return count++;
        }

        public int NumberOfDynels()
        {
            return Entities.Count;
        }

        public List<AOFunctions> EnvironmentFunctions { get; private set; }

        public bool IsInstancedPlayfield()
        {
            throw new NotImplementedException();
        }

        public Playfield()
        {
        }
        public Playfield(ServerBase server, IBus bus)
        {
            this.PlayfieldBus = bus;
            this.server = server;
        }
}
}
