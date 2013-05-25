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
    using System.Collections;
    using System.Collections.Generic;

    using ZoneEngine.GameObject.Enums;
    using ZoneEngine.GameObject.Items.Inventory;

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
        public AOItem[] Content { get; private set; }

        /// <summary>
        /// </summary>
        public int MaxSlots { get; private set; }

        private int firstSlotNumber;

        public Dynel Owner { get; private set; }

        public int MaxCount { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsFull { get; private set; }

        /// <summary>
        /// </summary>
        public BaseInventory()
        {
            this.MaxSlots = 0;
            this.FirstSlotNumber = 0;
        }

        protected BaseInventory(IItemContainer owner, AOItem[] items, int firstSlotNumber)
            : this()
        {
            this.owner = owner;
            this.Content = items;
            this.MaxSlots = items.Length;
            this.FirstSlotNumber = firstSlotNumber;
        }

        /// <summary>
        /// Create a EMPTY base inventory
        /// </summary>
        /// <param name="owner">
        /// </param>
        public BaseInventory(IItemContainer owner, int maxSlots, int firstSlotNumber)
            : this()
        {
            this.owner = owner;
            this.MaxSlots = maxSlots;
            this.Content = new AOItem[maxSlots];
            this.FirstSlotNumber = firstSlotNumber;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public int FindFreeSlot()
        {
            int slot = FirstSlotNumber;
            bool foundFreeSlot = false;
            foreach (AOItem item in Content)
            {
                if (item == null)
                {
                    foundFreeSlot = true;
                    break;
                }
                slot++;
            }
            if (!foundFreeSlot)
            {
                // no free slot found
                return -1;
            }
            return slot;
        }

        public virtual int FirstSlotNumber
        {
            get
            {
                return this.firstSlotNumber;
            }
            set
            {
                firstSlotNumber = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsValidSlot(int Slot)
        {
            return (Slot < this.MaxSlots + this.FirstSlotNumber) && (Slot >= this.FirstSlotNumber);
        }

        public InventoryError TryAdd(int slot, AOItem item, bool isNew, ItemReceptionType reception)
        {
            throw new NotImplementedException();
        }

        public InventoryError TryAdd(AOItem item, bool isNew, ItemReceptionType reception)
        {
            throw new NotImplementedException();
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
            // TODO: Check for Hotswapping
            if (Content[Slot] == null)
            {
                Content[Slot] = Item;
                return InventoryError.OK;
            }
            return InventoryError.Invalid;
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
            int nextFreeSlot = this.FindFreeSlot();
            if (nextFreeSlot == -1)
            {
                return InventoryError.Full;
            }
            Content[nextFreeSlot] = Item;
            return InventoryError.OK;
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
                this.Content[Slot] = null;
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
                this.Content[Slot] = null;
                return true;
            }

            return false;
        }

        public IEnumerator<AOItem> GetEnumerator()
        {
            for (var i = 0; i < Content.Length; i++)
            {
                var item = Content[i];
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(AOItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(AOItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(AOItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(AOItem item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }

        public int IndexOf(AOItem item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, AOItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public AOItem this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}