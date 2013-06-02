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

namespace ZoneEngine.GameObject.Items
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Gameobject.Items;

    #endregion

    /// <summary>
    /// </summary>
    public class Item : IItem
    {
        /// <summary>
        /// </summary>
        private ItemTemplate templateLow;

        /// <summary>
        /// </summary>
        private ItemTemplate templateHigh;

        /// <summary>
        /// </summary>
        private readonly Dictionary<int, int> Attributes = new Dictionary<int, int>();

        /// <summary>
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// </summary>
        public Identity Identity { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="attributeId">
        /// </param>
        /// <returns>
        /// </returns>
        public int GetAttribute(int attributeId)
        {
            if (this.Attributes.Keys.Contains(attributeId))
            {
                return this.Attributes[attributeId];
            }

            int lowAttribute = this.templateLow.getItemAttribute(attributeId);
            int highAttribute = this.templateHigh.getItemAttribute(attributeId);

            if (this.templateHigh.Quality - this.templateLow.Quality == 0)
            {
                return lowAttribute;
            }

            return
                Convert.ToInt32(
                    (double)lowAttribute
                    + (highAttribute - lowAttribute) * (this.Quality - this.templateLow.Quality)
                    / (this.templateHigh.Quality - this.templateLow.Quality));
        }

        /// <summary>
        /// </summary>
        /// <param name="attributeId">
        /// </param>
        /// <param name="newValue">
        /// </param>
        public void SetAttribute(int attributeId, int newValue)
        {
            if (this.GetAttribute(attributeId) != newValue)
            {
                if (this.Identity.Type == IdentityType.None)
                {
                    // TODO: Instantiate Item
                }
            }
            // Do always set it for caching purposes
            this.Attributes.Add(attributeId, newValue);
        }

        public int LowID
        {
            get
            {
                return templateLow.ID;
            }
        }

        public int HighID
        {
            get
            {
                return templateHigh.ID;
            }
        }

        public int Nothing
        {
            get
            {
                return templateLow.Nothing;
            }
        }

        public int MultipleCount
        {
            get
            {
                // Return MaxEnergy = MultipleCount
                return this.GetAttribute(212);
            }
        }

        public int Flags
        {
            get
            {
                return templateLow.Flags;
            }
        }

        public void WriteToDatabase()
        {

        }

        public Item(int QL, int lowID, int highID)
        {
            // Checks:
            if ((!ItemLoader.ItemList.ContainsKey(lowID)) || (!ItemLoader.ItemList.ContainsKey(highID)))
            {
                throw new ArgumentOutOfRangeException("No Item found with ID " + lowID);
            }
            this.templateLow = ItemLoader.ItemList[lowID];
            this.templateHigh = ItemLoader.ItemList[highID];
            this.Quality = QL < this.templateLow.Quality
                          ? this.templateLow.Quality
                          : (QL > this.templateHigh.Quality ? this.templateHigh.Quality : QL);
        }
    }
}