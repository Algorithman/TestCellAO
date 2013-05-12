using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmokeLounge.AOtomation.Messaging.GameData;
using ZoneEngine.GameObject.Stats;

namespace ZoneEngine.GameObject
{
    public class Dynel : GameObject, IInstancedEntity, IStats
    {
        protected DynelStats stats;


        public Dynel()
        {
            stats = new DynelStats(this);
        }
        
        public Identity Playfield
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        public DynelStats Stats
        {
            get
            {
                return stats;
            }
        }
    }
}