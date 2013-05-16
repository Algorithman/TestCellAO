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

namespace ZoneEngine.GameObject.Items
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>
    /// </summary>
    [Serializable]
    public class AOItem
    {
        #region Fields

        /// <summary>
        /// List of Item Actions (requirement checks)
        /// </summary>
        public List<AOActions> Actions = new List<AOActions>();

        /// <summary>
        /// List of Attack attributes
        /// </summary>
        public List<AOItemAttribute> Attack = new List<AOItemAttribute>();

        /// <summary>
        /// List of defensive attributes
        /// </summary>
        public List<AOItemAttribute> Defend = new List<AOItemAttribute>();

        /// <summary>
        /// List of Item events
        /// </summary>
        public List<AOEvents> Events = new List<AOEvents>();

        /// <summary>
        /// Item Flags
        /// </summary>
        public int Flags;

        /// <summary>
        /// Item Flags
        /// </summary>
        public int CanFlags;

        /// <summary>
        /// Item high ID
        /// </summary>
        public int HighID;

        /// <summary>
        /// Instance of instanced item
        /// </summary>
        public int Instance;

        /// <summary>
        /// Item type
        /// </summary>
        public int ItemType;

        /// <summary>
        /// Item low ID
        /// </summary>
        public int LowID;

        /// <summary>
        /// Stacked item count
        /// </summary>
        public int MultipleCount;

        /// <summary>
        /// dunno yet
        /// </summary>
        public int Nothing;

        /// <summary>
        /// Quality level
        /// </summary>
        public int Quality;

        /// <summary>
        /// Item attributes
        /// </summary>
        public List<AOItemAttribute> Stats = new List<AOItemAttribute>();

        /// <summary>
        /// Type of instanced item
        /// </summary>
        public int Type;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanCarry()
        {
            return (this.CanFlags & (1 << 0)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanSit()
        {
            return (this.CanFlags & (1 << 1)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Wearable()
        {
            return (this.CanFlags & (1 << 2)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool Useable()
        {
            return (this.CanFlags & (1 << 3)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool ConfirmOnUse()
        {
            return (this.CanFlags & (1 << 4)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsConsumable()
        {
            return (this.CanFlags & (1 << 5)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsTutorChip()
        {
            return (this.CanFlags & (1 << 6)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsTutorDevie()
        {
            return (this.CanFlags & (1 << 7)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsLockPicker()
        {
            return (this.CanFlags & (1 << 8)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsStackable()
        {
            return (this.CanFlags & (1 << 9)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool NeedsNoAmmo()
        {
            return (this.CanFlags & (1 << 10)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBurst()
        {
            return (this.CanFlags & (1 << 11)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanFlingShot()
        {
            return (this.CanFlags & (1 << 12)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanFullAuto()
        {
            return (this.CanFlags & (1 << 13)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanAimedShot()
        {
            return (this.CanFlags & (1 << 14)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBow()
        {
            return (this.CanFlags & (1 << 15)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanThrowAttack()
        {
            return (this.CanFlags & (1 << 16)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanSneakAttack()
        {
            return (this.CanFlags & (1 << 17)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanFastAttack()
        {
            return (this.CanFlags & (1 << 18)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanDisarmTraps()
        {
            return (this.CanFlags & (1 << 19)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool DoesAutoSelect()
        {
            return (this.CanFlags & (1 << 20)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeAppliedOnFriendly()
        {
            return (this.CanFlags & (1 << 21)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeAppliedOnHostile()
        {
            return (this.CanFlags & (1 << 22)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeAppliedOnSelf()
        {
            return (this.CanFlags & (1 << 23)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CantBeSplit()
        {
            return (this.CanFlags & (1 << 24)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBrawl()
        {
            return (this.CanFlags & (1 << 25)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanDimach()
        {
            return (this.CanFlags & (1 << 26)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool EnablesHandAttractors()
        {
            return (this.CanFlags & (1 << 27)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeWornWithSocialArmor()
        {
            return (this.CanFlags & (1 << 28)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanParryRiposte()
        {
            return (this.CanFlags & (1 << 29)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeParriedRiposted()
        {
            return (this.CanFlags & (1 << 30)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeAppliedOnFightingTarget()
        {
            return (this.CanFlags & (1 << 31)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsVisible()
        {
            return (this.Flags & (1 << 0)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasModifiedDescription()
        {
            return (this.Flags & (1 << 1)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasModifiedName()
        {
            return (this.Flags & (1 << 2)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeTemplateItem()
        {
            return (this.Flags & (1 << 3)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool TurnsOnUse()
        {
            return (this.Flags & (1 << 4)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasMultipleCount()
        {
            return (this.Flags & (1 << 5)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsLocked()
        {
            return (this.Flags & (1 << 6)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsOpen()
        {
            return (this.Flags & (1 << 7)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsItemSocialArmor()
        {
            return (this.Flags & (1 << 8)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool TellCollision()
        {
            return (this.Flags & (1 << 9)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool NoSelectionIndicator()
        {
            return (this.Flags & (1 << 10)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool UseEmptyDestruct()
        {
            return (this.Flags & (1 << 11)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsStaionary()
        {
            return (this.Flags & (1 << 12)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsRepulsive()
        {
            return (this.Flags & (1 << 13)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsDefaultTarget()
        {
            return (this.Flags & (1 << 14)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool OverrideItemTexture()
        {
            return (this.Flags & (1 << 15)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsNull()
        {
            return (this.Flags & (1 << 16)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasAnimation()
        {
            return (this.Flags & (1 << 17)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasRotation()
        {
            return (this.Flags & (1 << 18)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool WantsCollision()
        {
            return (this.Flags & (1 << 19)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool WantsSignals()
        {
            return (this.Flags & (1 << 20)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasSentFirstIIR()
        {
            return (this.Flags & (1 << 21)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasEnergy()
        {
            return (this.Flags & (1 << 22)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool MirrorsInLeftHand()
        {
            return (this.Flags & (1 << 23)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IllegalForClan()
        {
            return (this.Flags & (1 << 24)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IllegalForOmni()
        {
            return (this.Flags & (1 << 25)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsNoDrop()
        {
            return (this.Flags & (1 << 26)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsUnique()
        {
            return (this.Flags & (1 << 27)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool CanBeAttacked()
        {
            return (this.Flags & (1 << 28)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool FallingDisabled()
        {
            return (this.Flags & (1 << 29)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool HasDamage()
        {
            return (this.Flags & (1 << 30)) > 0;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool StatelCollisionDisabled()
        {
            return (this.Flags & (1 << 31)) > 0;
        }

        /// <summary>
        /// </summary>
        /// Methods to do:
        /// Read Item
        /// Write Item
        /// Return Dynel Item (placing on the ground)
        /// <returns>
        /// </returns>
        public AOItem ShallowCopy()
        {
            AOItem itemCopy = new AOItem();
            itemCopy.LowID = this.LowID;
            itemCopy.HighID = this.HighID;

            foreach (AOItemAttribute aoItemAttribute in this.Attack)
            {
                AOItemAttribute itemAttribute = new AOItemAttribute();
                itemAttribute.Stat = aoItemAttribute.Stat;
                itemAttribute.Value = aoItemAttribute.Value;
                itemCopy.Attack.Add(itemAttribute);
            }

            foreach (AOItemAttribute aoItemAttribute in this.Defend)
            {
                AOItemAttribute itemAttribute = new AOItemAttribute();
                itemAttribute.Stat = aoItemAttribute.Stat;
                itemAttribute.Value = aoItemAttribute.Value;
                itemCopy.Defend.Add(itemAttribute);
            }

            foreach (AOItemAttribute aoItemAttribute in this.Stats)
            {
                AOItemAttribute itemAttribute = new AOItemAttribute();
                itemAttribute.Stat = aoItemAttribute.Stat;
                itemAttribute.Value = aoItemAttribute.Value;
                itemCopy.Stats.Add(itemAttribute);
            }

            foreach (AOEvents aoEvents in this.Events)
            {
                AOEvents newEvent = new AOEvents();
                foreach (AOFunctions aoFunctions in aoEvents.Functions)
                {
                    AOFunctions newAOFunction = new AOFunctions();
                    foreach (AORequirements aoRequirements in aoFunctions.Requirements)
                    {
                        AORequirements newAORequirement = new AORequirements();
                        newAORequirement.ChildOperator = aoRequirements.ChildOperator;
                        newAORequirement.Operator = aoRequirements.Operator;
                        newAORequirement.Statnumber = aoRequirements.Statnumber;
                        newAORequirement.Target = aoRequirements.Target;
                        newAORequirement.Value = aoRequirements.Value;
                        newAOFunction.Requirements.Add(newAORequirement);
                    }

                    foreach (object ob in aoFunctions.Arguments.Values)
                    {
                        if (ob.GetType() == typeof(string))
                        {
                            string z = (string)ob;
                            newAOFunction.Arguments.Values.Add(z);
                        }

                        if (ob.GetType() == typeof(int))
                        {
                            int i = (int)ob;
                            newAOFunction.Arguments.Values.Add(i);
                        }

                        if (ob.GetType() == typeof(Single))
                        {
                            float s = (Single)ob;
                            newAOFunction.Arguments.Values.Add(s);
                        }
                    }

                    newAOFunction.dolocalstats = aoFunctions.dolocalstats;
                    newAOFunction.FunctionType = aoFunctions.FunctionType;
                    newAOFunction.Target = aoFunctions.Target;
                    newAOFunction.TickCount = aoFunctions.TickCount;
                    newAOFunction.TickInterval = aoFunctions.TickInterval;
                    newEvent.Functions.Add(newAOFunction);
                }

                newEvent.EventType = aoEvents.EventType;
                itemCopy.Events.Add(newEvent);
            }

            itemCopy.Flags = this.Flags;
            itemCopy.CanFlags = this.CanFlags;
            itemCopy.Instance = this.Instance;
            itemCopy.ItemType = this.ItemType;
            itemCopy.MultipleCount = this.MultipleCount;
            itemCopy.Nothing = this.Nothing;
            itemCopy.Quality = this.Quality;

            return itemCopy;
        }

        #endregion
    }
}