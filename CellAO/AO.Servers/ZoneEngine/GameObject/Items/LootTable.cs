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

namespace ZoneEngine.GameObject.Items
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using ZoneEngine.Gameobject.Items;

    #endregion

    /// <summary>
    /// </summary>
    public class LootTable
    {
        #region Fields

        /// <summary>
        /// </summary>
        private List<AOItem> Items;

        /// <summary>
        /// </summary>
        private readonly List<LootTableEntry> LootTableEntries;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public LootTable()
        {
            this.LootTableEntries = new List<LootTableEntry>();
            this.Items = new List<AOItem>();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public List<AOItem> GetLoot()
        {
            if (this.Items == null)
            {
                Random probability = new Random((int)DateTime.Now.Ticks);
                List<AOItem> temp = new List<AOItem>();
                foreach (LootTableEntry lootTableEntry in this.LootTableEntries)
                {
                    if (probability.Next() < lootTableEntry.DropRate)
                    {
                        int QL =
                            Convert.ToInt32(
                                probability.Next() * (lootTableEntry.MaxQL - lootTableEntry.MinQL)
                                + lootTableEntry.MinQL);
                        AOItem tempitem = new AOItem(QL, lootTableEntry.LowID, lootTableEntry.HighID);
                        temp.Add(tempitem);
                    }
                }

                this.Items = temp;
            }

            return this.Items;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="hash">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void AddByHash(string hash)
        {
            // Get access to DB/or file and read the appropriate stuff for the hash
            // Either a AOItem with QL Range (LootTableEntry), or a Hash Group (which lets AddByHash call itself again)
            Contract.Requires(hash != null);
            Contract.Requires(hash != string.Empty);
            throw new NotImplementedException("LootTable.AddByHash");
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    internal class LootTableEntry
    {
        #region Fields

        /// <summary>
        /// </summary>
        public float DropRate = 1.0f;

        /// <summary>
        /// </summary>
        public int HighID = 0;

        /// <summary>
        /// </summary>
        public int LowID = 0;

        /// <summary>
        /// </summary>
        public int MaxQL = 1;

        /// <summary>
        /// </summary>
        public int MinQL = 1;

        #endregion
    }
}