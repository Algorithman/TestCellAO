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

    #endregion

    /// <summary>
    /// </summary>
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