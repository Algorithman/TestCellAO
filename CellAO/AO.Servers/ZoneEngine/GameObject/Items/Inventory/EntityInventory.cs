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

    using ZoneEngine.GameObject.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public class EntityInventory : BaseInventory
    {
        /// <summary>
        /// </summary>
        internal Dynel Owner;

        /// <summary>
        /// </summary>
        internal ItemTemplate Ammunition;

        /// <summary>
        /// </summary>
        public PartialInventory[] MainInventory;

        /// <summary>
        /// </summary>
        /// <param name="innerSlot">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public static int TranslateToAOContainers(int innerSlot)
        {
            switch (innerSlot)
            {
                case 0:
                    return 0x65; // WeaponPage
                case 1:
                    return 0x66; // ArmorPage
                case 2:
                    return 0x67; // ImplantPage
                case 3:
                    return 0x68; // Inventory (places 64-93, TODO: Subject to Change)
                case 4:
                    return 0x69; // Bank
                case 5:
                    return 0x6B; // Backpack? No Idea
                case 6:
                    return 0x6C; // KnuBot Trade Window
                case 7:
                    return 0x6E; // Overflow Window
                case 8:
                    return 0x6F; // Trade window
                case 9:
                    return 0x73; // Social Page
                case 10:
                    return 0x767; // Shop Inventory
                case 11:
                    return 0x790; // Playershop Inventory
                case 12:
                    return 0xDEAD; // Trade window (incoming from player<>player trade)
                default:
                    throw new ArgumentOutOfRangeException("No suitable Containernumber found");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="AOSlot">
        /// </param>
        /// <returns>
        /// </returns>
        public static int TranslateFromAOContainers(int AOSlot)
        {
            switch (AOSlot)
            {
                case 0x65:
                    return 0;
                case 0x66:
                    return 1;
                case 0x67:
                    return 2;
                case 0x68:
                    return 3;
                case 0x69:
                    return 4;
                case 0x6B:
                    return 5;
                case 0x6C:
                    return 6;
                case 0x6E:
                    return 7;
                case 0x6F:
                    return 8;
                case 0x73:
                    return 9;
                case 0x767:
                    return 10;
                case 0x790:
                    return 11;
                case 0xDEAD:
                    return 12;
                default:
                    throw new ArgumentOutOfRangeException("No suitable Containernumber found");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dynel">
        /// </param>
        public EntityInventory(IItemContainer dynel) // 13 = number of subInventories (Weaponpage to Trade Window)
            : base(dynel, 0, 0)
        {
            this.MainInventory = new PartialInventory[13];
            this.MainInventory[0] = new WeaponPageInventory(this);
            this.MainInventory[1] = new ArmorPageInventory(this);
            this.MainInventory[2] = new ImplantPageInventory(this);
            this.MainInventory[3] = new PlayerInventory(this);
            this.MainInventory[4] = new BankInventory(this);
            this.MainInventory[5] = new BackPackInventory(this); // Cached Bags, refering to data sent by inventoryupdate packet, we need a handler for this
            this.MainInventory[6] = new KnuBotTradeInventory(this); // Selling inventory?
            this.MainInventory[7] = new OverflowInventory(this);
            this.MainInventory[8] = new OutgoingTradeInventory(this);
            this.MainInventory[9] = new SocialArmorPageInventory(this);
            this.MainInventory[10] = new ShopInventory(this); // Linker to Shop's Inventory
            this.MainInventory[11] = new PlayerShopInventory(this); // Linker to Player's Shop inventory
            this.MainInventory[12] = new IncomingTradeInventory(this); // DEAD window (stuff which will be put into overflow when needed?)
        }

        /// <summary>
        /// </summary>
        public IInventory WeaponPage
        {
            get
            {
                return this.MainInventory[0];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory ArmorPage
        {
            get
            {
                return this.MainInventory[1];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory ImplantPage
        {
            get
            {
                return this.MainInventory[2];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory InventoryPage
        {
            get
            {
                return this.MainInventory[3];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory BankPage
        {
            get
            {
                return this.MainInventory[4];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory BackPackPage
        {
            get
            {
                return this.MainInventory[5];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory KnuBotTradePage
        {
            get
            {
                return this.MainInventory[6];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory OverflowPage
        {
            get
            {
                return this.MainInventory[7];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory TradeWindowOutgoingPage
        {
            get
            {
                return this.MainInventory[8];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory SocialArmorPage
        {
            get
            {
                return this.MainInventory[9];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory ShopInventoryPage
        {
            get
            {
                return this.MainInventory[10];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory PlayerShopPage
        {
            get
            {
                return this.MainInventory[11];
            }
        }

        /// <summary>
        /// </summary>
        public IInventory TradeWindowIncomingPage
        {
            get
            {
                return this.MainInventory[12];
            }
        }

        /* Container ID's:
         * 0065 Weaponpage
         * 0066 Armorpage
         *  0067 Implantpage
         *  0068 Inventory (places 64-93)
         *  0069 Bank
         *  006B Backpack
         *  006C KnuBot Trade Window
         *  006E Overflow window
         *  006F Trade Window
         *  0073 Socialpage
         *  0767 Shop Inventory
         *  0790 Playershop Inventory
         *  DEAD Trade Window (incoming)
         */

        public override InventoryError TryAdd(Item it)
        {
            // If no special page is given, lets assume normal Backpack inventory
            return BackPackPage.TryAdd(it, true, ItemReceptionType.Receive);
        }
    }
}