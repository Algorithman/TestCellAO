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
    using System.Diagnostics.Contracts;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Function;
    using ZoneEngine.GameObject.Items;
    using ZoneEngine.GameObject.Playfields;
    using ZoneEngine.GameObject.Stats;

    #endregion

    /// <summary>
    /// </summary>
    public class Dynel : GameObject, IInstancedEntity, IStats
    {
        #region Fields

        /// <summary>
        /// </summary>
        public bool DoNotDoTimers = true;

        /// <summary>
        /// </summary>
        public bool Starting = true;

        /// <summary>
        /// </summary>
        protected DynelStats stats;

        /// <summary>
        /// </summary>
        protected IPlayfield playfield;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public Dynel()
        {
            this.stats = new DynelStats(this);

            // TODO: get correct playfield and set it
            // this.playfield = new Identity();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IPlayfield Playfield
        {
            get
            {
                return this.playfield;
            }

            set
            {
                this.playfield = value;
            }
        }

        /// <summary>
        /// </summary>
        private Vector3 coordinates = new Vector3();

        /// <summary>
        /// </summary>
        public Vector3 Coordinates
        {
            get
            {
                return this.coordinates;
            }

            set
            {
                this.coordinates = value;
            }
        }

        /// <summary>
        /// </summary>
        private Quaternion heading = new Quaternion();

        /// <summary>
        /// </summary>
        public Quaternion Heading
        {
            get
            {
                return this.heading;
            }

            set
            {
                this.heading = value;
            }
        }

        /// <summary>
        /// </summary>
        public DynelStats Stats
        {
            get
            {
                Contract.Ensures(this.stats != null);
                return this.stats;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="aof">
        /// </param>
        /// <param name="checkAll">
        /// </param>
        /// <returns>
        /// </returns>
        public bool CheckRequirements(AOFunctions aof, bool checkAll)
        {
            bool requirementsMet = true;
            int childOperator = -1; // Starting value
            bool foundCharRelated = (aof.FunctionType == Constants.FunctiontypeHairMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeBackMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeTexture)
                                    || (aof.FunctionType == Constants.FunctiontypeAttractorMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeCatMesh)
                                    || (aof.FunctionType == Constants.FunctiontypeChangeBodyMesh)
                                    || (aof.FunctionType == Constants.functiontype_shouldermesh)
                                    || (aof.FunctionType == Constants.FunctiontypeHeadMesh);

            Identity RequirementTargetIdentity = new Identity();
            foreach (AORequirements aor in aof.Requirements)
            {
                switch (aor.Target)
                {
                    case Constants.ItemtargetUser:
                        {
                            RequirementTargetIdentity = this.identity;
                            break;
                        }

                    case Constants.ItemtargetWearer:
                        {
                            // TODO: Subject to change, not sure about this one
                            RequirementTargetIdentity = this.identity;
                            break;
                        }

                    case Constants.ItemtargetTarget:
                        {
                            // TODO: pass on target 
                            break;
                        }

                    case Constants.ItemtargetFightingtarget:
                        {
                            var temp = this as ITargetingEntity;
                            if (temp != null)
                            {
                                RequirementTargetIdentity = temp.FightingTarget;
                            }

                            break;
                        }

                    case Constants.ItemtargetSelectedtarget:
                        {
                            var temp = this as ITargetingEntity;
                            if (temp != null)
                            {
                                RequirementTargetIdentity = temp.SelectedTarget;
                            }

                            break;
                        }

                    case Constants.ItemtargetSelf:
                        {
                            RequirementTargetIdentity = this.Identity;
                            break;
                        }

                    default:
                        {
                            RequirementTargetIdentity = new Identity();
                            break;
                        }
                }

                if (RequirementTargetIdentity.Type == IdentityType.None)
                {
                    return false;

                    // Target not found, cant check reqs -> FALSE
                }

                IStats reqTargetStatholder = ((IInstancedEntity)this).Playfield.FindByIdentity(
                    RequirementTargetIdentity);
                int statval = reqTargetStatholder.Stats.StatValueByName(aor.Statnumber);
                bool reqresult = true;
                switch (aor.Operator)
                {
                    case Constants.OperatorAnd:
                        {
                            reqresult = (statval & aor.Value) != 0;
                            break;
                        }

                    case Constants.OperatorOr:
                        {
                            reqresult = (statval | aor.Value) != 0;
                            break;
                        }

                    case Constants.OperatorEqualTo:
                        {
                            reqresult = statval == aor.Value;
                            break;
                        }

                    case Constants.OperatorLessThan:
                        {
                            reqresult = statval < aor.Value;
                            break;
                        }

                    case Constants.OperatorGreaterThan:
                        {
                            reqresult = statval > aor.Value;
                            break;
                        }

                    case Constants.OperatorUnequal:
                        {
                            reqresult = statval != aor.Value;
                            break;
                        }

                    case Constants.OperatorTrue:
                        {
                            reqresult = true;
                            break;
                        }

                    case Constants.OperatorFalse:
                        {
                            reqresult = false;
                            break;
                        }

                    case Constants.OperatorBitAnd:
                        {
                            reqresult = (statval & aor.Value) != 0;
                            break;
                        }

                    case Constants.OperatorBitOr:
                        {
                            reqresult = (statval | aor.Value) != 0;
                            break;
                        }

                    default:
                        {
                            // TRUE for now
                            reqresult = true;
                            break;
                        }
                }

                switch (childOperator)
                {
                    case Constants.OperatorAnd:
                        {
                            requirementsMet &= reqresult;
                            break;
                        }

                    case Constants.OperatorOr:
                        {
                            requirementsMet &= reqresult;
                            break;
                        }

                    case -1:
                        {
                            requirementsMet = reqresult;
                            break;
                        }

                    default:
                        break;
                }

                childOperator = aor.ChildOperator;
            }

            if (!checkAll)
            {
                if (foundCharRelated)
                {
                    requirementsMet &= foundCharRelated;
                }
                else
                {
                    requirementsMet = true;
                }
            }

            return requirementsMet;
        }

        #endregion
    }
}