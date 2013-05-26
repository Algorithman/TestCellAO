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

namespace ZoneEngine.GameObject.Nanos
{
    #region Usings ...

    using System;
    using System.Collections.Generic;

    using ZoneEngine.GameObject.Items;

    #endregion

    /// <summary>
    /// NanoFormula serializable class
    /// </summary>
    [Serializable]
    public class NanoFormula
    {
        /// <summary>
        /// Item Flags
        /// </summary>
        public int flags;

        /// <summary>
        /// Nano ID
        /// </summary>
        public int ID;

        /// <summary>
        /// NCUCost
        /// </summary>
        public int NCUCost()
        {
            return this.getItemAttribute(54);
        }

        /// <summary>
        /// Type of instanced item
        /// </summary>
        public int Type;

        /// <summary>
        /// Instance of instanced item
        /// </summary>
        public int Instance;

        /// <summary>
        /// Item type
        /// </summary>
        public int ItemType;

        /// <summary>
        /// Item attributes
        /// </summary>
        public Dictionary<int, int> Stats = new Dictionary<int, int>();

        /// <summary>
        /// List of Attack attributes
        /// </summary>
        public Dictionary<int, int> Attack = new Dictionary<int, int>();

        /// <summary>
        /// List of defensive attributes
        /// </summary>
        public Dictionary<int, int> Defend = new Dictionary<int, int>();

        /// <summary>
        /// List of Item events
        /// </summary>
        public List<AOEvents> Events = new List<AOEvents>();

        /// <summary>
        /// List of Item Actions (requirement checks)
        /// </summary>
        public List<AOActions> Actions = new List<AOActions>();

        /// <summary>
        /// </summary>
        /// Methods to do:
        /// Read Item
        /// Write Item
        /// Return Dynel Item (placing on the ground)
        /// <returns>
        /// </returns>
        public NanoFormula ShallowCopy()
        {
            NanoFormula nanoFormula = new NanoFormula();
            nanoFormula.ID = this.ID;

            foreach (KeyValuePair<int, int> nanoFormulaAttribute in this.Attack)
            {
                nanoFormula.Attack.Add(nanoFormulaAttribute.Key, nanoFormulaAttribute.Value);
            }

            foreach (KeyValuePair<int, int> nanoFormulaAttribute in this.Defend)
            {
                nanoFormula.Defend.Add(nanoFormulaAttribute.Key, nanoFormulaAttribute.Value);
            }

            foreach (KeyValuePair<int, int> nanoFormulaAttribute in this.Stats)
            {
                nanoFormula.Stats.Add(nanoFormulaAttribute.Key, nanoFormulaAttribute.Value);
            }

            foreach (AOEvents aoEvents in this.Events)
            {
                AOEvents newEvent = new AOEvents();
                foreach (AOFunctions aoFunctions in aoEvents.Functions)
                {
                    AOFunctions newAOFunctions = new AOFunctions();
                    foreach (AORequirements aor in aoFunctions.Requirements)
                    {
                        AORequirements newRequirement = new AORequirements();
                        newRequirement.ChildOperator = aor.ChildOperator;
                        newRequirement.Operator = aor.Operator;
                        newRequirement.Statnumber = aor.Statnumber;
                        newRequirement.Target = aor.Target;
                        newRequirement.Value = aor.Value;
                        newAOFunctions.Requirements.Add(newRequirement);
                    }

                    foreach (object argument in aoFunctions.Arguments.Values)
                    {
                        if (argument.GetType() == typeof(string))
                        {
                            string z = (string)argument;
                            newAOFunctions.Arguments.Values.Add(z);
                        }

                        if (argument.GetType() == typeof(int))
                        {
                            int i = (int)argument;
                            newAOFunctions.Arguments.Values.Add(i);
                        }

                        if (argument.GetType() == typeof(Single))
                        {
                            float s = (Single)argument;
                            newAOFunctions.Arguments.Values.Add(s);
                        }
                    }

                    newAOFunctions.dolocalstats = aoFunctions.dolocalstats;
                    newAOFunctions.FunctionType = aoFunctions.FunctionType;
                    newAOFunctions.Target = aoFunctions.Target;
                    newAOFunctions.TickCount = aoFunctions.TickCount;
                    newAOFunctions.TickInterval = aoFunctions.TickInterval;
                    newEvent.Functions.Add(newAOFunctions);
                }

                newEvent.EventType = aoEvents.EventType;
                nanoFormula.Events.Add(newEvent);
            }

            nanoFormula.flags = this.flags;
            nanoFormula.Instance = this.Instance;
            nanoFormula.ItemType = this.ItemType;

            return nanoFormula;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool isInstanced()
        {
            if ((this.Type == 0) && (this.Instance == 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get item attribute
        /// </summary>
        /// <param name="number">
        /// number of attribute
        /// </param>
        /// <returns>
        /// Value of item attribute
        /// </returns>
        public int getItemAttribute(int number)
        {
            if (Stats.ContainsKey(number))
            {
                return Stats[number];
            }
            return 0;
        }

        /// <summary>
        /// Nano strain
        /// </summary>
        /// <returns>
        /// </returns>
        public int NanoStrain()
        {
            return this.getItemAttribute(75);
        }
    }
}