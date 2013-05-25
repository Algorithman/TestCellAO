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

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Function;
    using ZoneEngine.GameObject.Enums;
    using ZoneEngine.GameObject.Items;
    using ZoneEngine.GameObject.Playfields;
    using ZoneEngine.GameObject.Stats;

    using Quaternion = AO.Core.Quaternion;
    using Vector3 = AO.Core.Vector3;

    #endregion

    /// <summary>
    /// </summary>
    public class Dynel : GameObject, IInstancedEntity
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
        AOCoord IInstancedEntity.Coordinates { get; set; }

        /// <summary>
        /// </summary>
        private DateTime predictionTime;

        /// <summary>
        /// </summary>
        public TimeSpan PredictionDuration
        {
            get
            {
                DateTime currentTime = DateTime.UtcNow;

                return currentTime - this.predictionTime;
            }
        }

        /// <summary>
        /// Calculate Turn time
        /// </summary>
        /// <returns>Turn time</returns>
        private double calculateTurnTime()
        {
            int turnSpeed;
            double turnTime;

            turnSpeed = this.stats.TurnSpeed.Value; // Stat #267 TurnSpeed

            if (turnSpeed == 0)
            {
                turnSpeed = 40000;
            }

            turnTime = 70000 / turnSpeed;

            return turnTime;
        }

        /// <summary>
        /// Calculate the effective run speed (run, walk, sneak etc)
        /// </summary>
        /// <returns>Effective run speed</returns>
        private int calculateEffectiveRunSpeed()
        {
            int effectiveRunSpeed;

            switch (this.moveMode)
            {
                case MoveModes.Run:
                    effectiveRunSpeed = this.stats.RunSpeed.Value; // Stat #156 = RunSpeed
                    break;

                case MoveModes.Walk:
                    effectiveRunSpeed = -500;
                    break;

                case MoveModes.Swim:

                    // Swim speed is calculated the same as Run Speed except is half as effective
                    effectiveRunSpeed = this.stats.Swim.Value >> 1; // Stat #138 = Swim
                    break;

                case MoveModes.Crawl:
                    effectiveRunSpeed = -600;
                    break;

                case MoveModes.Sneak:
                    effectiveRunSpeed = -500;
                    break;

                case MoveModes.Fly:
                    effectiveRunSpeed = 2200; // NV: TODO: Propper calc for this!
                    break;

                default:

                    // All other movement modes, sitting, sleeping, lounging, rooted, etc have a speed of 0
                    // As there is no way to 'force' that this way, we just default to 0 and hope that canMove() has been called to properly check.
                    effectiveRunSpeed = 0;
                    break;
            }

            return effectiveRunSpeed;
        }

        /// <summary>
        /// Calculate forward speed
        /// </summary>
        /// <returns>forward speed</returns>
        private double calculateForwardSpeed()
        {
            double speed;
            int effectiveRunSpeed;

            if ((this.moveDirection == MoveDirections.None) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            if (this.moveDirection == MoveDirections.Forwards)
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = Math.Max(0, (effectiveRunSpeed * 0.005) + 4);

                if (this.moveMode != MoveModes.Swim)
                {
                    speed = Math.Min(15, speed); // Forward speed is capped at 15 units/sec for non-swimming
                }
            }
            else
            {
                // NV: TODO: Verify this more. Especially with uber-low runspeeds (negative)
                speed = -Math.Max(0, (effectiveRunSpeed * 0.0035) + 4);

                if (this.moveMode != MoveModes.Swim)
                {
                    speed = Math.Max(-15, speed); // Backwards speed is capped at 15 units/sec for non-swimming
                }
            }

            return speed;
        }

        /// <summary>
        /// Can Character move?
        /// </summary>
        /// <returns>Can move=true</returns>
        private bool canMove()
        {
            if ((this.moveMode == MoveModes.Run) || (this.moveMode == MoveModes.Walk)
                || (this.moveMode == MoveModes.Swim) || (this.moveMode == MoveModes.Crawl)
                || (this.moveMode == MoveModes.Sneak) || (this.moveMode == MoveModes.Fly))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate strafe speed
        /// </summary>
        /// <returns>Strafe speed</returns>
        private double calculateStrafeSpeed()
        {
            double speed;
            int effectiveRunSpeed;

            // Note, you can not strafe while swimming or crawling
            if ((this.strafeDirection == SpinOrStrafeDirections.None) || (this.moveMode == MoveModes.Swim)
                || (this.moveMode == MoveModes.Crawl) || (!this.canMove()))
            {
                return 0;
            }

            effectiveRunSpeed = this.calculateEffectiveRunSpeed();

            // NV: TODO: Update this based off Forward runspeed when that is checked (strafe effective run speed = effective run speed / 2)
            speed = ((effectiveRunSpeed / 2) * 0.005) + 4;

            if (this.strafeDirection == SpinOrStrafeDirections.Left)
            {
                speed = -speed;
            }

            return speed;
        }

        /// <summary>
        /// Calculate move vector
        /// </summary>
        /// <returns>Movevector</returns>
        private Vector3 calculateMoveVector()
        {
            double forwardSpeed;
            double strafeSpeed;
            Vector3 forwardMove;
            Vector3 strafeMove;

            if (!this.canMove())
            {
                return Vector3.Origin;
            }

            forwardSpeed = this.calculateForwardSpeed();
            strafeSpeed = this.calculateStrafeSpeed();

            if ((forwardSpeed == 0) && (strafeSpeed == 0))
            {
                return Vector3.Origin;
            }

            if (forwardSpeed != 0)
            {
                forwardMove = Quaternion.RotateVector3(this.RawHeading, Vector3.AxisZ);
                forwardMove.Magnitude = Math.Abs(forwardSpeed);
                if (forwardSpeed < 0)
                {
                    forwardMove = -forwardMove;
                }
            }
            else
            {
                forwardMove = Vector3.Origin;
            }

            if (strafeSpeed != 0)
            {
                strafeMove = Quaternion.RotateVector3(this.RawHeading, Vector3.AxisX);
                strafeMove.Magnitude = Math.Abs(strafeSpeed);
                if (strafeSpeed < 0)
                {
                    strafeMove = -strafeMove;
                }
            }
            else
            {
                strafeMove = Vector3.Origin;
            }

            return forwardMove + strafeMove;
        }

        /// <summary>
        /// Calculate Turnangle
        /// </summary>
        /// <returns>Turnangle</returns>
        private double calculateTurnArcAngle()
        {
            double turnTime;
            double angle;
            double modifiedDuration;

            turnTime = this.calculateTurnTime();

            modifiedDuration = this.PredictionDuration.TotalSeconds % turnTime;

            angle = 2 * Math.PI * modifiedDuration / turnTime;

            return angle;
        }

        /// <summary>
        /// </summary>
        public AOCoord Coordinates
        {
            get
            {
                if ((this.moveDirection == MoveDirections.None) && (this.strafeDirection == SpinOrStrafeDirections.None))
                {
                    return new AOCoord(this.RawCoordinates);
                }
                else if (this.spinDirection == SpinOrStrafeDirections.None)
                {
                    Vector3 moveVector = this.calculateMoveVector();

                    moveVector = moveVector * this.PredictionDuration.TotalSeconds;

                    return new AOCoord(this.RawCoordinates + moveVector);
                }
                else
                {
                    Vector3 moveVector;
                    Vector3 positionFromCentreOfTurningCircle;
                    double turnArcAngle;
                    double y;
                    double duration;

                    duration = this.PredictionDuration.TotalSeconds;

                    moveVector = this.calculateMoveVector();
                    turnArcAngle = this.calculateTurnArcAngle();

                    // This is calculated seperately as height is unaffected by turning
                    y = this.RawCoordinates.y + (moveVector.y * duration);

                    if (this.spinDirection == SpinOrStrafeDirections.Left)
                    {
                        positionFromCentreOfTurningCircle = new Vector3(moveVector.z, y, -moveVector.x);
                    }
                    else
                    {
                        positionFromCentreOfTurningCircle = new Vector3(-moveVector.z, y, moveVector.x);
                    }

                    return
                        new AOCoord(
                            this.RawCoordinates
                            + Quaternion.RotateVector3(
                                new Quaternion(Vector3.AxisY, turnArcAngle), positionFromCentreOfTurningCircle)
                            - positionFromCentreOfTurningCircle);
                }
            }

            set
            {
                this.RawCoordinates = value.coordinate;
            }
        }

        /// <summary>
        /// </summary>
        private SpinOrStrafeDirections spinDirection = SpinOrStrafeDirections.None;

        /// <summary>
        /// </summary>
        private SpinOrStrafeDirections strafeDirection = SpinOrStrafeDirections.None;

        /// <summary>
        /// </summary>
        private MoveDirections moveDirection = MoveDirections.None;

        /// <summary>
        /// </summary>
        private MoveModes moveMode = MoveModes.Run; // Run should be an appropriate default for now

        /// <summary>
        /// </summary>
        private MoveModes previousMoveMode = MoveModes.Run; // Run should be an appropriate default for now

        /// <summary>
        /// </summary>
        public Quaternion Heading
        {
            get
            {
                if (this.spinDirection == SpinOrStrafeDirections.None)
                {
                    return this.RawHeading;
                }
                else
                {
                    double turnArcAngle;
                    Quaternion turnQuaterion;
                    Quaternion newHeading;

                    turnArcAngle = this.calculateTurnArcAngle();
                    turnQuaterion = new Quaternion(Vector3.AxisY, turnArcAngle);

                    newHeading = Quaternion.Hamilton(turnQuaterion, this.RawHeading);
                    newHeading.Normalize();

                    return newHeading;
                }
            }

            set
            {
                this.RawHeading = value;
            }
        }

        /// <summary>
        /// </summary>
        public Vector3 RawCoordinates { get; set; }

        /// <summary>
        /// </summary>
        public Quaternion RawHeading { get; set; }

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

        /// <summary>
        /// Update move type
        /// </summary>
        /// <param name="moveType">
        /// new move type
        /// </param>
        public void UpdateMoveType(byte moveType)
        {
            this.predictionTime = DateTime.UtcNow;

            /*
             * NV: Would be nice to have all other possible values filled out for this at some point... *Looks at Suiv*
             * More specifically, 10, 13 and 22 - 10 and 13 seem to be tied to spinning with mouse. 22 seems random (ping mabe?)
             * Also TODO: Tie this with CurrentMovementMode stat and persistance (ie, log off walking, log back on and still walking)
             * Values of CurrentMovementMode and their effects:
                0: slow moving feet not animating
                1: rooted cant sit
                2: walk
                3: run
                4: swim
                5: crawl
                6: sneak
                7: flying
                8: sitting
                9: rooted can sit
                10: same as 0
                11: sleeping
                12: lounging
                13: same as 0
                14: same as 0
                15: same as 0
                16: same as 0
             */
            switch (moveType)
            {
                case 1: // Forward Start
                    this.moveDirection = MoveDirections.Forwards;
                    break;
                case 2: // Forward Stop
                    this.moveDirection = MoveDirections.None;
                    break;

                case 3: // Reverse Start
                    this.moveDirection = MoveDirections.Backwards;
                    break;
                case 4: // Reverse Stop
                    this.moveDirection = MoveDirections.None;
                    break;

                case 5: // Strafe Right Start
                    this.strafeDirection = SpinOrStrafeDirections.Right;
                    break;
                case 6: // Strafe Stop (Right)
                    this.strafeDirection = SpinOrStrafeDirections.None;
                    break;

                case 7: // Strafe Left Start
                    this.strafeDirection = SpinOrStrafeDirections.Left;
                    break;
                case 8: // Strafe Stop (Left)
                    this.strafeDirection = SpinOrStrafeDirections.None;
                    break;

                case 9: // Turn Right Start
                    this.spinDirection = SpinOrStrafeDirections.Right;
                    break;
                case 10: // Mouse Turn Right Start
                    break;
                case 11: // Turn Stop (Right)
                    this.spinDirection = SpinOrStrafeDirections.None;
                    break;

                case 12: // Turn Left Start
                    this.spinDirection = SpinOrStrafeDirections.Left;
                    break;
                case 13: // Mouse Turn Left Start
                    break;
                case 14: // Turn Stop (Left)
                    this.spinDirection = SpinOrStrafeDirections.None;
                    break;

                case 15: // Jump Start

                    // NV: TODO: This!
                    break;
                case 16: // Jump Stop
                    break;

                case 17: // Elevate Up Start
                    break;
                case 18: // Elevate Up Stop
                    break;

                case 19: // ? 19 = 20 = 22 = 31 = 32
                    break;
                case 20: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 21: // Full Stop
                    break;

                case 22: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 23: // Switch To Frozen Mode
                    break;
                case 24: // Switch To Walk Mode
                    this.moveMode = MoveModes.Walk;
                    break;
                case 25: // Switch To Run Mode
                    this.moveMode = MoveModes.Run;
                    break;
                case 26: // Switch To Swim Mode
                    break;
                case 27: // Switch To Crawl Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Crawl;
                    break;
                case 28: // Switch To Sneak Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Sneak;
                    break;
                case 29: // Switch To Fly Mode
                    break;
                case 30: // Switch To Sit Ground Mode
                    this.previousMoveMode = this.moveMode;
                    this.moveMode = MoveModes.Sit;
                    this.stats.NanoDelta.CalcTrickle();
                    this.stats.HealDelta.CalcTrickle();
                    this.stats.NanoInterval.CalcTrickle();
                    this.stats.HealInterval.CalcTrickle();
                    break;

                case 31: // ? 19 = 20 = 22 = 31 = 32
                    break;
                case 32: // ? 19 = 20 = 22 = 31 = 32
                    break;

                case 33: // Switch To Sleep Mode
                    this.moveMode = MoveModes.Sleep;
                    break;
                case 34: // Switch To Lounge Mode
                    this.moveMode = MoveModes.Lounge;
                    break;

                case 35: // Leave Swim Mode
                    break;
                case 36: // Leave Sneak Mode
                    this.moveMode = this.previousMoveMode;
                    break;
                case 37: // Leave Sit Mode
                    this.moveMode = this.previousMoveMode;
                    this.stats.NanoDelta.CalcTrickle();
                    this.stats.HealDelta.CalcTrickle();
                    this.stats.NanoInterval.CalcTrickle();
                    this.stats.HealInterval.CalcTrickle();
                    break;
                case 38: // Leave Frozen Mode
                    break;
                case 39: // Leave Fly Mode
                    break;
                case 40: // Leave Crawl Mode
                    this.moveMode = this.previousMoveMode;
                    break;
                case 41: // Leave Sleep Mode
                    break;
                case 42: // Leave Lounge Mode
                    break;
                default:

                    // Console.WriteLine("Unknown MoveType: " + moveType);
                    break;
            }

            // Console.WriteLine((moveDirection != 0 ? moveMode.ToString() : "Stand") + "ing in the direction " + moveDirection.ToString() + (spinDirection != 0 ? " while spinning " + spinDirection.ToString() : "") + (strafeDirection != 0 ? " and strafing " + strafeDirection.ToString() : ""));
        }

        public void WriteStats()
        {
            // TODO: Implement it
        }

        #endregion
    }
}