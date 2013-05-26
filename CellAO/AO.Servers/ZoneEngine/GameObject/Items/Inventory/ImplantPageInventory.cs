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

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public class ImplantPageInventory : PartialInventory, IItemSlotHandler, IItemHotSwapHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="baseInventory">
        /// </param>
        public ImplantPageInventory(EntityInventory baseInventory)
            : base(baseInventory)
        {
            this.InitializeInventory(15, 0x21);
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <param name="err">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CheckAdd(int slot, AOItemTemplate item, ref InventoryError err)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="templ">
        /// </param>
        /// <param name="err">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CheckRemove(int slot, AOItemTemplate templ, ref InventoryError err)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Added(AOItemTemplate item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="slot">
        /// </param>
        /// <param name="item">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Removed(int slot, AOItemTemplate item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="slotFrom">
        /// </param>
        /// <param name="slotTo">
        /// </param>
        /// <param name="err">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void TryHotSwap(int slotFrom, int slotTo, ref InventoryError err)
        {
            throw new NotImplementedException();
        }
    }
}