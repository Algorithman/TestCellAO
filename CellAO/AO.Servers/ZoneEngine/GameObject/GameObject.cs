using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace ZoneEngine.GameObject
{
    public class GameObject : IEntity
    {

        protected Identity identity;

        public Identity Identity
        {
            get
            {
                return identity;
            }
        }
    }
}