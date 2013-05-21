#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace ZoneEngine.GameObject
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using AO.Core;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;

    using ZoneEngine.GameObject.Enums;
    using ZoneEngine.GameObject.Items;
    using ZoneEngine.GameObject.Misc;

    #endregion

    /// <summary>
    /// </summary>
    public sealed class Character : Dynel, 
                                    IPacketReceivingEntity, 
                                    INamedEntity, 
                                    ISummoner, 
                                    IAOEvents, 
                                    IAOActions, 
                                    IItemContainer
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly IZoneClient client;

        /// <summary>
        /// </summary>
        private MoveModes moveMode = MoveModes.None;

        /// <summary>
        /// </summary>
        private readonly IList<Pet> pets = new List<Pet>();

        /// <summary>
        /// </summary>
        public Identity FightingTarget = new Identity();

        /// <summary>
        /// </summary>
        private string name = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public IClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
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

        /// <summary>
        /// Active Nanos list
        /// </summary>
        private readonly List<AONano> activeNanos = new List<AONano>();

        /// <summary>
        /// Active Nanos list
        /// </summary>
        public List<AONano> ActiveNanos
        {
            get
            {
                return this.activeNanos;
            }
        }

        /// <summary>
        /// Textures
        /// </summary>
        public List<AOTextures> Textures;

        /// <summary>
        /// Social Tab structure. Mostly a 'cache', so we don't have to recalc the stuff all over again
        /// </summary>
        public Dictionary<int, int> SocialTab = new Dictionary<int, int>();

        /// <summary>
        /// Caching Mesh layer structure
        /// </summary>
        private readonly MeshLayers meshLayer = new MeshLayers();

        /// <summary>
        /// Caching Mesh layer for social tab items
        /// </summary>
        private readonly MeshLayers socialMeshLayer = new MeshLayers();

        /// <summary>
        /// Caching Mesh layer structure
        /// </summary>
        public MeshLayers MeshLayer
        {
            get
            {
                return this.meshLayer;
            }
        }

        /// <summary>
        /// Caching Mesh layer for social tab items
        /// </summary>
        public MeshLayers SocialMeshLayer
        {
            get
            {
                return this.socialMeshLayer;
            }
        }

        /// <summary>
        /// </summary>
        public string OrganizationName = string.Empty;

        /// <summary>
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// </summary>
        public IList<Pet> Pets
        {
            get
            {
                return this.pets;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        internal void Send(MessageBody messageBody)
        {
            Contract.Requires(messageBody != null);
            this.client.SendCompressed(messageBody);
        }

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        /// <param name="announceToPlayfield">
        /// </param>
        public void Send(MessageBody messageBody, bool announceToPlayfield)
        {
            if (!announceToPlayfield)
            {
                this.Send(messageBody);
                return;
            }

            this.playfield.Announce(messageBody);
        }

        #endregion

        /// <summary>
        /// Uploaded Nanos list
        /// </summary>
        private readonly List<AOUploadedNanos> uploadedNanos = new List<AOUploadedNanos>();

        /// <summary>
        /// Uploaded Nanos list
        /// </summary>
        public List<AOUploadedNanos> UploadedNanos
        {
            get
            {
                return this.uploadedNanos;
            }
        }

        /// <summary>
        /// </summary>
        public BaseInventory BaseInventory { get; private set; }

        /// <summary>
        /// </summary>
        public Character()
        {
            this.BaseInventory = new BaseInventory(this);
            this.pets = new List<Pet>();
        }

        /// <summary>
        /// </summary>
        /// <param name="identity">
        /// </param>
        /// <param name="client">
        /// </param>
        public Character(Identity identity, IZoneClient client)
        {
            this.identity = identity;
            this.BaseInventory = new BaseInventory(this);
            this.pets = new List<Pet>();
            this.client = client;
            this.Textures=new List<AOTextures>();
            this.uploadedNanos=new List<AOUploadedNanos>();
            this.meshLayer=new MeshLayers();
            this.socialMeshLayer=new MeshLayers();
            this.SocialTab=new Dictionary<int, int>();

            // TODO: Load names here?
        }

        /// <summary>
        /// </summary>
        /// <param name="identity">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal void SetTarget(Identity identity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal void CalculateSkills()
        {
            // TODO: Calculate Skills
        }
    }
}