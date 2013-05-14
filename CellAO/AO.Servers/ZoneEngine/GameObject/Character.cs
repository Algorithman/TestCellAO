using System;
using System.Collections.Generic;
using Cell.Core;
using SmokeLounge.AOtomation.Messaging.GameData;
using ZoneEngine.GameObject.Enums;

namespace ZoneEngine.GameObject
{
    using System.Diagnostics.Contracts;

    public class Character : Dynel, IPacketReceivingEntity, INamedEntity, ISummoner, IAOEvents, IAOActions, IInstancedEntity
    {
        protected MoveModes moveMode = MoveModes.None;

        protected IClient client;

        protected IList<Pet> pets = new List<Pet>();

        public IClient Client
        {
            get
            {
                return this.client;
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
                throw new NotImplementedException();
            }
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

        public string Name
        {
            get
            {
                Contract.Ensures(Name != null, "Name is Null");
                Contract.Ensures(Name != "", "No name set");
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value != "");
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        public IList<Pet> Pets
        {
            get
            {
                return pets;
            }
        }

        internal void Send(SmokeLounge.AOtomation.Messaging.Messages.MessageBody message, bool announceToPlayfield)
        {
            throw new NotImplementedException();
        }
    }
}