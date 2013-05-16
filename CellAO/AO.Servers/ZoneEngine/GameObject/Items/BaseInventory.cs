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

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public class BaseInventory : IInventory
    {
        /// <summary>
        /// </summary>
        public IItemContainer owner { get; private set; }

        /// <summary>
        /// </summary>
        public Dictionary<int, AOItem> Content { get; private set; }

        /// <summary>
        /// </summary>
        public int MaxSlots { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsFull { get; private set; }

        public BaseInventory()
        {
            this.Content=new Dictionary<int, AOItem>();
            this.MaxSlots = 0;
        }

        public BaseInventory(IItemContainer owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public int FindFreeSlot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public int InventoryOffset { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsValidSlot(int Slot)
        {
            return (Slot < this.MaxSlots + this.InventoryOffset) && (Slot >= this.InventoryOffset);
        }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <param name="Item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public InventoryError TryAdd(int Slot, AOItem Item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="Item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public InventoryError TryAdd(AOItem Item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <param name="ownerChange">
        /// </param>
        /// <returns>
        /// </returns>
        public AOItem Remove(int Slot, bool ownerChange)
        {
            if (this.IsValidSlot(Slot))
            {
                // TODO: Add more checks (eg. is equipped)
                AOItem item = this.Content[Slot];
                this.Content.Remove(Slot);
                return item;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Destroy(int Slot)
        {
            if (this.IsValidSlot(Slot))
            {
                // TODO: Add more checks (eg. is equipped)
                this.Content.Remove(Slot);
                return true;
            }

            return false;
        }
    }
}