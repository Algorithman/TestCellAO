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
    using System.Collections;
    using System.Collections.Generic;

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// ALL SLOT NUMBERS ARE OUTSIDE SLOT NUMBERS
    /// means for standard inventory slotnumber would be 0x40 - 0x5e
    /// All Calculation is made INSIDE, dont bother
    /// </summary>
    public abstract class PartialInventory : IInventory
    {
        /// <summary>
        /// </summary>
        protected EntityInventory inventory;

        /// <summary>
        /// </summary>
        /// <param name="baseInventory">
        /// </param>
        protected PartialInventory(EntityInventory baseInventory)
        {
            this.inventory = baseInventory;
        }

        /// <summary>
        /// </summary>
        public Dynel Owner
        {
            get
            {
                return this.inventory.Owner;
            }
        }

        /// <summary>
        /// </summary>
        public IItem[] Content { get; private set; }

        /// <summary>
        /// ONLY SET IT WHEN INVENTORY IS EMPTY
        /// </summary>
        public int MaxCount
        {
            get
            {
                return this.Content.Length;
            }
            set
            {
                this.Content = new Item[value];
            }
        }

        /// <summary>
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                foreach (Item it in this.Content)
                {
                    if (it != null)
                    {
                        return false;
                    }
                }
                return true;
            }
            private set
            {
                for (int i = 0; i < this.Content.Length; i++)
                {
                    this.Content[i] = null;
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool IsFull
        {
            get
            {
                foreach (Item it in this.Content)
                {
                    if (it == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            private set { }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public int FindFreeSlot()
        {
            int slot = 0;
            foreach (Item it in Content)
            {
                if (it == null)
                {
                    return slot + FirstSlotNumber;
                }
                slot++;
            }
            return -1;
        }

        /// <summary>
        /// </summary>
        public virtual int FirstSlotNumber { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual bool IsValidSlot(int slot)
        {
            // Subtract offset (firstslotnumber) then check
            // Apply other checks for subclasses
            return slot - FirstSlotNumber < Content.Length;
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <param name="isNew">
        /// </param>
        /// <param name="reception">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual InventoryError TryAdd(int slot, Item item, bool isNew, ItemReceptionType reception)
        {
            if (slot - FirstSlotNumber >= Content.Length) return InventoryError.Invalid;
            if (Content[slot - FirstSlotNumber] != null) return InventoryError.Invalid;

            Content[slot - FirstSlotNumber] = item;
            return InventoryError.OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <param name="isNew">
        /// </param>
        /// <param name="reception">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public InventoryError TryAdd(Item item, bool isNew, ItemReceptionType reception)
        {
            return TryAdd(this.FindFreeSlot(), item, isNew, reception);
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="ownerChange">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IItem Remove(int slot, bool ownerChange)
        {
            IItem temp = null;
            if (ownerChange)
            {
                temp = this.Content[slot];
            }
            this.Content[slot - FirstSlotNumber] = null;
            return temp;
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Destroy(int slot)
        {
            this.Content[slot - FirstSlotNumber] = null;
            // TODO: Need to check if AO has non-destroyable Items
            return true;
        }

        public List<IItem> List()
        {
            List<IItem> temp = new List<IItem>();
            foreach (IItem it in this.Content)
            {
                if (it != null)
                {
                    temp.Add(it);
                }
            }
            return temp;
        }

        public void InitializeInventory(int slots, int firstSlot)
        {
            Content = new Item[slots];
            FirstSlotNumber = firstSlot;
        }
    }
}