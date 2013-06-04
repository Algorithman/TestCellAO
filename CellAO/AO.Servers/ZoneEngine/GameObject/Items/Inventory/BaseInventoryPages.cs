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

namespace ZoneEngine.GameObject.Items.Inventory
{
    #region Usings ...

    using System;
    using System.Collections.Generic;

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public abstract class BaseInventoryPages : IInventoryPages
    {
        /// <summary>
        /// </summary>
        public IDictionary<int, IInventoryPage> Pages { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="pageNum">
        /// </param>
        /// <param name="slotNum">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public InventoryError AddToPage(int pageNum, int slotNum, IItem item)
        {
            if (!this.Pages.ContainsKey(pageNum))
            {
                throw new ArgumentOutOfRangeException("There is no inventorypage #" + pageNum);
            }

            this.Pages[pageNum].Add(slotNum, item);
            return InventoryError.OK;
        }

        /// <summary>
        /// </summary>
        public int StandardPage { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="pageNum">
        /// </param>
        /// <param name="slotNum">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public IItem RemoveItem(int pageNum, int slotNum)
        {
            if (!this.Pages.ContainsKey(pageNum))
            {
                throw new ArgumentOutOfRangeException("There is no inventorypage #" + pageNum);
            }

            return this.Pages[pageNum].Remove(slotNum);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Read()
        {
            foreach (IInventoryPage ip in this.Pages.Values)
            {
                ip.Read();
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Write()
        {
            foreach (IInventoryPage ip in this.Pages.Values)
            {
                ip.Write();
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <returns>
        /// </returns>
        public IInventoryPage this[int index]
        {
            get
            {
                if (!this.Pages.ContainsKey(index))
                {
                    throw new ArgumentOutOfRangeException("There is no inventorypage #" + index);
                }

                return this.Pages[index];
            }
        }

        /// <summary>
        /// </summary>
        public BaseInventoryPages()
        {
            this.Pages = new Dictionary<int, IInventoryPage>();
        }
    }
}