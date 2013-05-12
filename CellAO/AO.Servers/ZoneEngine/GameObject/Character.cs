using System;
using System.Collections.Generic;
using ZoneEngine.GameObject.Enums;

namespace ZoneEngine.GameObject
{
    public class Character : Dynel, IPacketReceivingEntity, INamedEntity, ISummoner, IAOEvents, IAOActions
    {
        protected MoveModes moveMode = MoveModes.None;

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

        public MoveModes MoveMode
        {
            get
            {
                return this.moveMode;
            }
            set
            {
                throw new NotImplementedException("Character.MoveMode.set");
            }
        }
    }
}