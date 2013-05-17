﻿#region License

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

namespace ZoneEngine.CoreServer
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;

    using AO.Core.Logger;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Component;
    using ZoneEngine.GameObject.Playfields;

    #endregion

    /// <summary>
    /// </summary>
    [Export]
    public sealed class ZoneServer : ServerBase
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        public static readonly DateTime StartTime;

        #endregion

        #region Fields

        /// <summary>
        /// </summary>
        private readonly ClientFactory clientFactory;

        /// <summary>
        /// </summary>
        private readonly PlayfieldFactory playfieldFactory;

        private List<IPlayfield> playfields = new List<IPlayfield>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        static ZoneServer()
        {
            StartTime = DateTime.Now;
            LogUtil.Debug("Server is starting at " + StartTime.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="clientfactory">
        /// </param>
        /// <param name="playfieldFactory">
        /// </param>
        [ImportingConstructor]
        public ZoneServer(ClientFactory clientfactory, PlayfieldFactory playfieldFactory)
        {
            this.clientFactory = clientfactory;
            this.playfieldFactory = playfieldFactory;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public static TimeSpan RunTime
        {
            get
            {
                return DateTime.Now - StartTime;
            }
        }

        /// <summary>
        /// </summary>
        public HashSet<IClient> Clients
        {
            get
            {
                return base._clients;
            }
        }

        /// <summary>
        /// </summary>
        public bool Running
        {
            get
            {
                return this._running;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        protected override IClient CreateClient()
        {
            return this.clientFactory.Create(this);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        protected IPlayfield CreatePlayfield(Identity playfieldIdentity)
        {
            return this.playfieldFactory.Create(this, playfieldIdentity);
        }

        /// <summary>
        /// </summary>
        /// <param name="num_bytes">
        /// </param>
        /// <param name="buf">
        /// </param>
        /// <param name="ip">
        /// </param>
        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="clientIP">
        /// </param>
        /// <param name="num_bytes">
        /// </param>
        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
            Console.WriteLine("Sending to " + clientIP.Address);
        }

        public int CreatePlayfields()
        {
            foreach (PlayfieldInfo playfieldInfo in Playfields.Instance.playfields)
            {
                if (!playfieldInfo.disabled)
                {
                    Identity identity = new Identity();
                    identity.Type = IdentityType.Playfield;
                    identity.Instance = playfieldInfo.id;
                    IPlayfield playfield = this.CreatePlayfield(identity);

                    foreach (DistrictInfo districtInfo in playfieldInfo.districts)
                    {
                        PlayfieldDistrict playfieldDistrict=new PlayfieldDistrict();
                        playfieldDistrict.Name = districtInfo.districtName;
                        playfieldDistrict.MinLevel = districtInfo.minLevel;
                        playfieldDistrict.MaxLevel = districtInfo.maxLevel;
                        playfieldDistrict.SuppressionGas = districtInfo.suppressionGas;
                        playfield.Districts.Add(playfieldDistrict);
                    }

                    playfields.Add(playfield);
                }
            }
            return playfields.Count;
        }

        #endregion
    }
}