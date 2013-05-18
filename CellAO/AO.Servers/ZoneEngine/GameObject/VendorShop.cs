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

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Playfields;
    using ZoneEngine.GameObject.Stats;

    #endregion

    /// <summary>
    /// </summary>
    public class VendorShop : GameObject, INamedEntity, IInventory, IAOEvents
    {
        #region Public Properties

        /// <summary>
        /// </summary>
        public string Name
        {
            get
            {
                return this.Name;
            }

            set
            {
                this.Name = value;
            }
        }

        private IPlayfield playfield;
        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IPlayfield Playfield
        {
            get
            {
                return this.playfield;
            }

            set
            {
                this.playfield = value;
            }
        }

        /// <summary>
        /// </summary>
        private Vector3 coordinates = new Vector3();

        /// <summary>
        /// </summary>
        public Vector3 Coordinates
        {
            get
            {
                return this.coordinates;
            }

            set
            {
                this.coordinates = value;
            }
        }

        /// <summary>
        /// </summary>
        private Quaternion heading = new Quaternion();

        /// <summary>
        /// </summary>
        public Quaternion Heading
        {
            get
            {
                return this.heading;
            }

            set
            {
                this.heading = value;
            }
        }

        /// <summary>
        /// </summary>
        public DynelStats Stats
        {
            get
            {
                return this.Stats;
            }
        }

        #endregion
    }
}