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

    using System.Collections.Generic;

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// </summary>
        Character owner { get; }

        /// <summary>
        /// </summary>
        Dictionary<int, AOItem> Content { get; }

        /// <summary>
        /// </summary>
        int MaxSlots { get; }

        /// <summary>
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        int FindFreeSlot();

        /// <summary>
        /// </summary>
        int InventoryOffset { get; }

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <returns>
        /// </returns>
        bool IsValidSlot(int Slot);

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <param name="Item">
        /// </param>
        /// <returns>
        /// </returns>
        InventoryError TryAdd(int Slot, AOItem Item);

        /// <summary>
        /// </summary>
        /// <param name="Item">
        /// </param>
        /// <returns>
        /// </returns>
        InventoryError TryAdd(AOItem Item);

        /// <summary>
        /// </summary>
        /// <param name="Slot">
        /// </param>
        /// <param name="ownerChange">
        /// </param>
        /// <returns>
        /// </returns>
        AOItem Remove(int Slot, bool ownerChange);

        /// <summary>
        /// Destroys AOItem at slot <see cref="slot"/>
        /// </summary>
        /// <param name="Slot">
        /// Slot number of the Item
        /// </param>
        /// <returns>
        /// Item could be destroyed
        /// </returns>
        bool Destroy(int Slot);
    }
}