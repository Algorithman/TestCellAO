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
        /// <returns>
        /// </returns>
        public IEnumerator<AOItem> GetEnumerator()
        {
            foreach (AOItem item in this.Content)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Add(AOItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Contains(AOItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="array">
        /// </param>
        /// <param name="arrayIndex">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CopyTo(AOItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Remove(AOItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public int IndexOf(AOItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Insert(int index, AOItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        /// <returns>
        /// </returns>
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

        /// <summary>
        /// </summary>
        public AOItem[] Content { get; private set; }

        /// <summary>
        /// </summary>
        public int MaxCount { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsFull { get; private set; }

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
        public virtual int FirstSlotNumber { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool IsValidSlot(int Slot)
        {
            throw new NotImplementedException();
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
        public InventoryError TryAdd(int slot, AOItem item, bool isNew, ItemReceptionType reception)
        {
            throw new NotImplementedException();
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
        public InventoryError TryAdd(AOItem item, bool isNew, ItemReceptionType reception)
        {
            throw new NotImplementedException();
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
        public AOItem Remove(int slot, bool ownerChange)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void InitializeInventory(int slots, int firstSlot)
        {
            Content = new AOItem[slots];
            FirstSlotNumber = firstSlot;
        }
    }
}