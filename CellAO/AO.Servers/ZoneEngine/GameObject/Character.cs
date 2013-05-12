using System;
using System.Collections.Generic;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace ZoneEngine.GameObject
{
    public class Character : Dynel, IPacketReceivingEntity, INamedEntity, ISummoner, IAOEvents, IAOActions, IInstancedEntity
    {
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

        public string Name
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

        public IList<Pet> Pets
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
        }
    }
}