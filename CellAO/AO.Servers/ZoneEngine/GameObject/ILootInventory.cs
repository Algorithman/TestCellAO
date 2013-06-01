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

namespace ZoneEngine.GameObject
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using ZoneEngine.GameObject.Items;

    #endregion

    /// <summary>
    /// </summary>
    [ContractClass(typeof(ILootInventoryContract))]
    public interface ILootInventory
    {
        #region Public Properties

        /// <summary>
        /// </summary>
        IList<Item> Inventory { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="lootTable">
        /// </param>
        /// <returns>
        /// </returns>
        bool FillInventory(LootTable lootTable);

        #endregion
    }

    /// <summary>
    /// </summary>
    [ContractClassFor(typeof(ILootInventory))]
    internal abstract class ILootInventoryContract : ILootInventory
    {
        /// <summary>
        /// </summary>
        public IList<Item> Inventory
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<ItemTemplate>>() != null);
                return default(IList<Item>);
            }

            private set
            {
                Contract.Requires(value != null);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="lootTable">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool FillInventory(LootTable lootTable)
        {
            throw new NotImplementedException();
        }
    }
}