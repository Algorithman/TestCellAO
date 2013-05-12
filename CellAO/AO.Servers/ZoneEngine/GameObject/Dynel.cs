using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
        public DynelStats Stats
        {
            get
            {
                return stats;
            }
        }
    }
}