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

namespace ZoneEngine.GameObject.Items.Inventory
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    using AO.Database;
    using AO.Database.Dao;
    using AO.Database.Entities;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public abstract class BaseInventoryPage : IInventoryPage
    {
        /// <summary>
        /// </summary>
        private readonly IDictionary<int, IItem> Content;

        /// <summary>
        /// </summary>
        public Identity Identity { get; set; }

        /// <summary>
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// </summary>
        public int MaxSlots { get; set; }

        /// <summary>
        /// </summary>
        public int FirstSlotNumber { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual InventoryError Add(int slot, IItem item)
        {
            if (this.Content.ContainsKey(slot))
            {
                throw new ArgumentException("Already item in slot " + slot + " of container " + this.Identity.Type + ":" + this.Identity.Instance);
            }
            this.Content.Add(slot, item);
            return InventoryError.OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="slotNum">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IItem Remove(int slotNum)
        {
            if (!this.Content.ContainsKey(slotNum))
            {
                throw new ArgumentOutOfRangeException("No item in slot "+slotNum+" of container "+this.Identity.Type+":"+this.Identity.Instance);
            }
            IItem temp = this.Content[slotNum];
            return temp;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Read()
        {
            foreach (DBItem item in ItemDao.GetAllInContainer((int)this.Identity.Type, this.Identity.Instance))
            {
                Item newItem = new Item(item.quality, item.lowid, item.highid);
                newItem.SetAttribute(212, item.multiplecount);
                this.Content.Add(item.containerplacement, newItem);
            }

            foreach (DBInstancedItem item in
                InstancedItemDao.GetAllInContainer((int)this.Identity.Type, this.Identity.Instance))
            {
                Item newItem = new Item(item.quality, item.lowid, item.highid);
                newItem.SetAttribute(212, item.multiplecount);
                Identity temp = new Identity();
                temp.Type = (IdentityType)item.itemtype;
                temp.Instance = item.iteminstance;
                newItem.Identity = temp;

                byte[] binaryStats = item.stats.ToArray();
                for (int i = 0; i < binaryStats.Length / 8; i++)
                {
                    int statid = BitConverter.ToInt32(binaryStats, i * 8);
                    int statvalue = BitConverter.ToInt32(binaryStats, i * 8 + 4);
                    newItem.SetAttribute(statid, statvalue);
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Write()
        {
            List<DBInstancedItem> DBinstanced = new List<DBInstancedItem>();
            List<DBItem> DBuninstanced = new List<DBItem>();
            foreach (KeyValuePair<int, IItem> kv in this.Content)
            {
                if (kv.Value.Identity.Type != IdentityType.None)
                {
                    DBInstancedItem dbi = new DBInstancedItem
                                              {
                                                  containerinstance = this.Identity.Instance, 
                                                  containertype = (int)this.Identity.Type, 
                                                  containerplacement = kv.Key, 
                                                  itemtype = (int)kv.Value.Identity.Type, 
                                                  iteminstance = kv.Value.Identity.Instance, 
                                                  lowid = kv.Value.LowID, 
                                                  highid = kv.Value.HighID, 
                                                  quality = kv.Value.Quality, 
                                                  multiplecount = kv.Value.MultipleCount, 
                                                  stats = new Binary(kv.Value.GetItemAttributes())
                                              };

                    DBinstanced.Add(dbi);
                }
                else
                {
                    DBItem dbi = new DBItem
                                     {
                                         containerinstance = this.Identity.Instance, 
                                         containertype = (int)this.Identity.Type, 
                                         containerplacement = kv.Key, 
                                         lowid = kv.Value.LowID, 
                                         highid = kv.Value.HighID, 
                                         quality = kv.Value.Quality, 
                                         multiplecount = kv.Value.MultipleCount
                                     };

                    DBuninstanced.Add(dbi);
                }
            }

            ItemDao.Save(DBuninstanced);
            InstancedItemDao.Save(DBinstanced);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <returns>
        /// </returns>
        public IItem this[int index]
        {
            get
            {
                if (this.Content.ContainsKey(index))
                {
                    return this.Content[index];
                }

                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="pagenum">
        /// </param>
        /// <param name="maxslots">
        /// </param>
        /// <param name="firstslotnumber">
        /// </param>
        public BaseInventoryPage(int pagenum, int maxslots, int firstslotnumber)
        {
            this.Page = pagenum;
            this.MaxSlots = maxslots;
            this.FirstSlotNumber = firstslotnumber;
            this.Content = new Dictionary<int, IItem>();
        }
    }
}