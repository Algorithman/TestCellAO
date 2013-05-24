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

namespace ZoneEngine.GameObject.Stats
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using AO.Core;

    using Cell.Core;

    using Database;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Network;
    using ZoneEngine.Network.InternalBus.InternalMessages;
    using ZoneEngine.Network.Packets;

    #endregion

    #region StatChangedEventArgs

    /// <summary>
    /// Event Arguments for changed stats
    /// </summary>
    public class StatChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="changedStat">
        /// </param>
        /// <param name="valueBeforeChange">
        /// </param>
        /// <param name="valueAfterChange">
        /// </param>
        /// <param name="announceToPlayfield">
        /// </param>
        public StatChangedEventArgs(
            ClassStat changedStat, uint valueBeforeChange, uint valueAfterChange, bool announceToPlayfield)
        {
            this.Stat = changedStat;
            this.OldValue = valueBeforeChange;
            this.NewValue = valueAfterChange;
            this.AnnounceToPlayfield = announceToPlayfield;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public bool AnnounceToPlayfield { get; set; }

        /// <summary>
        /// </summary>
        public uint NewValue { get; set; }

        /// <summary>
        /// </summary>
        public uint OldValue { get; set; }

        /// <summary>
        /// </summary>
        public Dynel Parent
        {
            get
            {
                return this.Stat.Parent;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Stat { get; private set; }

        #endregion
    }

    #endregion

    #region ClassStat  class for one stat

    /// <summary>
    /// </summary>
    public class ClassStat
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly List<int> affects = new List<int>();

        /// <summary>
        /// </summary>
        private bool announceToPlayfield = true;

        /// <summary>
        /// </summary>
        private bool sendBaseValue = true;

        /// <summary>
        /// </summary>
        private int statPercentageModifier = 100; // From Items/Perks/Nanos

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <param name="name">
        /// </param>
        /// <param name="sendBaseValue">
        /// </param>
        /// <param name="dontWrite">
        /// </param>
        /// <param name="announceToPlayfield">
        /// </param>
        public ClassStat(
            int number, uint defaultValue, string name, bool sendBaseValue, bool dontWrite, bool announceToPlayfield)
        {
            this.DoNotDontWriteToSql = true;
            this.StatNumber = number;
            this.StatDefaultValue = defaultValue;
            this.StatBaseValue = defaultValue;
            this.StatDefaultValue = defaultValue;
            this.sendBaseValue = sendBaseValue;
            this.DoNotDontWriteToSql = dontWrite;
            this.announceToPlayfield = announceToPlayfield;

            // Obsolete            StatName = name;
        }

        /// <summary>
        /// </summary>
        public ClassStat()
        {
        }

        #endregion

        #region Public Events

        /// <summary>
        /// </summary>
        public event EventHandler<StatChangedEventArgs> RaiseAfterStatChangedEvent;

        /// <summary>
        /// </summary>
        public event EventHandler<StatChangedEventArgs> RaiseBeforeStatChangedEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public List<int> Affects
        {
            get
            {
                return this.affects;
            }
        }

        /// <summary>
        /// </summary>
        public bool AnnounceToPlayfield
        {
            get
            {
                return this.announceToPlayfield;
            }

            set
            {
                this.announceToPlayfield = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool Changed { get; set; }

        /// <summary>
        /// </summary>
        public bool DoNotDontWriteToSql { get; set; }

        /// <summary>
        /// </summary>
        public Dynel Parent { get; private set; }

        /// <summary>
        /// </summary>
        public bool SendBaseValue
        {
            get
            {
                return this.sendBaseValue;
            }

            set
            {
                this.sendBaseValue = value;
            }
        }

        private uint statBaseValue;
        /// <summary>
        /// </summary>
        public uint StatBaseValue
        {
            get
            {
                return statBaseValue;
            }
            set
            {
                bool sendit = value != statBaseValue;
                statBaseValue = value;
                if (sendit)
                {
                    Stat.Send(this.Parent, this.StatNumber, value, this.announceToPlayfield);
                }
            }
        }

        /// <summary>
        /// </summary>
        public uint StatDefaultValue { get; set; }

        /// <summary>
        /// </summary>
        public int StatModifier { get; set; }

        /// <summary>
        /// </summary>
        public int StatNumber { get; set; }

        /// <summary>
        /// </summary>
        public int StatPercentageModifier
        {
            get
            {
                return this.statPercentageModifier;
            }

            set
            {
                this.statPercentageModifier = value;
            }
        }

        /// <summary>
        /// </summary>
        public int Trickle { get; set; }

        /// <summary>
        /// </summary>
        public virtual int Value
        {
            get
            {
                return (int)Math.Floor(
                    (double) // ReSharper disable PossibleLossOfFraction
                    ((this.StatBaseValue + this.StatModifier + this.Trickle) * this.statPercentageModifier / 100));

                // ReSharper restore PossibleLossOfFraction
            }

            set
            {
                this.Set(value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        public void AffectStats()
        {
            if (!(this.Parent is Character) && !(this.Parent is NonPlayerCharacter))
            {
                return;
            }

            foreach (int c in this.affects)
            {
                this.Parent.Stats.GetStatbyNumber(c).CalcTrickle();
            }
        }

        /// <summary>
        /// Calculate trickle value (prototype)
        /// </summary>
        public virtual void CalcTrickle()
        {
            if (!this.Parent.Starting)
            {
                this.AffectStats();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="val">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual uint GetMaxValue(uint val)
        {
            return val;
        }

        /// <summary>
        /// Read stat from Sql
        /// </summary>
        public void ReadStatFromSql()
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }

            int id = this.Parent.Identity.Instance;

            this.StatBaseValue = DBStats.GetById(id, this.StatNumber).First().Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void Set(uint value)
        {
            if ((this.Parent == null) || this.Parent.Starting)
            {
                this.StatBaseValue = value;
                return;
            }

            if (value != this.StatBaseValue)
            {
                uint oldvalue = (uint)this.Value;
                uint max = this.GetMaxValue(value);
                this.OnBeforeStatChangedEvent(new StatChangedEventArgs(this, oldvalue, max, this.announceToPlayfield));
                this.StatBaseValue = max;
                this.OnAfterStatChangedEvent(new StatChangedEventArgs(this, oldvalue, max, this.announceToPlayfield));
                this.Changed = true;
                this.WriteStatToSql();

                if (!this.Parent.Starting)
                {
                    this.AffectStats();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        public void Set(int value)
        {
            this.Set((uint)value);
        }

        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        public void SetParent(Dynel parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Write Stat to Sql
        /// </summary>
        public void WriteStatToSql()
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }

            int id = this.Parent.Identity.Instance;
            SqlWrapper sql = new SqlWrapper();
            if (this.Changed)
            {
                /* TODO: REDO
                if (this.Parent is NonPlayerCharacter)
                {
                sql.SqlInsert(
                "INSERT INTO " + (this.Parent).GetSqlTablefromDynelType() +
                "_stats (ID, Playfield, Stat, Value) VALUES (" + id + "," + this.Parent.PlayField + "," +
                this.StatNumber + "," + ((Int32)this.StatBaseValue) + ") ON DUPLICATE KEY UPDATE Value=" +
                ((Int32)this.StatBaseValue) + ";");
                }
                else
                {
                sql.SqlInsert(
                "INSERT INTO " + (this.Parent).GetSqlTablefromDynelType() + "_stats (ID, Stat, Value) VALUES (" +
                id + "," + this.StatNumber + "," + ((Int32)this.StatBaseValue) +
                ") ON DUPLICATE KEY UPDATE Value=" + ((Int32)this.StatBaseValue) + ";");
                }
                */
            }
        }

        /// <summary>
        /// Write Stat to Sql
        /// </summary>
        /// <param name="doit">
        /// </param>
        public void WriteStatToSql(bool doit)
        {
            if (this.DoNotDontWriteToSql)
            {
                return;
            }

            int id = this.Parent.Identity.Instance;
            SqlWrapper sql = new SqlWrapper();
            if (doit)
            {
                /* TODO: REDO
                if (this.Parent is NonPlayerCharacter)
                {
                sql.SqlInsert(
                "INSERT INTO " + (this.Parent).GetSqlTablefromDynelType() +
                "_stats (ID, Playfield, Stat, Value) VALUES (" + id + "," + this.Parent.PlayField + "," +
                this.StatNumber + "," + ((Int32)this.StatBaseValue) + ") ON DUPLICATE KEY UPDATE Value=" +
                ((Int32)this.StatBaseValue) + ";");
                }
                else
                {
                sql.SqlInsert(
                "INSERT INTO " + (this.Parent).GetSqlTablefromDynelType() + "_stats (ID, Stat, Value) VALUES (" +
                id + "," + this.StatNumber + "," + ((Int32)this.StatBaseValue) +
                ") ON DUPLICATE KEY UPDATE Value=" + ((Int32)this.StatBaseValue) + ";");
                }
                */
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnAfterStatChangedEvent(StatChangedEventArgs e)
        {
            EventHandler<StatChangedEventArgs> handler = this.RaiseAfterStatChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnBeforeStatChangedEvent(StatChangedEventArgs e)
        {
            EventHandler<StatChangedEventArgs> handler = this.RaiseBeforeStatChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }

    #endregion

    #region Character_Stats holder for Character's stats

    /// <summary>
    /// </summary>
    public class DynelStats
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbChemicalAC = new ClassStat(241, 0, "AbsorbChemicalAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbColdAC = new ClassStat(243, 0, "AbsorbColdAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbEnergyAC = new ClassStat(240, 0, "AbsorbEnergyAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbFireAC = new ClassStat(244, 0, "AbsorbFireAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbMeleeAC = new ClassStat(239, 0, "AbsorbMeleeAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbNanoAC = new ClassStat(246, 0, "AbsorbNanoAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbPoisonAC = new ClassStat(245, 0, "AbsorbPoisonAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbProjectileAC = new ClassStat(238, 0, "AbsorbProjectileAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat absorbRadiationAC = new ClassStat(242, 0, "AbsorbRadiationAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat accessCount = new ClassStat(35, 1234567890, "AccessCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat accessGrant = new ClassStat(258, 1234567890, "AccessGrant", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat accessKey = new ClassStat(195, 1234567890, "AccessKey", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat accountFlags = new ClassStat(660, 1234567890, "AccountFlags", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat accumulatedDamage = new ClassStat(
            222, 1234567890, "AccumulatedDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgEntranceStyles = new ClassStat(
            384, 1234567890, "ACGEntranceStyles", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgItemCategoryId = new ClassStat(
            704, 1234567890, "ACGItemCategoryID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgItemLevel = new ClassStat(701, 1234567890, "ACGItemLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgItemSeed = new ClassStat(700, 1234567890, "ACGItemSeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgItemTemplateId = new ClassStat(
            702, 1234567890, "ACGItemTemplateID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat acgItemTemplateId2 = new ClassStat(
            703, 1234567890, "ACGItemTemplateID2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat actionCategory = new ClassStat(
            588, 1234567890, "ActionCategory", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat advantageHash1 = new ClassStat(
            651, 1234567890, "AdvantageHash1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat advantageHash2 = new ClassStat(
            652, 1234567890, "AdvantageHash2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat advantageHash3 = new ClassStat(
            653, 1234567890, "AdvantageHash3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat advantageHash4 = new ClassStat(
            654, 1234567890, "AdvantageHash4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat advantageHash5 = new ClassStat(
            655, 1234567890, "AdvantageHash5", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill adventuring = new StatSkill(137, 5, "Adventuring", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat age = new ClassStat(58, 0, "Age", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat aggDef = new ClassStat(51, 100, "AggDef", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat aggressiveness = new ClassStat(
            201, 1234567890, "Aggressiveness", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat agility = new ClassStat(17, 0, "Agility", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill aimedShot = new StatSkill(151, 5, "AimedShot", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat alienLevel = new ClassStat(169, 0, "AlienLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatAlienNextXP alienNextXP = new StatAlienNextXP(
            178, 1500, "AlienNextXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat alienXP = new ClassStat(40, 0, "AlienXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat alignment = new ClassStat(62, 0, "Alignment", false, false, false);

        /// <summary>
        /// </summary>
        private readonly List<ClassStat> all = new List<ClassStat>();

        /// <summary>
        /// </summary>
        private readonly ClassStat ammoName = new ClassStat(399, 1234567890, "AmmoName", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat ammoType = new ClassStat(420, 1234567890, "AmmoType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat ams = new ClassStat(22, 1234567890, "AMS", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat amsCap = new ClassStat(538, 1234567890, "AmsCap", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat amsModifier = new ClassStat(276, 0, "AMSModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat anim = new ClassStat(13, 1234567890, "Anim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat animPlay = new ClassStat(501, 1234567890, "AnimPlay", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat animPos = new ClassStat(500, 1234567890, "AnimPos", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat animSet = new ClassStat(353, 1234567890, "AnimSet", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat animSpeed = new ClassStat(502, 1234567890, "AnimSpeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat apartmentAccessCard = new ClassStat(
            584, 1234567890, "ApartmentAccessCard", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat apartmentsAllowed = new ClassStat(582, 1, "ApartmentsAllowed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat apartmentsOwned = new ClassStat(583, 0, "ApartmentsOwned", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat areaInstance = new ClassStat(87, 1234567890, "AreaInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat areaType = new ClassStat(86, 1234567890, "AreaType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat armourType = new ClassStat(424, 1234567890, "ArmourType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill assaultRifle = new StatSkill(116, 5, "AssaultRifle", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attackCount = new ClassStat(36, 1234567890, "AttackCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attackRange = new ClassStat(287, 1234567890, "AttackRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attackShield = new ClassStat(516, 1234567890, "AttackShield", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attackSpeed = new ClassStat(3, 5, "AttackSpeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attackType = new ClassStat(354, 1234567890, "AttackType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat attitude = new ClassStat(63, 0, "Attitude", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat autoAttackFlags = new ClassStat(349, 5, "AutoAttackFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat autoLockTimeDefault = new ClassStat(
            175, 1234567890, "AutoLockTimeDefault", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat autoUnlockTimeDefault = new ClassStat(
            176, 1234567890, "AutoUnlockTimeDefault", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat backMesh = new ClassStat(38, 0, "BackMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat backstab = new ClassStat(489, 1234567890, "Backstab", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat bandolierSlots = new ClassStat(46, 1234567890, "BandolierSlots", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat battlestationRep = new ClassStat(670, 10, "BattlestationRep", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat battlestationSide = new ClassStat(668, 0, "BattlestationSide", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat beltSlots = new ClassStat(45, 0, "BeltSlots", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat berserkMode = new ClassStat(235, 1234567890, "BerserkMode", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill biologicalMetamorphose = new StatSkill(
            128, 5, "BiologicalMetamorphose", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat birthDate = new ClassStat(248, 1234567890, "BirthDate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill bodyDevelopment = new StatSkill(152, 5, "BodyDevelopment", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill bow = new StatSkill(111, 5, "Bow", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill bowSpecialAttack = new StatSkill(121, 5, "BowSpecialAttack", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat brainType = new ClassStat(340, 1234567890, "BrainType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill brawl = new StatSkill(142, 5, "Brawl", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill breakingEntry = new StatSkill(165, 5, "BreakingEntry", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat breed = new ClassStat(4, 1234567890, "Breed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat breedHostility = new ClassStat(
            204, 1234567890, "BreedHostility", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat breedLimit = new ClassStat(320, 1234567890, "BreedLimit", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat buildingComplexInst = new ClassStat(
            188, 1234567890, "BuildingComplexInst", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat buildingInstance = new ClassStat(
            185, 1234567890, "BuildingInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat buildingType = new ClassStat(184, 1234567890, "BuildingType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill burst = new StatSkill(148, 5, "Burst", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat burstRecharge = new ClassStat(374, 1234567890, "BurstRecharge", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat buyModifier = new ClassStat(426, 1234567890, "BuyModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat can = new ClassStat(30, 1234567890, "Can", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat canChangeClothes = new ClassStat(
            223, 1234567890, "CanChangeClothes", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat cardOwnerInstance = new ClassStat(
            187, 1234567890, "CardOwnerInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat cardOwnerType = new ClassStat(186, 1234567890, "CardOwnerType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat cash = new ClassStat(61, 0, "Cash", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat castEffectType = new ClassStat(
            428, 1234567890, "CastEffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat castSelfAbstractAnim = new ClassStat(
            378, 1234567890, "CastSelfAbstractAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat castSound = new ClassStat(270, 1234567890, "CastSound", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat castTargetAbstractAnim = new ClassStat(
            377, 1234567890, "CastTargetAbstractAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat catAnim = new ClassStat(401, 1234567890, "CATAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat catAnimFlags = new ClassStat(402, 1234567890, "CATAnimFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat catMesh = new ClassStat(42, 1234567890, "CATMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chanceOfBreakOnDebuff = new ClassStat(
            386, 1234567890, "ChanceOfBreakOnDebuff", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chanceOfBreakOnSpellAttack = new ClassStat(
            385, 1234567890, "ChanceOfBreakOnSpellAttack", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chanceOfUse = new ClassStat(422, 1234567890, "ChanceOfUse", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat changeSideCount = new ClassStat(237, 0, "ChangeSideCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charRadius = new ClassStat(421, 1234567890, "CharRadius", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charState = new ClassStat(434, 1234567890, "CharState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charTmp1 = new ClassStat(441, 1234567890, "CharTmp1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charTmp2 = new ClassStat(442, 1234567890, "CharTmp2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charTmp3 = new ClassStat(443, 1234567890, "CharTmp3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat charTmp4 = new ClassStat(444, 1234567890, "CharTmp4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chemicalAC = new ClassStat(93, 0, "ChemicalAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chemicalDamageModifier = new ClassStat(
            281, 0, "ChemicalDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill chemistry = new StatSkill(163, 5, "Chemistry", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat chestFlags = new ClassStat(394, 1234567890, "ChestFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat cityInstance = new ClassStat(640, 1234567890, "CityInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat cityTerminalRechargePercent = new ClassStat(
            642, 1234567890, "CityTerminalRechargePercent", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clan = new ClassStat(5, 0, "Clan", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanConserver = new ClassStat(571, 0, "ClanConserver", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanDevoted = new ClassStat(570, 0, "ClanDevoted", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanFinalized = new ClassStat(314, 1234567890, "ClanFinalized", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanGaia = new ClassStat(563, 0, "ClanGaia", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanHierarchy = new ClassStat(260, 1234567890, "ClanHierarchy", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanInstance = new ClassStat(305, 1234567890, "ClanInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanItemInstance = new ClassStat(
            331, 1234567890, "ClanItemInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanItemType = new ClassStat(330, 1234567890, "ClanItemType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanLevel = new ClassStat(48, 1234567890, "ClanLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanPrice = new ClassStat(302, 1234567890, "ClanPrice", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanRedeemed = new ClassStat(572, 0, "ClanRedeemed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanSentinels = new ClassStat(561, 0, "ClanSentinels", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanType = new ClassStat(304, 1234567890, "ClanType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanUpkeepInterval = new ClassStat(
            312, 1234567890, "ClanUpkeepInterval", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clanVanguards = new ClassStat(565, 0, "ClanVanguards", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat clientActivated = new ClassStat(
            262, 1234567890, "ClientActivated", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill closeCombatInitiative = new StatSkill(
            118, 5, "CloseCombatInitiative", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat coldAC = new ClassStat(95, 0, "ColdAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat coldDamageModifier = new ClassStat(
            311, 1234567890, "ColdDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat collideCheckInterval = new ClassStat(
            437, 1234567890, "CollideCheckInterval", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat collisionRadius = new ClassStat(
            357, 1234567890, "CollisionRadius", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat commandRange = new ClassStat(456, 1234567890, "CommandRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat compulsion = new ClassStat(328, 1234567890, "Compulsion", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill computerLiteracy = new StatSkill(161, 5, "ComputerLiteracy", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill concealment = new StatSkill(164, 5, "Concealment", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat conditionState = new ClassStat(
            530, 1234567890, "ConditionState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat conformity = new ClassStat(200, 1234567890, "Conformity", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat corpseAnimKey = new ClassStat(417, 1234567890, "CorpseAnimKey", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat corpseHash = new ClassStat(398, 1234567890, "Corpse_Hash", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat corpseInstance = new ClassStat(
            416, 1234567890, "CorpseInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat corpseType = new ClassStat(415, 1234567890, "CorpseType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat criticalDecrease = new ClassStat(
            391, 1234567890, "CriticalDecrease", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat criticalIncrease = new ClassStat(
            379, 1234567890, "CriticalIncrease", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currBodyLocation = new ClassStat(220, 0, "CurrBodyLocation", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentMass = new ClassStat(78, 0, "CurrentMass", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentMovementMode = new ClassStat(
            173, 3, "CurrentMovementMode", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentNCU = new ClassStat(180, 0, "CurrentNCU", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNanoPoints currentNano = new StatNanoPoints(214, 1, "CurrentNano", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentPlayfield = new ClassStat(
            589, 1234567890, "CurrentPlayfield", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentState = new ClassStat(423, 0, "CurrentState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat currentTime = new ClassStat(578, 1234567890, "CurrentTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat damageBonus = new ClassStat(284, 1234567890, "DamageBonus", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat damageOverrideType = new ClassStat(
            339, 1234567890, "DamageOverrideType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat damageToNano = new ClassStat(659, 1234567890, "DamageToNano", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat damageToNanoMultiplier = new ClassStat(
            661, 1234567890, "DamageToNanoMultiplier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat damageType = new ClassStat(436, 1234567890, "DamageType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat deadTimer = new ClassStat(34, 0, "DeadTimer", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat deathReason = new ClassStat(338, 1234567890, "DeathReason", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat debuffFormula = new ClassStat(332, 1234567890, "DebuffFormula", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat defaultAttackType = new ClassStat(
            292, 1234567890, "DefaultAttackType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat defaultPos = new ClassStat(88, 1234567890, "DefaultPos", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat desiredTargetDistance = new ClassStat(
            447, 1234567890, "DesiredTargetDistance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat dieAnim = new ClassStat(387, 1234567890, "DieAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill dimach = new StatSkill(144, 5, "Dimach", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill disarmTrap = new StatSkill(135, 5, "DisarmTrap", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat displayCATAnim = new ClassStat(
            403, 1234567890, "DisplayCATAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat displayCATMesh = new ClassStat(
            404, 1234567890, "DisplayCATMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat distanceToSpawnpoint = new ClassStat(
            641, 1234567890, "DistanceToSpawnpoint", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill distanceWeaponInitiative = new StatSkill(
            119, 5, "DistanceWeaponInitiative", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat districtNano = new ClassStat(590, 1234567890, "DistrictNano", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat districtNanoInterval = new ClassStat(
            591, 1234567890, "DistrictNanoInterval", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat dms = new ClassStat(29, 1234567890, "DMS", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat dmsModifier = new ClassStat(277, 0, "DMSModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill dodge = new StatSkill(154, 5, "Dodge", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat doorBlockTime = new ClassStat(335, 1234567890, "DoorBlockTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat doorFlags = new ClassStat(259, 1234567890, "DoorFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill driveAir = new StatSkill(139, 5, "DriveAir", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill driveGround = new StatSkill(166, 5, "DriveGround", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill driveWater = new StatSkill(117, 5, "DriveWater", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill duck = new StatSkill(153, 5, "Duck", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat dudChance = new ClassStat(534, 1234567890, "DudChance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat durationModifier = new ClassStat(
            464, 1234567890, "DurationModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat effectBlue = new ClassStat(462, 1234567890, "EffectBlue", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat effectGreen = new ClassStat(461, 1234567890, "EffectGreen", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat effectIcon = new ClassStat(183, 1234567890, "EffectIcon", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat effectRed = new ClassStat(460, 1234567890, "EffectRed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat effectType = new ClassStat(413, 1234567890, "EffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill electricalEngineering = new StatSkill(
            126, 5, "ElectricalEngineering", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat energy = new ClassStat(26, 1234567890, "Energy", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat energyAC = new ClassStat(92, 0, "EnergyAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat energyDamageModifier = new ClassStat(
            280, 0, "EnergyDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat equipDelay = new ClassStat(211, 1234567890, "EquipDelay", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat equippedWeapons = new ClassStat(
            274, 1234567890, "EquippedWeapons", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill evade = new StatSkill(155, 5, "Evade", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat exitInstance = new ClassStat(189, 1234567890, "ExitInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat expansion = new ClassStat(389, 0, "Expansion", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat expansionPlayfield = new ClassStat(
            531, 1234567890, "ExpansionPlayfield", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat extenalDoorInstance = new ClassStat(
            193, 1234567890, "ExtenalDoorInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat extenalPlayfieldInstance = new ClassStat(
            192, 1234567890, "ExtenalPlayfieldInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat extendedFlags = new ClassStat(598, 1234567890, "ExtendedFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat extendedTime = new ClassStat(373, 1234567890, "ExtendedTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat extroverty = new ClassStat(203, 1234567890, "Extroverty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fabricType = new ClassStat(41, 1234567890, "FabricType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat face = new ClassStat(31, 1234567890, "Face", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat faceTexture = new ClassStat(347, 1234567890, "FaceTexture", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat factionModifier = new ClassStat(
            543, 1234567890, "FactionModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fallDamage = new ClassStat(474, 1234567890, "FallDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill fastAttack = new StatSkill(147, 5, "FastAttack", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fatness = new ClassStat(47, 1234567890, "Fatness", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat features = new ClassStat(224, 6, "Features", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill fieldQuantumPhysics = new StatSkill(
            157, 5, "FieldQuantumPhysics", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fireAC = new ClassStat(97, 0, "FireAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fireDamageModifier = new ClassStat(316, 0, "FireDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill firstAid = new StatSkill(123, 5, "FirstAid", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fixtureFlags = new ClassStat(473, 1234567890, "FixtureFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat flags = new ClassStat(0, 8917569, "Flags", false, false, true);

        /// <summary>
        /// </summary>
        private readonly StatSkill flingShot = new StatSkill(150, 5, "FlingShot", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill fullAuto = new StatSkill(167, 5, "FullAuto", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat fullAutoRecharge = new ClassStat(
            375, 1234567890, "FullAutoRecharge", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat gatherAbstractAnim = new ClassStat(
            376, 1234567890, "GatherAbstractAnim", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat gatherEffectType = new ClassStat(
            366, 1234567890, "GatherEffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat gatherSound = new ClassStat(269, 1234567890, "GatherSound", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat genderLimit = new ClassStat(321, 1234567890, "GenderLimit", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat globalClanInstance = new ClassStat(
            310, 1234567890, "GlobalClanInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat globalClanType = new ClassStat(
            309, 1234567890, "GlobalClanType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat globalResearchGoal = new ClassStat(266, 0, "GlobalResearchGoal", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat globalResearchLevel = new ClassStat(
            264, 0, "GlobalResearchLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat gmLevel = new ClassStat(215, 0, "GmLevel", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat gos = new ClassStat(566, 0, "GOS", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill grenade = new StatSkill(109, 5, "Grenade", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hairMesh = new ClassStat(32, 0, "HairMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hasAlwaysLootable = new ClassStat(
            345, 1234567890, "HasAlwaysLootable", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hasKnuBotData = new ClassStat(768, 1234567890, "HasKnuBotData", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hateValueModifyer = new ClassStat(
            288, 1234567890, "HateValueModifyer", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat headMesh = new ClassStat(64, 0, "HeadMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatHealDelta healDelta = new StatHealDelta(343, 1234567890, "HealDelta", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatHealInterval healInterval = new StatHealInterval(
            342, 29, "HealInterval", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat healMultiplier = new ClassStat(
            535, 1234567890, "HealMultiplier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatHitPoints health = new StatHitPoints(27, 1, "Health", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat healthChange = new ClassStat(172, 1234567890, "HealthChange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat healthChangeBest = new ClassStat(
            170, 1234567890, "HealthChangeBest", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat healthChangeWorst = new ClassStat(
            171, 1234567890, "HealthChangeWorst", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat height = new ClassStat(28, 1234567890, "Height", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hitEffectType = new ClassStat(361, 1234567890, "HitEffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hitSound = new ClassStat(272, 1234567890, "HitSound", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat houseTemplate = new ClassStat(620, 1234567890, "HouseTemplate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hpLevelUp = new ClassStat(601, 1234567890, "HPLevelUp", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat hpPerSkill = new ClassStat(602, 1234567890, "HPPerSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat icon = new ClassStat(79, 0, "Icon", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat impactEffectType = new ClassStat(
            414, 1234567890, "ImpactEffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat inPlay = new ClassStat(194, 0, "InPlay", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat info = new ClassStat(15, 1234567890, "Info", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat initiativeType = new ClassStat(
            440, 1234567890, "InitiativeType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat instance = new ClassStat(1002, 1234567890, "Instance", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat insurancePercentage = new ClassStat(
            236, 0, "InsurancePercentage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat insuranceTime = new ClassStat(49, 0, "InsuranceTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat intelligence = new ClassStat(19, 0, "Intelligence", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat interactionRadius = new ClassStat(
            297, 1234567890, "InteractionRadius", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat interruptModifier = new ClassStat(
            383, 1234567890, "InterruptModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat invadersKilled = new ClassStat(615, 0, "InvadersKilled", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat inventoryId = new ClassStat(55, 1234567890, "InventoryId", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat inventoryTimeout = new ClassStat(
            50, 1234567890, "InventoryTimeout", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatIP ip = new StatIP(53, 1500, "IP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat isFightingMe = new ClassStat(410, 0, "IsFightingMe", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat isVehicle = new ClassStat(658, 1234567890, "IsVehicle", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemAnim = new ClassStat(99, 1234567890, "ItemAnim", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemClass = new ClassStat(76, 1234567890, "ItemClass", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemDelay = new ClassStat(294, 1234567890, "ItemDelay", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemDelayCap = new ClassStat(523, 1234567890, "ItemDelayCap", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemHateValue = new ClassStat(283, 1234567890, "ItemHateValue", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemOpposedSkill = new ClassStat(
            295, 1234567890, "ItemOpposedSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemSIS = new ClassStat(296, 1234567890, "ItemSIS", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemSkill = new ClassStat(293, 1234567890, "ItemSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat itemType = new ClassStat(72, 0, "ItemType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat killedByInvaders = new ClassStat(616, 0, "KilledByInvaders", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastConcretePlayfieldInstance = new ClassStat(
            191, 0, "LastConcretePlayfieldInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastMailCheckTime = new ClassStat(
            650, 1283065897, "LastMailCheckTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastPerkResetTime = new ClassStat(577, 0, "LastPerkResetTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastRnd = new ClassStat(522, 1234567890, "LastRnd", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastSK = new ClassStat(574, 0, "LastSK", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastSaveXP = new ClassStat(372, 0, "LastSaveXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastSaved = new ClassStat(249, 1234567890, "LastSaved", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lastXP = new ClassStat(57, 0, "LastXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat leaderLockDownTime = new ClassStat(
            614, 1234567890, "LeaderLockDownTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat level = new ClassStat(54, 1234567890, "Level", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat levelLimit = new ClassStat(322, 1234567890, "LevelLimit", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatHealth life = new StatHealth(1, 1, "Life", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat liquidType = new ClassStat(268, 1234567890, "LiquidType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lockDifficulty = new ClassStat(
            299, 1234567890, "LockDifficulty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lockDownTime = new ClassStat(613, 1234567890, "LockDownTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat losHeight = new ClassStat(466, 1234567890, "LOSHeight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat lowresMesh = new ClassStat(390, 1234567890, "LowresMesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill lrEnergyWeapon = new StatSkill(133, 5, "LR_EnergyWeapon", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill lrMultipleWeapon = new StatSkill(134, 5, "LR_MultipleWeapon", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapAreaPart1 = new ClassStat(471, 0, "MapAreaPart1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapAreaPart2 = new ClassStat(472, 0, "MapAreaPart2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapAreaPart3 = new ClassStat(585, 0, "MapAreaPart3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapAreaPart4 = new ClassStat(586, 0, "MapAreaPart4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapFlags = new ClassStat(9, 0, "MapFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill mapNavigation = new StatSkill(140, 5, "MapNavigation", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mapOptions = new ClassStat(470, 0, "MapOptions", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill martialArts = new StatSkill(100, 5, "MartialArts", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill materialCreation = new StatSkill(130, 5, "MaterialCreation", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill materialLocation = new StatSkill(131, 5, "MaterialLocation", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill materialMetamorphose = new StatSkill(
            127, 5, "MaterialMetamorphose", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat maxDamage = new ClassStat(285, 1234567890, "MaxDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat maxEnergy = new ClassStat(212, 1234567890, "MaxEnergy", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat maxMass = new ClassStat(24, 1234567890, "MaxMass", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat maxNCU = new ClassStat(181, 8, "MaxNCU", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNano maxNanoEnergy = new StatNano(221, 1, "MaxNanoEnergy", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat maxShopItems = new ClassStat(606, 1234567890, "MaxShopItems", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mechData = new ClassStat(662, 0, "MechData", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill mechanicalEngineering = new StatSkill(
            125, 5, "MechanicalEngineering", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat meleeAC = new ClassStat(91, 0, "MeleeAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat meleeDamageModifier = new ClassStat(
            279, 0, "MeleeDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill meleeEnergyWeapon = new StatSkill(104, 5, "MeleeEnergyWeapon", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill meleeMultiple = new StatSkill(101, 5, "MeleeMultiple", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat memberInstance = new ClassStat(
            308, 1234567890, "MemberInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat memberType = new ClassStat(307, 1234567890, "MemberType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat members = new ClassStat(300, 999, "Members", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat mesh = new ClassStat(12, 17530, "Mesh", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat metaType = new ClassStat(75, 0, "MetaType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat metersWalked = new ClassStat(252, 1234567890, "MeetersWalked", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat minDamage = new ClassStat(286, 1234567890, "MinDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat minMembers = new ClassStat(301, 1234567890, "MinMembers", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits1 = new ClassStat(256, 0, "MissionBits1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits10 = new ClassStat(617, 0, "MissionBits10", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits11 = new ClassStat(618, 0, "MissionBits11", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits12 = new ClassStat(619, 0, "MissionBits12", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits2 = new ClassStat(257, 0, "MissionBits2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits3 = new ClassStat(303, 0, "MissionBits3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits4 = new ClassStat(432, 0, "MissionBits4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits5 = new ClassStat(65, 0, "MissionBits5", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits6 = new ClassStat(66, 0, "MissionBits6", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits7 = new ClassStat(67, 0, "MissionBits7", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits8 = new ClassStat(544, 0, "MissionBits8", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat missionBits9 = new ClassStat(545, 0, "MissionBits9", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat monsterData = new ClassStat(359, 0, "MonsterData", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat monsterLevelsKilled = new ClassStat(
            254, 1234567890, "MonsterLevelsKilled", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat monsterScale = new ClassStat(360, 1234567890, "MonsterScale", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat monsterTexture = new ClassStat(
            344, 1234567890, "MonsterTexture", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat monthsPaid = new ClassStat(69, 0, "MonthsPaid", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat moreFlags = new ClassStat(177, 1234567890, "MoreFlags", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat multipleCount = new ClassStat(412, 1234567890, "MultipleCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat name = new ClassStat(14, 1234567890, "Name", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nameTemplate = new ClassStat(446, 1234567890, "NameTemplate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill nanoAC = new StatSkill(168, 5, "NanoAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoDamageModifier = new ClassStat(315, 0, "NanoDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoDamageMultiplier = new ClassStat(
            536, 0, "NanoDamageMultiplier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNanoDelta nanoDelta = new StatNanoDelta(364, 1234567890, "NanoDelta", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill nanoEnergyPool = new StatSkill(132, 5, "NanoEnergyPool", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoFocusLevel = new ClassStat(355, 0, "NanoFocusLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNanoInterval nanoInterval = new StatNanoInterval(
            363, 28, "NanoInterval", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoPoints = new ClassStat(407, 1234567890, "NanoPoints", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill nanoProgramming = new StatSkill(160, 5, "NanoProgramming", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill nanoProwessInitiative = new StatSkill(
            149, 5, "NanoProwessInitiative", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoSpeed = new ClassStat(406, 1234567890, "NanoSpeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nanoVulnerability = new ClassStat(
            537, 1234567890, "NanoVulnerability", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat newbieHP = new ClassStat(600, 1234567890, "NewbieHP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat newbieNP = new ClassStat(603, 1234567890, "NewbieNP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nextDoorInBuilding = new ClassStat(
            190, 1234567890, "NextDoorInBuilding", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat nextFormula = new ClassStat(411, 1234567890, "NextFormula", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNextSK nextSK = new StatNextSK(575, 0, "NextSK", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatNextXP nextXP = new StatNextXP(350, 1450, "NextXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npCostModifier = new ClassStat(318, 0, "NPCostModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npLevelUp = new ClassStat(604, 1234567890, "NPLevelUp", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npPerSkill = new ClassStat(605, 1234567890, "NPPerSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcBrainState = new ClassStat(429, 1234567890, "NPCBrainState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcCommand = new ClassStat(439, 1234567890, "NPCCommand", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcCommandArg = new ClassStat(445, 1234567890, "NPCCommandArg", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcCryForHelpRange = new ClassStat(
            465, 1234567890, "NPCCryForHelpRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcFamily = new ClassStat(455, 1234567890, "NPCFamily", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcFlags = new ClassStat(179, 1234567890, "NPCFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcFovStatus = new ClassStat(533, 1234567890, "NPCFovStatus", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcHasPatrolList = new ClassStat(
            452, 1234567890, "NPCHasPatrolList", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcHash = new ClassStat(356, 1234567890, "NPCHash", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcHatelistSize = new ClassStat(
            457, 1234567890, "NPCHatelistSize", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcIsSurrendering = new ClassStat(
            449, 1234567890, "NPCIsSurrendering", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcNumPets = new ClassStat(458, 1234567890, "NPCNumPets", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcScriptAmsScale = new ClassStat(
            581, 1234567890, "NPCScriptAMSScale", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcSpellArg1 = new ClassStat(638, 1234567890, "NPCSpellArg1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcSpellRet1 = new ClassStat(639, 1234567890, "NPCSpellRet1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcSurrenderInstance = new ClassStat(
            451, 1234567890, "NPCSurrenderInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcUseFightModeRegenRate = new ClassStat(
            519, 1234567890, "NPCUseFightModeRegenRate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcVicinityChars = new ClassStat(
            453, 1234567890, "NPCVicinityChars", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcVicinityFamily = new ClassStat(
            580, 1234567890, "NPCVicinityFamily", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat npcVicinityPlayers = new ClassStat(
            518, 1234567890, "NPCVicinityPlayers", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat numAttackEffects = new ClassStat(
            291, 1234567890, "NumAttackEffects", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat numberOfItems = new ClassStat(396, 1234567890, "NumberOfItems", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat numberOfTeamMembers = new ClassStat(
            587, 1234567890, "NumberOfTeamMembers", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat numberOnHateList = new ClassStat(
            529, 1234567890, "NumberOnHateList", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat objectType = new ClassStat(1001, 1234567890, "Type", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat odMaxSizeAdd = new ClassStat(463, 1234567890, "ODMaxSizeAdd", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat odMinSizeAdd = new ClassStat(459, 1234567890, "ODMinSizeAdd", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat oldTimeExist = new ClassStat(392, 1234567890, "OldTimeExist", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat onTowerCreation = new ClassStat(
            513, 1234567890, "OnTowerCreation", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill onehBluntWeapons = new StatSkill(102, 5, "1hBluntWeapons", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill onehEdgedWeapon = new StatSkill(103, 5, "1hEdgedWeapon", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat orientationMode = new ClassStat(
            197, 1234567890, "OrientationMode", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat originatorType = new ClassStat(
            490, 1234567890, "OriginatorType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otArmedForces = new ClassStat(560, 0, "OTArmedForces", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otFollowers = new ClassStat(567, 0, "OTFollowers", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otMed = new ClassStat(562, 1234567890, "OTMed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otOperator = new ClassStat(568, 0, "OTOperator", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otTrans = new ClassStat(564, 0, "OTTrans", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat otUnredeemed = new ClassStat(569, 0, "OTUnredeemed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat outerRadius = new ClassStat(358, 1234567890, "OuterRadius", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideMaterial = new ClassStat(
            337, 1234567890, "OverrideMaterial", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTexture = new ClassStat(
            336, 1234567890, "OverrideTexture", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureAttractor = new ClassStat(
            1014, 0, "OverrideTextureAttractor", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureBack = new ClassStat(
            1013, 0, "OverrideTextureBack", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureHead = new ClassStat(
            1008, 0, "OverrideTextureHead", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureShoulderpadLeft = new ClassStat(
            1012, 0, "OverrideTextureShoulderpadLeft", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureShoulderpadRight = new ClassStat(
            1011, 0, "OverrideTextureShoulderpadRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureWeaponLeft = new ClassStat(
            1010, 0, "OverrideTextureWeaponLeft", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat overrideTextureWeaponRight = new ClassStat(
            1009, 0, "OverrideTextureWeaponRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat ownedTowers = new ClassStat(514, 1234567890, "OwnedTowers", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat ownerInstance = new ClassStat(433, 1234567890, "OwnerInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat paidPoints = new ClassStat(672, 0, "PaidPoints", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat parentInstance = new ClassStat(44, 1234567890, "ParentInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat parentType = new ClassStat(43, 1234567890, "ParentType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill parry = new StatSkill(145, 5, "Parry", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentChemicalDamage = new ClassStat(
            628, 1234567890, "PercentChemicalDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentColdDamage = new ClassStat(
            622, 1234567890, "PercentColdDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentEnergyDamage = new ClassStat(
            627, 1234567890, "PercentEnergyDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentFireDamage = new ClassStat(
            621, 1234567890, "PercentFireDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentMeleeDamage = new ClassStat(
            623, 1234567890, "PercentMeleeDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentPoisonDamage = new ClassStat(
            625, 1234567890, "PercentPoisonDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentProjectileDamage = new ClassStat(
            624, 1234567890, "PercentProjectileDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentRadiationDamage = new ClassStat(
            626, 1234567890, "PercentRadiationDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentRemainingHealth = new ClassStat(
            525, 1234567890, "PercentRemainingHealth", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat percentRemainingNano = new ClassStat(
            526, 1234567890, "PercentRemainingNano", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill perception = new StatSkill(136, 5, "Perception", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat personalResearchGoal = new ClassStat(
            265, 0, "PersonalResearchGoal", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat personalResearchLevel = new ClassStat(
            263, 0, "PersonalResearchLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petCounter = new ClassStat(251, 1234567890, "PetCounter", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petMaster = new ClassStat(196, 1234567890, "PetMaster", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReq1 = new ClassStat(467, 1234567890, "PetReq1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReq2 = new ClassStat(468, 1234567890, "PetReq2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReq3 = new ClassStat(469, 1234567890, "PetReq3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReqVal1 = new ClassStat(485, 1234567890, "PetReqVal1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReqVal2 = new ClassStat(486, 1234567890, "PetReqVal2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petReqVal3 = new ClassStat(487, 1234567890, "PetReqVal3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petState = new ClassStat(671, 1234567890, "PetState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat petType = new ClassStat(512, 1234567890, "PetType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill pharmaceuticals = new StatSkill(159, 5, "Pharmaceuticals", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill physicalProwessInitiative = new StatSkill(
            120, 5, "PhysicalProwessInitiative", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill piercing = new StatSkill(106, 5, "Piercing", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill pistol = new StatSkill(112, 5, "Pistol", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat placement = new ClassStat(298, 1234567890, "Placement", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat playerId = new ClassStat(607, 1234567890, "PlayerID", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat playerKilling = new ClassStat(323, 1234567890, "PlayerKilling", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat playerOptions = new ClassStat(576, 0, "PlayerOptions", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat playfieldType = new ClassStat(438, 1234567890, "PlayfieldType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat poisonAC = new ClassStat(96, 0, "PoisonAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat poisonDamageModifier = new ClassStat(
            317, 0, "PoisonDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat prevMovementMode = new ClassStat(174, 3, "PrevMovementMode", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat previousHealth = new ClassStat(11, 50, "PreviousHealth", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat price = new ClassStat(74, 1234567890, "Price", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat primaryItemInstance = new ClassStat(
            81, 1234567890, "PrimaryItemInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat primaryItemType = new ClassStat(
            80, 1234567890, "PrimaryItemType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat primaryTemplateId = new ClassStat(
            395, 1234567890, "PrimaryTemplateID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procChance1 = new ClassStat(556, 1234567890, "ProcChance1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procChance2 = new ClassStat(557, 1234567890, "ProcChance2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procChance3 = new ClassStat(558, 1234567890, "ProcChance3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procChance4 = new ClassStat(559, 1234567890, "ProcChance4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procInitiative1 = new ClassStat(
            539, 1234567890, "ProcInitiative1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procInitiative2 = new ClassStat(
            540, 1234567890, "ProcInitiative2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procInitiative3 = new ClassStat(
            541, 1234567890, "ProcInitiative3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procInitiative4 = new ClassStat(
            542, 1234567890, "ProcInitiative4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procNano1 = new ClassStat(552, 1234567890, "ProcNano1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procNano2 = new ClassStat(553, 1234567890, "ProcNano2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procNano3 = new ClassStat(554, 1234567890, "ProcNano3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat procNano4 = new ClassStat(555, 1234567890, "ProcNano4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat profession = new ClassStat(60, 1234567890, "Profession", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat professionLevel = new ClassStat(
            10, 1234567890, "ProfessionLevel", false, true, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat projectileAC = new ClassStat(90, 0, "ProjectileAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat projectileDamageModifier = new ClassStat(
            278, 0, "ProjectileDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat proximityRangeIndoors = new ClassStat(
            484, 1234567890, "ProximityRangeIndoors", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat proximityRangeOutdoors = new ClassStat(
            454, 1234567890, "ProximityRangeOutdoors", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat psychic = new ClassStat(21, 0, "Psychic", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill psychologicalModification = new StatSkill(
            129, 5, "PsychologicalModification", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill psychology = new StatSkill(162, 5, "Psychology", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvPLevelsKilled = new ClassStat(
            255, 1234567890, "PvPLevelsKilled", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpDuelDeaths = new ClassStat(675, 0, "PVPDuelDeaths", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpDuelKills = new ClassStat(674, 0, "PVPDuelKills", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpDuelScore = new ClassStat(684, 0, "PVPDuelScore", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpProfessionDuelDeaths = new ClassStat(
            677, 0, "PVPProfessionDuelDeaths", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpProfessionDuelKills = new ClassStat(
            676, 0, "PVPProfessionDuelKills", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpRankedSoloDeaths = new ClassStat(
            679, 0, "PVPRankedSoloDeaths", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpRankedSoloKills = new ClassStat(678, 0, "PVPRankedSoloKills", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpRankedTeamDeaths = new ClassStat(
            681, 0, "PVPRankedTeamDeaths", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpRankedTeamKills = new ClassStat(680, 0, "PVPRankedTeamKills", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpRating = new ClassStat(333, 1300, "PvP_Rating", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpSoloScore = new ClassStat(682, 0, "PVPSoloScore", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat pvpTeamScore = new ClassStat(683, 0, "PVPTeamScore", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtDungeonInstance = new ClassStat(
            497, 1234567890, "QTDungeonInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterCount1 = new ClassStat(
            504, 1234567890, "QTKillNumMonsterCount1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterCount2 = new ClassStat(
            506, 1234567890, "QTKillNumMonsterCount2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterCount3 = new ClassStat(
            508, 1234567890, "QTKillNumMonsterCount3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterID3 = new ClassStat(
            507, 1234567890, "QTKillNumMonsterID3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterId1 = new ClassStat(
            503, 1234567890, "QTKillNumMonsterID1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKillNumMonsterId2 = new ClassStat(
            505, 1234567890, "QTKillNumMonsterID2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtKilledMonsters = new ClassStat(
            499, 1234567890, "QTKilledMonsters", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat qtNumMonsters = new ClassStat(498, 1234567890, "QTNumMonsters", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questAsMaximumRange = new ClassStat(
            802, 1234567890, "QuestASMaximumRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questAsMinimumRange = new ClassStat(
            801, 1234567890, "QuestASMinimumRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questBoothDifficulty = new ClassStat(
            800, 1234567890, "QuestBoothDifficulty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex0 = new ClassStat(509, 1234567890, "QuestIndex0", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex1 = new ClassStat(492, 1234567890, "QuestIndex1", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex2 = new ClassStat(493, 1234567890, "QuestIndex2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex3 = new ClassStat(494, 1234567890, "QuestIndex3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex4 = new ClassStat(495, 1234567890, "QuestIndex4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questIndex5 = new ClassStat(496, 1234567890, "QuestIndex5", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questInstance = new ClassStat(491, 1234567890, "QuestInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questLevelsSolved = new ClassStat(
            253, 1234567890, "QuestLevelsSolved", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questStat = new ClassStat(261, 1234567890, "QuestStat", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat questTimeout = new ClassStat(510, 1234567890, "QuestTimeout", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat race = new ClassStat(89, 1, "Race", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat radiationAC = new ClassStat(94, 0, "RadiationAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat radiationDamageModifier = new ClassStat(
            282, 0, "RadiationDamageModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rangeIncreaserNF = new ClassStat(381, 0, "RangeIncreaserNF", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rangeIncreaserWeapon = new ClassStat(
            380, 0, "RangeIncreaserWeapon", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat readOnly = new ClassStat(435, 1234567890, "ReadOnly", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rechargeDelay = new ClassStat(210, 1234567890, "RechargeDelay", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rechargeDelayCap = new ClassStat(
            524, 1234567890, "RechargeDelayCap", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reclaimItem = new ClassStat(365, 1234567890, "ReclaimItem", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectChemicalAC = new ClassStat(208, 0, "ReflectChemicalAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectColdAC = new ClassStat(217, 0, "ReflectColdAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectEnergyAC = new ClassStat(207, 0, "ReflectEnergyAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectFireAC = new ClassStat(219, 0, "ReflectFireAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectMeleeAC = new ClassStat(206, 0, "ReflectMeleeAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectNanoAC = new ClassStat(218, 0, "ReflectNanoAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectPoisonAC = new ClassStat(225, 0, "ReflectPoisonAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectProjectileAC = new ClassStat(
            205, 0, "ReflectProjectileAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectRadiationAC = new ClassStat(216, 0, "ReflectRadiationAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedChemicalAC = new ClassStat(
            478, 0, "ReflectReturnedChemicalAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedColdAC = new ClassStat(
            480, 0, "ReflectReturnedColdAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedEnergyAC = new ClassStat(
            477, 0, "ReflectReturnedEnergyAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedFireAC = new ClassStat(
            482, 0, "ReflectReturnedFireAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedMeleeAC = new ClassStat(
            476, 0, "ReflectReturnedMeleeAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedNanoAC = new ClassStat(
            481, 0, "ReflectReturnedNanoAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedPoisonAC = new ClassStat(
            483, 0, "ReflectReturnedPoisonAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedProjectileAC = new ClassStat(
            475, 0, "ReflectReturnedProjectileAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat reflectReturnedRadiationAC = new ClassStat(
            479, 0, "ReflectReturnedRadiationAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat regainXPPercentage = new ClassStat(593, 0, "RegainXPPercentage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat repairDifficulty = new ClassStat(
            73, 1234567890, "RepairDifficulty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat repairSkill = new ClassStat(77, 1234567890, "RepairSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat resistModifier = new ClassStat(
            393, 1234567890, "ResistModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat restModifier = new ClassStat(425, 1234567890, "RestModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat resurrectDest = new ClassStat(362, 1234567890, "ResurrectDest", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill rifle = new StatSkill(113, 5, "Rifle", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill riposte = new StatSkill(143, 5, "Riposte", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat ritualTargetInst = new ClassStat(
            370, 1234567890, "RitualTargetInst", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rnd = new ClassStat(520, 1234567890, "Rnd", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rotation = new ClassStat(400, 1234567890, "Rotation", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat rp = new ClassStat(199, 0, "RP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill runSpeed = new StatSkill(156, 5, "RunSpeed", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat savedXP = new ClassStat(334, 0, "SavedXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat school = new ClassStat(405, 1234567890, "School", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat secondaryItemInstance = new ClassStat(
            83, 1234567890, "SecondaryItemInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat secondaryItemTemplate = new ClassStat(
            273, 1234567890, "SecondaryItemTemplate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat secondaryItemType = new ClassStat(
            82, 1234567890, "SecondaryItemType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat selectedTarget = new ClassStat(
            431, 1234567890, "SelectedTarget", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat selectedTargetType = new ClassStat(
            397, 1234567890, "SelectedTargetType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sellModifier = new ClassStat(427, 1234567890, "SellModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sense = new ClassStat(20, 0, "Sense", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill senseImprovement = new StatSkill(122, 5, "SenseImprovement", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sessionTime = new ClassStat(198, 1234567890, "SessionTime", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sex = new ClassStat(59, 1234567890, "Sex", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shadowBreed = new ClassStat(532, 0, "ShadowBreed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shadowBreedTemplate = new ClassStat(
            579, 0, "ShadowBreedTemplate", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldChemicalAC = new ClassStat(229, 0, "ShieldChemicalAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldColdAC = new ClassStat(231, 0, "ShieldColdAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldEnergyAC = new ClassStat(228, 0, "ShieldEnergyAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldFireAC = new ClassStat(233, 0, "ShieldFireAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldMeleeAC = new ClassStat(227, 0, "ShieldMeleeAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldNanoAC = new ClassStat(232, 0, "ShieldNanoAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldPoisonAC = new ClassStat(234, 0, "ShieldPoisonAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldProjectileAC = new ClassStat(226, 0, "ShieldProjectileAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shieldRadiationAC = new ClassStat(230, 0, "ShieldRadiationAC", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopFlags = new ClassStat(610, 1234567890, "ShopFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopId = new ClassStat(657, 1234567890, "ShopID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopIndex = new ClassStat(656, 1234567890, "ShopIndex", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopLastUsed = new ClassStat(611, 1234567890, "ShopLastUsed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopPrice = new ClassStat(599, 1234567890, "ShopPrice", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopRent = new ClassStat(608, 1234567890, "ShopRent", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shopType = new ClassStat(612, 1234567890, "ShopType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill shotgun = new StatSkill(115, 5, "Shotgun", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shoulderMeshHolder = new ClassStat(39, 0, "WeaponMeshRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shoulderMeshLeft = new ClassStat(1005, 0, "ShoulderMeshLeft", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat shoulderMeshRight = new ClassStat(1004, 0, "ShoulderMeshRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat side = new ClassStat(33, 0, "Side", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sisCap = new ClassStat(352, 1234567890, "SISCap", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat sk = new ClassStat(573, 0, "SK", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat skillDisabled = new ClassStat(329, 1234567890, "SkillDisabled", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat skillLockModifier = new ClassStat(382, 0, "SkillLockModifier", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat skillTimeOnSelectedTarget = new ClassStat(
            371, 1234567890, "SkillTimeOnSelectedTarget", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill sneakAttack = new StatSkill(146, 5, "SneakAttack", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat socialStatus = new ClassStat(521, 0, "SocialStatus", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat soundVolume = new ClassStat(250, 1234567890, "SoundVolume", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat specialAttackShield = new ClassStat(
            517, 1234567890, "SpecialAttackShield", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat specialCondition = new ClassStat(348, 1, "SpecialCondition", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat specialization = new ClassStat(182, 0, "Specialization", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat speedPenalty = new ClassStat(70, 1234567890, "SpeedPenalty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stability = new ClassStat(202, 1234567890, "Stability", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingLine2 = new ClassStat(546, 1234567890, "StackingLine2", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingLine3 = new ClassStat(547, 1234567890, "StackingLine3", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingLine4 = new ClassStat(548, 1234567890, "StackingLine4", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingLine5 = new ClassStat(549, 1234567890, "StackingLine5", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingLine6 = new ClassStat(550, 1234567890, "StackingLine6", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stackingOrder = new ClassStat(551, 1234567890, "StackingOrder", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stamina = new ClassStat(18, 0, "Stamina", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat statOne = new ClassStat(290, 1234567890, "StatOne", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat state = new ClassStat(7, 0, "State", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stateAction = new ClassStat(98, 1234567890, "StateAction", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat stateMachine = new ClassStat(450, 1234567890, "StateMachine", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat staticInstance = new ClassStat(23, 1234567890, "StaticInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat staticType = new ClassStat(25, 1234567890, "StaticType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat streamCheckMagic = new ClassStat(
            999, 1234567890, "StreamCheckMagic", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat strength = new ClassStat(16, 0, "Strength", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill subMachineGun = new StatSkill(114, 5, "SubMachineGun", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill swim = new StatSkill(138, 5, "Swim", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat synergyHash = new ClassStat(609, 1234567890, "SynergyHash", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat taboo = new ClassStat(327, 1234567890, "Taboo", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat targetDistance = new ClassStat(
            527, 1234567890, "TargetDistance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat targetDistanceChange = new ClassStat(
            889, 1234567890, "TargetDistanceChange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat targetFacing = new ClassStat(488, 1234567890, "TargetFacing", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat team = new ClassStat(6, 0, "Team", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat teamAllowed = new ClassStat(324, 1234567890, "TeamAllowed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat teamCloseness = new ClassStat(528, 1234567890, "TeamCloseness", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat teamSide = new ClassStat(213, 0, "TeamSide", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat teleportPauseMilliSeconds = new ClassStat(
            351, 1234567890, "TeleportPauseMilliSeconds", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tempSavePlayfield = new ClassStat(595, 0, "TempSavePlayfield", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tempSaveTeamId = new ClassStat(594, 0, "TempSaveTeamID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tempSaveX = new ClassStat(596, 0, "TempSaveX", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tempSaveY = new ClassStat(597, 0, "TempSaveY", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat temporarySkillReduction = new ClassStat(
            247, 0, "TemporarySkillReduction", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill throwingKnife = new StatSkill(108, 5, "ThrowingKnife", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill thrownGrapplingWeapons = new StatSkill(
            110, 5, "ThrownGrapplingWeapons", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tideRequiredDynelId = new ClassStat(
            900, 1234567890, "TideRequiredDynelID", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat timeExist = new ClassStat(8, 1234567890, "TimeExist", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat timeSinceCreation = new ClassStat(
            56, 1234567890, "TimeSinceCreation", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat timeSinceUpkeep = new ClassStat(
            313, 1234567890, "TimeSinceUpkeep", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatTitleLevel titleLevel = new StatTitleLevel(37, 1, "TitleLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat totalDamage = new ClassStat(629, 1234567890, "TotalDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat totalMass = new ClassStat(71, 1234567890, "TotalMass", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat towerInstance = new ClassStat(515, 1234567890, "TowerInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat towerNpcHash = new ClassStat(511, 1234567890, "Tower_NPCHash", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat towerType = new ClassStat(388, 1234567890, "TowerType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tracerEffectType = new ClassStat(
            419, 1234567890, "TracerEffectType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackChemicalDamage = new ClassStat(
            633, 1234567890, "TrackChemicalDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackColdDamage = new ClassStat(
            635, 1234567890, "TrackColdDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackEnergyDamage = new ClassStat(
            632, 1234567890, "TrackEnergyDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackFireDamage = new ClassStat(
            637, 1234567890, "TrackFireDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackMeleeDamage = new ClassStat(
            631, 1234567890, "TrackMeleeDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackPoisonDamage = new ClassStat(
            636, 1234567890, "TrackPoisonDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackProjectileDamage = new ClassStat(
            630, 1234567890, "TrackProjectileDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trackRadiationDamage = new ClassStat(
            634, 1234567890, "TrackRadiationDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat tradeLimit = new ClassStat(346, 1234567890, "TradeLimit", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trainSkill = new ClassStat(408, 1234567890, "TrainSkill", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trainSkillCost = new ClassStat(
            409, 1234567890, "TrainSkillCost", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat trapDifficulty = new ClassStat(
            289, 1234567890, "TrapDifficulty", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat travelSound = new ClassStat(271, 1234567890, "TravelSound", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill treatment = new StatSkill(124, 5, "Treatment", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat turnSpeed = new ClassStat(267, 40000, "TurnSpeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill tutoring = new StatSkill(141, 5, "Tutoring", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill twohBluntWeapons = new StatSkill(107, 5, "2hBluntWeapons", true, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill twohEdgedWeapons = new StatSkill(105, 5, "2hEdgedWeapons", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat unarmedTemplateInstance = new ClassStat(
            418, 0, "UnarmedTemplateInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat unreadMailCount = new ClassStat(649, 0, "UnreadMailCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat unsavedXP = new ClassStat(592, 0, "UnsavedXP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat userInstance = new ClassStat(85, 1234567890, "UserInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat userType = new ClassStat(84, 1234567890, "UserType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat vehicleAC = new ClassStat(664, 1234567890, "VehicleAC", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat vehicleDamage = new ClassStat(665, 1234567890, "VehicleDamage", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat vehicleHealth = new ClassStat(666, 1234567890, "VehicleHealth", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat vehicleSpeed = new ClassStat(667, 1234567890, "VehicleSpeed", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat veteranPoints = new ClassStat(68, 0, "VeteranPoints", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat vicinityRange = new ClassStat(448, 1234567890, "VicinityRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat victoryPoints = new ClassStat(669, 0, "VP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat visualBreed = new ClassStat(367, 1234567890, "VisualBreed", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat visualFlags = new ClassStat(673, 31, "VisualFlags", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat visualLodLevel = new ClassStat(
            888, 1234567890, "VisualLODLevel", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat visualProfession = new ClassStat(
            368, 1234567890, "VisualProfession", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat visualSex = new ClassStat(369, 1234567890, "VisualSex", false, false, true);

        /// <summary>
        /// </summary>
        private readonly ClassStat volumeMass = new ClassStat(2, 1234567890, "VolumeMass", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat voteCount = new ClassStat(306, 1234567890, "VoteCount", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat waitState = new ClassStat(430, 2, "WaitState", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponDisallowedInstance = new ClassStat(
            326, 1234567890, "WeaponDisallowedInstance", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponDisallowedType = new ClassStat(
            325, 1234567890, "WeaponDisallowedType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponMeshHolder = new ClassStat(209, 0, "WeaponMeshRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponMeshLeft = new ClassStat(1007, 0, "WeaponMeshLeft", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponMeshRight = new ClassStat(1006, 0, "WeaponMeshRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly StatSkill weaponSmithing = new StatSkill(158, 5, "WeaponSmithing", true, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponStyleLeft = new ClassStat(1015, 0, "WeaponStyleLeft", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponStyleRight = new ClassStat(1016, 0, "WeaponStyleRight", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat weaponsStyle = new ClassStat(1003, 1234567890, "WeaponType", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat xp = new ClassStat(52, 0, "XP", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat xpBonus = new ClassStat(341, 1234567890, "XPBonus", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat xpKillRange = new ClassStat(275, 5, "XPKillRange", false, false, false);

        /// <summary>
        /// </summary>
        private readonly ClassStat xpModifier = new ClassStat(319, 0, "XPModifier", false, false, false);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Character_Stats
        /// Class for character's stats
        /// </summary>
        /// <param name="parent">
        /// Stat's owner (Character or derived class)
        /// </param>
        public DynelStats(Dynel parent)
        {
            this.all.Add(this.flags);
            this.all.Add(this.life);
            this.all.Add(this.volumeMass);
            this.all.Add(this.attackSpeed);
            this.all.Add(this.breed);
            this.all.Add(this.clan);
            this.all.Add(this.team);
            this.all.Add(this.state);
            this.all.Add(this.timeExist);
            this.all.Add(this.mapFlags);
            this.all.Add(this.professionLevel);
            this.all.Add(this.previousHealth);
            this.all.Add(this.mesh);
            this.all.Add(this.anim);
            this.all.Add(this.name);
            this.all.Add(this.info);
            this.all.Add(this.strength);
            this.all.Add(this.agility);
            this.all.Add(this.stamina);
            this.all.Add(this.intelligence);
            this.all.Add(this.sense);
            this.all.Add(this.psychic);
            this.all.Add(this.ams);
            this.all.Add(this.staticInstance);
            this.all.Add(this.maxMass);
            this.all.Add(this.staticType);
            this.all.Add(this.energy);
            this.all.Add(this.health);
            this.all.Add(this.height);
            this.all.Add(this.dms);
            this.all.Add(this.can);
            this.all.Add(this.face);
            this.all.Add(this.hairMesh);
            this.all.Add(this.side);
            this.all.Add(this.deadTimer);
            this.all.Add(this.accessCount);
            this.all.Add(this.attackCount);
            this.all.Add(this.titleLevel);
            this.all.Add(this.backMesh);
            this.all.Add(this.alienXP);
            this.all.Add(this.fabricType);
            this.all.Add(this.catMesh);
            this.all.Add(this.parentType);
            this.all.Add(this.parentInstance);
            this.all.Add(this.beltSlots);
            this.all.Add(this.bandolierSlots);
            this.all.Add(this.fatness);
            this.all.Add(this.clanLevel);
            this.all.Add(this.insuranceTime);
            this.all.Add(this.inventoryTimeout);
            this.all.Add(this.aggDef);
            this.all.Add(this.xp);
            this.all.Add(this.ip);
            this.all.Add(this.level);
            this.all.Add(this.inventoryId);
            this.all.Add(this.timeSinceCreation);
            this.all.Add(this.lastXP);
            this.all.Add(this.age);
            this.all.Add(this.sex);
            this.all.Add(this.profession);
            this.all.Add(this.cash);
            this.all.Add(this.alignment);
            this.all.Add(this.attitude);
            this.all.Add(this.headMesh);
            this.all.Add(this.missionBits5);
            this.all.Add(this.missionBits6);
            this.all.Add(this.missionBits7);
            this.all.Add(this.veteranPoints);
            this.all.Add(this.monthsPaid);
            this.all.Add(this.speedPenalty);
            this.all.Add(this.totalMass);
            this.all.Add(this.itemType);
            this.all.Add(this.repairDifficulty);
            this.all.Add(this.price);
            this.all.Add(this.metaType);
            this.all.Add(this.itemClass);
            this.all.Add(this.repairSkill);
            this.all.Add(this.currentMass);
            this.all.Add(this.icon);
            this.all.Add(this.primaryItemType);
            this.all.Add(this.primaryItemInstance);
            this.all.Add(this.secondaryItemType);
            this.all.Add(this.secondaryItemInstance);
            this.all.Add(this.userType);
            this.all.Add(this.userInstance);
            this.all.Add(this.areaType);
            this.all.Add(this.areaInstance);
            this.all.Add(this.defaultPos);
            this.all.Add(this.race);
            this.all.Add(this.projectileAC);
            this.all.Add(this.meleeAC);
            this.all.Add(this.energyAC);
            this.all.Add(this.chemicalAC);
            this.all.Add(this.radiationAC);
            this.all.Add(this.coldAC);
            this.all.Add(this.poisonAC);
            this.all.Add(this.fireAC);
            this.all.Add(this.stateAction);
            this.all.Add(this.itemAnim);
            this.all.Add(this.martialArts);
            this.all.Add(this.meleeMultiple);
            this.all.Add(this.onehBluntWeapons);
            this.all.Add(this.onehEdgedWeapon);
            this.all.Add(this.meleeEnergyWeapon);
            this.all.Add(this.twohEdgedWeapons);
            this.all.Add(this.piercing);
            this.all.Add(this.twohBluntWeapons);
            this.all.Add(this.throwingKnife);
            this.all.Add(this.grenade);
            this.all.Add(this.thrownGrapplingWeapons);
            this.all.Add(this.bow);
            this.all.Add(this.pistol);
            this.all.Add(this.rifle);
            this.all.Add(this.subMachineGun);
            this.all.Add(this.shotgun);
            this.all.Add(this.assaultRifle);
            this.all.Add(this.driveWater);
            this.all.Add(this.closeCombatInitiative);
            this.all.Add(this.distanceWeaponInitiative);
            this.all.Add(this.physicalProwessInitiative);
            this.all.Add(this.bowSpecialAttack);
            this.all.Add(this.senseImprovement);
            this.all.Add(this.firstAid);
            this.all.Add(this.treatment);
            this.all.Add(this.mechanicalEngineering);
            this.all.Add(this.electricalEngineering);
            this.all.Add(this.materialMetamorphose);
            this.all.Add(this.biologicalMetamorphose);
            this.all.Add(this.psychologicalModification);
            this.all.Add(this.materialCreation);
            this.all.Add(this.materialLocation);
            this.all.Add(this.nanoEnergyPool);
            this.all.Add(this.lrEnergyWeapon);
            this.all.Add(this.lrMultipleWeapon);
            this.all.Add(this.disarmTrap);
            this.all.Add(this.perception);
            this.all.Add(this.adventuring);
            this.all.Add(this.swim);
            this.all.Add(this.driveAir);
            this.all.Add(this.mapNavigation);
            this.all.Add(this.tutoring);
            this.all.Add(this.brawl);
            this.all.Add(this.riposte);
            this.all.Add(this.dimach);
            this.all.Add(this.parry);
            this.all.Add(this.sneakAttack);
            this.all.Add(this.fastAttack);
            this.all.Add(this.burst);
            this.all.Add(this.nanoProwessInitiative);
            this.all.Add(this.flingShot);
            this.all.Add(this.aimedShot);
            this.all.Add(this.bodyDevelopment);
            this.all.Add(this.duck);
            this.all.Add(this.dodge);
            this.all.Add(this.evade);
            this.all.Add(this.runSpeed);
            this.all.Add(this.fieldQuantumPhysics);
            this.all.Add(this.weaponSmithing);
            this.all.Add(this.pharmaceuticals);
            this.all.Add(this.nanoProgramming);
            this.all.Add(this.computerLiteracy);
            this.all.Add(this.psychology);
            this.all.Add(this.chemistry);
            this.all.Add(this.concealment);
            this.all.Add(this.breakingEntry);
            this.all.Add(this.driveGround);
            this.all.Add(this.fullAuto);
            this.all.Add(this.nanoAC);
            this.all.Add(this.alienLevel);
            this.all.Add(this.healthChangeBest);
            this.all.Add(this.healthChangeWorst);
            this.all.Add(this.healthChange);
            this.all.Add(this.currentMovementMode);
            this.all.Add(this.prevMovementMode);
            this.all.Add(this.autoLockTimeDefault);
            this.all.Add(this.autoUnlockTimeDefault);
            this.all.Add(this.moreFlags);
            this.all.Add(this.alienNextXP);
            this.all.Add(this.npcFlags);
            this.all.Add(this.currentNCU);
            this.all.Add(this.maxNCU);
            this.all.Add(this.specialization);
            this.all.Add(this.effectIcon);
            this.all.Add(this.buildingType);
            this.all.Add(this.buildingInstance);
            this.all.Add(this.cardOwnerType);
            this.all.Add(this.cardOwnerInstance);
            this.all.Add(this.buildingComplexInst);
            this.all.Add(this.exitInstance);
            this.all.Add(this.nextDoorInBuilding);
            this.all.Add(this.lastConcretePlayfieldInstance);
            this.all.Add(this.extenalPlayfieldInstance);
            this.all.Add(this.extenalDoorInstance);
            this.all.Add(this.inPlay);
            this.all.Add(this.accessKey);
            this.all.Add(this.petMaster);
            this.all.Add(this.orientationMode);
            this.all.Add(this.sessionTime);
            this.all.Add(this.rp);
            this.all.Add(this.conformity);
            this.all.Add(this.aggressiveness);
            this.all.Add(this.stability);
            this.all.Add(this.extroverty);
            this.all.Add(this.breedHostility);
            this.all.Add(this.reflectProjectileAC);
            this.all.Add(this.reflectMeleeAC);
            this.all.Add(this.reflectEnergyAC);
            this.all.Add(this.reflectChemicalAC);
            this.all.Add(this.rechargeDelay);
            this.all.Add(this.equipDelay);
            this.all.Add(this.maxEnergy);
            this.all.Add(this.teamSide);
            this.all.Add(this.currentNano);
            this.all.Add(this.gmLevel);
            this.all.Add(this.reflectRadiationAC);
            this.all.Add(this.reflectColdAC);
            this.all.Add(this.reflectNanoAC);
            this.all.Add(this.reflectFireAC);
            this.all.Add(this.currBodyLocation);
            this.all.Add(this.maxNanoEnergy);
            this.all.Add(this.accumulatedDamage);
            this.all.Add(this.canChangeClothes);
            this.all.Add(this.features);
            this.all.Add(this.reflectPoisonAC);
            this.all.Add(this.shieldProjectileAC);
            this.all.Add(this.shieldMeleeAC);
            this.all.Add(this.shieldEnergyAC);
            this.all.Add(this.shieldChemicalAC);
            this.all.Add(this.shieldRadiationAC);
            this.all.Add(this.shieldColdAC);
            this.all.Add(this.shieldNanoAC);
            this.all.Add(this.shieldFireAC);
            this.all.Add(this.shieldPoisonAC);
            this.all.Add(this.berserkMode);
            this.all.Add(this.insurancePercentage);
            this.all.Add(this.changeSideCount);
            this.all.Add(this.absorbProjectileAC);
            this.all.Add(this.absorbMeleeAC);
            this.all.Add(this.absorbEnergyAC);
            this.all.Add(this.absorbChemicalAC);
            this.all.Add(this.absorbRadiationAC);
            this.all.Add(this.absorbColdAC);
            this.all.Add(this.absorbFireAC);
            this.all.Add(this.absorbPoisonAC);
            this.all.Add(this.absorbNanoAC);
            this.all.Add(this.temporarySkillReduction);
            this.all.Add(this.birthDate);
            this.all.Add(this.lastSaved);
            this.all.Add(this.soundVolume);
            this.all.Add(this.petCounter);
            this.all.Add(this.metersWalked);
            this.all.Add(this.questLevelsSolved);
            this.all.Add(this.monsterLevelsKilled);
            this.all.Add(this.pvPLevelsKilled);
            this.all.Add(this.missionBits1);
            this.all.Add(this.missionBits2);
            this.all.Add(this.accessGrant);
            this.all.Add(this.doorFlags);
            this.all.Add(this.clanHierarchy);
            this.all.Add(this.questStat);
            this.all.Add(this.clientActivated);
            this.all.Add(this.personalResearchLevel);
            this.all.Add(this.globalResearchLevel);
            this.all.Add(this.personalResearchGoal);
            this.all.Add(this.globalResearchGoal);
            this.all.Add(this.turnSpeed);
            this.all.Add(this.liquidType);
            this.all.Add(this.gatherSound);
            this.all.Add(this.castSound);
            this.all.Add(this.travelSound);
            this.all.Add(this.hitSound);
            this.all.Add(this.secondaryItemTemplate);
            this.all.Add(this.equippedWeapons);
            this.all.Add(this.xpKillRange);
            this.all.Add(this.amsModifier);
            this.all.Add(this.dmsModifier);
            this.all.Add(this.projectileDamageModifier);
            this.all.Add(this.meleeDamageModifier);
            this.all.Add(this.energyDamageModifier);
            this.all.Add(this.chemicalDamageModifier);
            this.all.Add(this.radiationDamageModifier);
            this.all.Add(this.itemHateValue);
            this.all.Add(this.damageBonus);
            this.all.Add(this.maxDamage);
            this.all.Add(this.minDamage);
            this.all.Add(this.attackRange);
            this.all.Add(this.hateValueModifyer);
            this.all.Add(this.trapDifficulty);
            this.all.Add(this.statOne);
            this.all.Add(this.numAttackEffects);
            this.all.Add(this.defaultAttackType);
            this.all.Add(this.itemSkill);
            this.all.Add(this.itemDelay);
            this.all.Add(this.itemOpposedSkill);
            this.all.Add(this.itemSIS);
            this.all.Add(this.interactionRadius);
            this.all.Add(this.placement);
            this.all.Add(this.lockDifficulty);
            this.all.Add(this.members);
            this.all.Add(this.minMembers);
            this.all.Add(this.clanPrice);
            this.all.Add(this.missionBits3);
            this.all.Add(this.clanType);
            this.all.Add(this.clanInstance);
            this.all.Add(this.voteCount);
            this.all.Add(this.memberType);
            this.all.Add(this.memberInstance);
            this.all.Add(this.globalClanType);
            this.all.Add(this.globalClanInstance);
            this.all.Add(this.coldDamageModifier);
            this.all.Add(this.clanUpkeepInterval);
            this.all.Add(this.timeSinceUpkeep);
            this.all.Add(this.clanFinalized);
            this.all.Add(this.nanoDamageModifier);
            this.all.Add(this.fireDamageModifier);
            this.all.Add(this.poisonDamageModifier);
            this.all.Add(this.npCostModifier);
            this.all.Add(this.xpModifier);
            this.all.Add(this.breedLimit);
            this.all.Add(this.genderLimit);
            this.all.Add(this.levelLimit);
            this.all.Add(this.playerKilling);
            this.all.Add(this.teamAllowed);
            this.all.Add(this.weaponDisallowedType);
            this.all.Add(this.weaponDisallowedInstance);
            this.all.Add(this.taboo);
            this.all.Add(this.compulsion);
            this.all.Add(this.skillDisabled);
            this.all.Add(this.clanItemType);
            this.all.Add(this.clanItemInstance);
            this.all.Add(this.debuffFormula);
            this.all.Add(this.pvpRating);
            this.all.Add(this.savedXP);
            this.all.Add(this.doorBlockTime);
            this.all.Add(this.overrideTexture);
            this.all.Add(this.overrideMaterial);
            this.all.Add(this.deathReason);
            this.all.Add(this.damageOverrideType);
            this.all.Add(this.brainType);
            this.all.Add(this.xpBonus);
            this.all.Add(this.healInterval);
            this.all.Add(this.healDelta);
            this.all.Add(this.monsterTexture);
            this.all.Add(this.hasAlwaysLootable);
            this.all.Add(this.tradeLimit);
            this.all.Add(this.faceTexture);
            this.all.Add(this.specialCondition);
            this.all.Add(this.autoAttackFlags);
            this.all.Add(this.nextXP);
            this.all.Add(this.teleportPauseMilliSeconds);
            this.all.Add(this.sisCap);
            this.all.Add(this.animSet);
            this.all.Add(this.attackType);
            this.all.Add(this.nanoFocusLevel);
            this.all.Add(this.npcHash);
            this.all.Add(this.collisionRadius);
            this.all.Add(this.outerRadius);
            this.all.Add(this.monsterData);
            this.all.Add(this.monsterScale);
            this.all.Add(this.hitEffectType);
            this.all.Add(this.resurrectDest);
            this.all.Add(this.nanoInterval);
            this.all.Add(this.nanoDelta);
            this.all.Add(this.reclaimItem);
            this.all.Add(this.gatherEffectType);
            this.all.Add(this.visualBreed);
            this.all.Add(this.visualProfession);
            this.all.Add(this.visualSex);
            this.all.Add(this.ritualTargetInst);
            this.all.Add(this.skillTimeOnSelectedTarget);
            this.all.Add(this.lastSaveXP);
            this.all.Add(this.extendedTime);
            this.all.Add(this.burstRecharge);
            this.all.Add(this.fullAutoRecharge);
            this.all.Add(this.gatherAbstractAnim);
            this.all.Add(this.castTargetAbstractAnim);
            this.all.Add(this.castSelfAbstractAnim);
            this.all.Add(this.criticalIncrease);
            this.all.Add(this.rangeIncreaserWeapon);
            this.all.Add(this.rangeIncreaserNF);
            this.all.Add(this.skillLockModifier);
            this.all.Add(this.interruptModifier);
            this.all.Add(this.acgEntranceStyles);
            this.all.Add(this.chanceOfBreakOnSpellAttack);
            this.all.Add(this.chanceOfBreakOnDebuff);
            this.all.Add(this.dieAnim);
            this.all.Add(this.towerType);
            this.all.Add(this.expansion);
            this.all.Add(this.lowresMesh);
            this.all.Add(this.criticalDecrease);
            this.all.Add(this.oldTimeExist);
            this.all.Add(this.resistModifier);
            this.all.Add(this.chestFlags);
            this.all.Add(this.primaryTemplateId);
            this.all.Add(this.numberOfItems);
            this.all.Add(this.selectedTargetType);
            this.all.Add(this.corpseHash);
            this.all.Add(this.ammoName);
            this.all.Add(this.rotation);
            this.all.Add(this.catAnim);
            this.all.Add(this.catAnimFlags);
            this.all.Add(this.displayCATAnim);
            this.all.Add(this.displayCATMesh);
            this.all.Add(this.school);
            this.all.Add(this.nanoSpeed);
            this.all.Add(this.nanoPoints);
            this.all.Add(this.trainSkill);
            this.all.Add(this.trainSkillCost);
            this.all.Add(this.isFightingMe);
            this.all.Add(this.nextFormula);
            this.all.Add(this.multipleCount);
            this.all.Add(this.effectType);
            this.all.Add(this.impactEffectType);
            this.all.Add(this.corpseType);
            this.all.Add(this.corpseInstance);
            this.all.Add(this.corpseAnimKey);
            this.all.Add(this.unarmedTemplateInstance);
            this.all.Add(this.tracerEffectType);
            this.all.Add(this.ammoType);
            this.all.Add(this.charRadius);
            this.all.Add(this.chanceOfUse);
            this.all.Add(this.currentState);
            this.all.Add(this.armourType);
            this.all.Add(this.restModifier);
            this.all.Add(this.buyModifier);
            this.all.Add(this.sellModifier);
            this.all.Add(this.castEffectType);
            this.all.Add(this.npcBrainState);
            this.all.Add(this.waitState);
            this.all.Add(this.selectedTarget);
            this.all.Add(this.missionBits4);
            this.all.Add(this.ownerInstance);
            this.all.Add(this.charState);
            this.all.Add(this.readOnly);
            this.all.Add(this.damageType);
            this.all.Add(this.collideCheckInterval);
            this.all.Add(this.playfieldType);
            this.all.Add(this.npcCommand);
            this.all.Add(this.initiativeType);
            this.all.Add(this.charTmp1);
            this.all.Add(this.charTmp2);
            this.all.Add(this.charTmp3);
            this.all.Add(this.charTmp4);
            this.all.Add(this.npcCommandArg);
            this.all.Add(this.nameTemplate);
            this.all.Add(this.desiredTargetDistance);
            this.all.Add(this.vicinityRange);
            this.all.Add(this.npcIsSurrendering);
            this.all.Add(this.stateMachine);
            this.all.Add(this.npcSurrenderInstance);
            this.all.Add(this.npcHasPatrolList);
            this.all.Add(this.npcVicinityChars);
            this.all.Add(this.proximityRangeOutdoors);
            this.all.Add(this.npcFamily);
            this.all.Add(this.commandRange);
            this.all.Add(this.npcHatelistSize);
            this.all.Add(this.npcNumPets);
            this.all.Add(this.odMinSizeAdd);
            this.all.Add(this.effectRed);
            this.all.Add(this.effectGreen);
            this.all.Add(this.effectBlue);
            this.all.Add(this.odMaxSizeAdd);
            this.all.Add(this.durationModifier);
            this.all.Add(this.npcCryForHelpRange);
            this.all.Add(this.losHeight);
            this.all.Add(this.petReq1);
            this.all.Add(this.petReq2);
            this.all.Add(this.petReq3);
            this.all.Add(this.mapOptions);
            this.all.Add(this.mapAreaPart1);
            this.all.Add(this.mapAreaPart2);
            this.all.Add(this.fixtureFlags);
            this.all.Add(this.fallDamage);
            this.all.Add(this.reflectReturnedProjectileAC);
            this.all.Add(this.reflectReturnedMeleeAC);
            this.all.Add(this.reflectReturnedEnergyAC);
            this.all.Add(this.reflectReturnedChemicalAC);
            this.all.Add(this.reflectReturnedRadiationAC);
            this.all.Add(this.reflectReturnedColdAC);
            this.all.Add(this.reflectReturnedNanoAC);
            this.all.Add(this.reflectReturnedFireAC);
            this.all.Add(this.reflectReturnedPoisonAC);
            this.all.Add(this.proximityRangeIndoors);
            this.all.Add(this.petReqVal1);
            this.all.Add(this.petReqVal2);
            this.all.Add(this.petReqVal3);
            this.all.Add(this.targetFacing);
            this.all.Add(this.backstab);
            this.all.Add(this.originatorType);
            this.all.Add(this.questInstance);
            this.all.Add(this.questIndex1);
            this.all.Add(this.questIndex2);
            this.all.Add(this.questIndex3);
            this.all.Add(this.questIndex4);
            this.all.Add(this.questIndex5);
            this.all.Add(this.qtDungeonInstance);
            this.all.Add(this.qtNumMonsters);
            this.all.Add(this.qtKilledMonsters);
            this.all.Add(this.animPos);
            this.all.Add(this.animPlay);
            this.all.Add(this.animSpeed);
            this.all.Add(this.qtKillNumMonsterId1);
            this.all.Add(this.qtKillNumMonsterCount1);
            this.all.Add(this.qtKillNumMonsterId2);
            this.all.Add(this.qtKillNumMonsterCount2);
            this.all.Add(this.qtKillNumMonsterID3);
            this.all.Add(this.qtKillNumMonsterCount3);
            this.all.Add(this.questIndex0);
            this.all.Add(this.questTimeout);
            this.all.Add(this.towerNpcHash);
            this.all.Add(this.petType);
            this.all.Add(this.onTowerCreation);
            this.all.Add(this.ownedTowers);
            this.all.Add(this.towerInstance);
            this.all.Add(this.attackShield);
            this.all.Add(this.specialAttackShield);
            this.all.Add(this.npcVicinityPlayers);
            this.all.Add(this.npcUseFightModeRegenRate);
            this.all.Add(this.rnd);
            this.all.Add(this.socialStatus);
            this.all.Add(this.lastRnd);
            this.all.Add(this.itemDelayCap);
            this.all.Add(this.rechargeDelayCap);
            this.all.Add(this.percentRemainingHealth);
            this.all.Add(this.percentRemainingNano);
            this.all.Add(this.targetDistance);
            this.all.Add(this.teamCloseness);
            this.all.Add(this.numberOnHateList);
            this.all.Add(this.conditionState);
            this.all.Add(this.expansionPlayfield);
            this.all.Add(this.shadowBreed);
            this.all.Add(this.npcFovStatus);
            this.all.Add(this.dudChance);
            this.all.Add(this.healMultiplier);
            this.all.Add(this.nanoDamageMultiplier);
            this.all.Add(this.nanoVulnerability);
            this.all.Add(this.amsCap);
            this.all.Add(this.procInitiative1);
            this.all.Add(this.procInitiative2);
            this.all.Add(this.procInitiative3);
            this.all.Add(this.procInitiative4);
            this.all.Add(this.factionModifier);
            this.all.Add(this.missionBits8);
            this.all.Add(this.missionBits9);
            this.all.Add(this.stackingLine2);
            this.all.Add(this.stackingLine3);
            this.all.Add(this.stackingLine4);
            this.all.Add(this.stackingLine5);
            this.all.Add(this.stackingLine6);
            this.all.Add(this.stackingOrder);
            this.all.Add(this.procNano1);
            this.all.Add(this.procNano2);
            this.all.Add(this.procNano3);
            this.all.Add(this.procNano4);
            this.all.Add(this.procChance1);
            this.all.Add(this.procChance2);
            this.all.Add(this.procChance3);
            this.all.Add(this.procChance4);
            this.all.Add(this.otArmedForces);
            this.all.Add(this.clanSentinels);
            this.all.Add(this.otMed);
            this.all.Add(this.clanGaia);
            this.all.Add(this.otTrans);
            this.all.Add(this.clanVanguards);
            this.all.Add(this.gos);
            this.all.Add(this.otFollowers);
            this.all.Add(this.otOperator);
            this.all.Add(this.otUnredeemed);
            this.all.Add(this.clanDevoted);
            this.all.Add(this.clanConserver);
            this.all.Add(this.clanRedeemed);
            this.all.Add(this.sk);
            this.all.Add(this.lastSK);
            this.all.Add(this.nextSK);
            this.all.Add(this.playerOptions);
            this.all.Add(this.lastPerkResetTime);
            this.all.Add(this.currentTime);
            this.all.Add(this.shadowBreedTemplate);
            this.all.Add(this.npcVicinityFamily);
            this.all.Add(this.npcScriptAmsScale);
            this.all.Add(this.apartmentsAllowed);
            this.all.Add(this.apartmentsOwned);
            this.all.Add(this.apartmentAccessCard);
            this.all.Add(this.mapAreaPart3);
            this.all.Add(this.mapAreaPart4);
            this.all.Add(this.numberOfTeamMembers);
            this.all.Add(this.actionCategory);
            this.all.Add(this.currentPlayfield);
            this.all.Add(this.districtNano);
            this.all.Add(this.districtNanoInterval);
            this.all.Add(this.unsavedXP);
            this.all.Add(this.regainXPPercentage);
            this.all.Add(this.tempSaveTeamId);
            this.all.Add(this.tempSavePlayfield);
            this.all.Add(this.tempSaveX);
            this.all.Add(this.tempSaveY);
            this.all.Add(this.extendedFlags);
            this.all.Add(this.shopPrice);
            this.all.Add(this.newbieHP);
            this.all.Add(this.hpLevelUp);
            this.all.Add(this.hpPerSkill);
            this.all.Add(this.newbieNP);
            this.all.Add(this.npLevelUp);
            this.all.Add(this.npPerSkill);
            this.all.Add(this.maxShopItems);
            this.all.Add(this.playerId);
            this.all.Add(this.shopRent);
            this.all.Add(this.synergyHash);
            this.all.Add(this.shopFlags);
            this.all.Add(this.shopLastUsed);
            this.all.Add(this.shopType);
            this.all.Add(this.lockDownTime);
            this.all.Add(this.leaderLockDownTime);
            this.all.Add(this.invadersKilled);
            this.all.Add(this.killedByInvaders);
            this.all.Add(this.missionBits10);
            this.all.Add(this.missionBits11);
            this.all.Add(this.missionBits12);
            this.all.Add(this.houseTemplate);
            this.all.Add(this.percentFireDamage);
            this.all.Add(this.percentColdDamage);
            this.all.Add(this.percentMeleeDamage);
            this.all.Add(this.percentProjectileDamage);
            this.all.Add(this.percentPoisonDamage);
            this.all.Add(this.percentRadiationDamage);
            this.all.Add(this.percentEnergyDamage);
            this.all.Add(this.percentChemicalDamage);
            this.all.Add(this.totalDamage);
            this.all.Add(this.trackProjectileDamage);
            this.all.Add(this.trackMeleeDamage);
            this.all.Add(this.trackEnergyDamage);
            this.all.Add(this.trackChemicalDamage);
            this.all.Add(this.trackRadiationDamage);
            this.all.Add(this.trackColdDamage);
            this.all.Add(this.trackPoisonDamage);
            this.all.Add(this.trackFireDamage);
            this.all.Add(this.npcSpellArg1);
            this.all.Add(this.npcSpellRet1);
            this.all.Add(this.cityInstance);
            this.all.Add(this.distanceToSpawnpoint);
            this.all.Add(this.cityTerminalRechargePercent);
            this.all.Add(this.unreadMailCount);
            this.all.Add(this.lastMailCheckTime);
            this.all.Add(this.advantageHash1);
            this.all.Add(this.advantageHash2);
            this.all.Add(this.advantageHash3);
            this.all.Add(this.advantageHash4);
            this.all.Add(this.advantageHash5);
            this.all.Add(this.shopIndex);
            this.all.Add(this.shopId);
            this.all.Add(this.isVehicle);
            this.all.Add(this.damageToNano);
            this.all.Add(this.accountFlags);
            this.all.Add(this.damageToNanoMultiplier);
            this.all.Add(this.mechData);
            this.all.Add(this.vehicleAC);
            this.all.Add(this.vehicleDamage);
            this.all.Add(this.vehicleHealth);
            this.all.Add(this.vehicleSpeed);
            this.all.Add(this.battlestationSide);
            this.all.Add(this.victoryPoints);
            this.all.Add(this.battlestationRep);
            this.all.Add(this.petState);
            this.all.Add(this.paidPoints);
            this.all.Add(this.visualFlags);
            this.all.Add(this.pvpDuelKills);
            this.all.Add(this.pvpDuelDeaths);
            this.all.Add(this.pvpProfessionDuelKills);
            this.all.Add(this.pvpProfessionDuelDeaths);
            this.all.Add(this.pvpRankedSoloKills);
            this.all.Add(this.pvpRankedSoloDeaths);
            this.all.Add(this.pvpRankedTeamKills);
            this.all.Add(this.pvpRankedTeamDeaths);
            this.all.Add(this.pvpSoloScore);
            this.all.Add(this.pvpTeamScore);
            this.all.Add(this.pvpDuelScore);
            this.all.Add(this.acgItemSeed);
            this.all.Add(this.acgItemLevel);
            this.all.Add(this.acgItemTemplateId);
            this.all.Add(this.acgItemTemplateId2);
            this.all.Add(this.acgItemCategoryId);
            this.all.Add(this.hasKnuBotData);
            this.all.Add(this.questBoothDifficulty);
            this.all.Add(this.questAsMinimumRange);
            this.all.Add(this.questAsMaximumRange);
            this.all.Add(this.visualLodLevel);
            this.all.Add(this.targetDistanceChange);
            this.all.Add(this.tideRequiredDynelId);
            this.all.Add(this.streamCheckMagic);
            this.all.Add(this.objectType);
            this.all.Add(this.instance);
            this.all.Add(this.weaponsStyle);
            this.all.Add(this.shoulderMeshRight);
            this.all.Add(this.shoulderMeshLeft);
            this.all.Add(this.weaponMeshRight);
            this.all.Add(this.weaponMeshLeft);
            this.all.Add(this.overrideTextureAttractor);
            this.all.Add(this.overrideTextureBack);
            this.all.Add(this.overrideTextureHead);
            this.all.Add(this.overrideTextureShoulderpadLeft);
            this.all.Add(this.overrideTextureShoulderpadRight);
            this.all.Add(this.overrideTextureWeaponLeft);
            this.all.Add(this.overrideTextureWeaponRight);

            // add Tricklers, try not to do circulars!!
            this.SetAbilityTricklers();
            this.bodyDevelopment.Affects.Add(this.life.StatNumber);
            this.nanoEnergyPool.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.level.Affects.Add(this.life.StatNumber);
            this.level.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.level.Affects.Add(this.titleLevel.StatNumber);
            this.level.Affects.Add(this.nextSK.StatNumber);
            this.level.Affects.Add(this.nextXP.StatNumber);
            this.alienLevel.Affects.Add(this.alienNextXP.StatNumber);
            this.xp.Affects.Add(this.level.StatNumber);
            this.sk.Affects.Add(this.level.StatNumber);
            this.alienXP.Affects.Add(this.alienLevel.StatNumber);
            this.profession.Affects.Add(this.health.StatNumber);
            this.profession.Affects.Add(this.maxNanoEnergy.StatNumber);
            this.profession.Affects.Add(this.ip.StatNumber);
            this.stamina.Affects.Add(this.healDelta.StatNumber);
            this.psychic.Affects.Add(this.nanoDelta.StatNumber);
            this.stamina.Affects.Add(this.healInterval.StatNumber);
            this.psychic.Affects.Add(this.nanoInterval.StatNumber);
            this.level.Affects.Add(this.ip.StatNumber);

            foreach (ClassStat c in this.all)
            {
                c.SetParent(parent);
            }

            if (!(parent is NonPlayerCharacter))
            {
                /*
                Flags.RaiseBeforeStatChangedEvent += Send;
                Life.RaiseBeforeStatChangedEvent += Send;
                VolumeMass.RaiseBeforeStatChangedEvent += Send;
                AttackSpeed.RaiseBeforeStatChangedEvent += Send;
                Breed.RaiseBeforeStatChangedEvent += Send;
                Clan.RaiseBeforeStatChangedEvent += Send;
                Team.RaiseBeforeStatChangedEvent += Send;
                State.RaiseBeforeStatChangedEvent += Send;
                TimeExist.RaiseBeforeStatChangedEvent += Send;
                MapFlags.RaiseBeforeStatChangedEvent += Send;
                ProfessionLevel.RaiseBeforeStatChangedEvent += Send;
                PreviousHealth.RaiseBeforeStatChangedEvent += Send;
                Mesh.RaiseBeforeStatChangedEvent += Send;
                Anim.RaiseBeforeStatChangedEvent += Send;
                Name.RaiseBeforeStatChangedEvent += Send;
                Info.RaiseBeforeStatChangedEvent += Send;
                Strength.RaiseBeforeStatChangedEvent += Send;
                Agility.RaiseBeforeStatChangedEvent += Send;
                Stamina.RaiseBeforeStatChangedEvent += Send;
                Intelligence.RaiseBeforeStatChangedEvent += Send;
                Sense.RaiseBeforeStatChangedEvent += Send;
                Psychic.RaiseBeforeStatChangedEvent += Send;
                AMS.RaiseBeforeStatChangedEvent += Send;
                StaticInstance.RaiseBeforeStatChangedEvent += Send;
                MaxMass.RaiseBeforeStatChangedEvent += Send;
                StaticType.RaiseBeforeStatChangedEvent += Send;
                Energy.RaiseBeforeStatChangedEvent += Send;
                Health.RaiseBeforeStatChangedEvent += Send;
                Height.RaiseBeforeStatChangedEvent += Send;
                DMS.RaiseBeforeStatChangedEvent += Send;
                Can.RaiseBeforeStatChangedEvent += Send;
                Face.RaiseBeforeStatChangedEvent += Send;
                HairMesh.RaiseBeforeStatChangedEvent += Send;
                Side.RaiseBeforeStatChangedEvent += Send;
                DeadTimer.RaiseBeforeStatChangedEvent += Send;
                AccessCount.RaiseBeforeStatChangedEvent += Send;
                AttackCount.RaiseBeforeStatChangedEvent += Send;
                TitleLevel.RaiseBeforeStatChangedEvent += Send;
                BackMesh.RaiseBeforeStatChangedEvent += Send;
                AlienXP.RaiseBeforeStatChangedEvent += Send;
                FabricType.RaiseBeforeStatChangedEvent += Send;
                CATMesh.RaiseBeforeStatChangedEvent += Send;
                ParentType.RaiseBeforeStatChangedEvent += Send;
                ParentInstance.RaiseBeforeStatChangedEvent += Send;
                BeltSlots.RaiseBeforeStatChangedEvent += Send;
                BandolierSlots.RaiseBeforeStatChangedEvent += Send;
                Fatness.RaiseBeforeStatChangedEvent += Send;
                ClanLevel.RaiseBeforeStatChangedEvent += Send;
                InsuranceTime.RaiseBeforeStatChangedEvent += Send;
                InventoryTimeout.RaiseBeforeStatChangedEvent += Send;
                AggDef.RaiseBeforeStatChangedEvent += Send;
                XP.RaiseBeforeStatChangedEvent += Send;
                IP.RaiseBeforeStatChangedEvent += Send;
                Level.RaiseBeforeStatChangedEvent += Send;
                InventoryId.RaiseBeforeStatChangedEvent += Send;
                TimeSinceCreation.RaiseBeforeStatChangedEvent += Send;
                LastXP.RaiseBeforeStatChangedEvent += Send;
                Age.RaiseBeforeStatChangedEvent += Send;
                Sex.RaiseBeforeStatChangedEvent += Send;
                Profession.RaiseBeforeStatChangedEvent += Send;
                Cash.RaiseBeforeStatChangedEvent += Send;
                Alignment.RaiseBeforeStatChangedEvent += Send;
                Attitude.RaiseBeforeStatChangedEvent += Send;
                HeadMesh.RaiseBeforeStatChangedEvent += Send;
                MissionBits5.RaiseBeforeStatChangedEvent += Send;
                MissionBits6.RaiseBeforeStatChangedEvent += Send;
                MissionBits7.RaiseBeforeStatChangedEvent += Send;
                VeteranPoints.RaiseBeforeStatChangedEvent += Send;
                MonthsPaid.RaiseBeforeStatChangedEvent += Send;
                SpeedPenalty.RaiseBeforeStatChangedEvent += Send;
                TotalMass.RaiseBeforeStatChangedEvent += Send;
                ItemType.RaiseBeforeStatChangedEvent += Send;
                RepairDifficulty.RaiseBeforeStatChangedEvent += Send;
                Price.RaiseBeforeStatChangedEvent += Send;
                MetaType.RaiseBeforeStatChangedEvent += Send;
                ItemClass.RaiseBeforeStatChangedEvent += Send;
                RepairSkill.RaiseBeforeStatChangedEvent += Send;
                CurrentMass.RaiseBeforeStatChangedEvent += Send;
                Icon.RaiseBeforeStatChangedEvent += Send;
                PrimaryItemType.RaiseBeforeStatChangedEvent += Send;
                PrimaryItemInstance.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemType.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemInstance.RaiseBeforeStatChangedEvent += Send;
                UserType.RaiseBeforeStatChangedEvent += Send;
                UserInstance.RaiseBeforeStatChangedEvent += Send;
                AreaType.RaiseBeforeStatChangedEvent += Send;
                AreaInstance.RaiseBeforeStatChangedEvent += Send;
                DefaultPos.RaiseBeforeStatChangedEvent += Send;
                Race.RaiseBeforeStatChangedEvent += Send;
                ProjectileAC.RaiseBeforeStatChangedEvent += Send;
                MeleeAC.RaiseBeforeStatChangedEvent += Send;
                EnergyAC.RaiseBeforeStatChangedEvent += Send;
                ChemicalAC.RaiseBeforeStatChangedEvent += Send;
                RadiationAC.RaiseBeforeStatChangedEvent += Send;
                ColdAC.RaiseBeforeStatChangedEvent += Send;
                PoisonAC.RaiseBeforeStatChangedEvent += Send;
                FireAC.RaiseBeforeStatChangedEvent += Send;
                StateAction.RaiseBeforeStatChangedEvent += Send;
                ItemAnim.RaiseBeforeStatChangedEvent += Send;
                MartialArts.RaiseBeforeStatChangedEvent += Send;
                MeleeMultiple.RaiseBeforeStatChangedEvent += Send;
                OnehBluntWeapons.RaiseBeforeStatChangedEvent += Send;
                OnehEdgedWeapon.RaiseBeforeStatChangedEvent += Send;
                MeleeEnergyWeapon.RaiseBeforeStatChangedEvent += Send;
                TwohEdgedWeapons.RaiseBeforeStatChangedEvent += Send;
                Piercing.RaiseBeforeStatChangedEvent += Send;
                TwohBluntWeapons.RaiseBeforeStatChangedEvent += Send;
                ThrowingKnife.RaiseBeforeStatChangedEvent += Send;
                Grenade.RaiseBeforeStatChangedEvent += Send;
                ThrownGrapplingWeapons.RaiseBeforeStatChangedEvent += Send;
                Bow.RaiseBeforeStatChangedEvent += Send;
                Pistol.RaiseBeforeStatChangedEvent += Send;
                Rifle.RaiseBeforeStatChangedEvent += Send;
                SubMachineGun.RaiseBeforeStatChangedEvent += Send;
                Shotgun.RaiseBeforeStatChangedEvent += Send;
                AssaultRifle.RaiseBeforeStatChangedEvent += Send;
                DriveWater.RaiseBeforeStatChangedEvent += Send;
                CloseCombatInitiative.RaiseBeforeStatChangedEvent += Send;
                DistanceWeaponInitiative.RaiseBeforeStatChangedEvent += Send;
                PhysicalProwessInitiative.RaiseBeforeStatChangedEvent += Send;
                BowSpecialAttack.RaiseBeforeStatChangedEvent += Send;
                SenseImprovement.RaiseBeforeStatChangedEvent += Send;
                FirstAid.RaiseBeforeStatChangedEvent += Send;
                Treatment.RaiseBeforeStatChangedEvent += Send;
                MechanicalEngineering.RaiseBeforeStatChangedEvent += Send;
                ElectricalEngineering.RaiseBeforeStatChangedEvent += Send;
                MaterialMetamorphose.RaiseBeforeStatChangedEvent += Send;
                BiologicalMetamorphose.RaiseBeforeStatChangedEvent += Send;
                PsychologicalModification.RaiseBeforeStatChangedEvent += Send;
                MaterialCreation.RaiseBeforeStatChangedEvent += Send;
                MaterialLocation.RaiseBeforeStatChangedEvent += Send;
                NanoEnergyPool.RaiseBeforeStatChangedEvent += Send;
                LR_EnergyWeapon.RaiseBeforeStatChangedEvent += Send;
                LR_MultipleWeapon.RaiseBeforeStatChangedEvent += Send;
                DisarmTrap.RaiseBeforeStatChangedEvent += Send;
                Perception.RaiseBeforeStatChangedEvent += Send;
                Adventuring.RaiseBeforeStatChangedEvent += Send;
                Swim.RaiseBeforeStatChangedEvent += Send;
                DriveAir.RaiseBeforeStatChangedEvent += Send;
                MapNavigation.RaiseBeforeStatChangedEvent += Send;
                Tutoring.RaiseBeforeStatChangedEvent += Send;
                Brawl.RaiseBeforeStatChangedEvent += Send;
                Riposte.RaiseBeforeStatChangedEvent += Send;
                Dimach.RaiseBeforeStatChangedEvent += Send;
                Parry.RaiseBeforeStatChangedEvent += Send;
                SneakAttack.RaiseBeforeStatChangedEvent += Send;
                FastAttack.RaiseBeforeStatChangedEvent += Send;
                Burst.RaiseBeforeStatChangedEvent += Send;
                NanoProwessInitiative.RaiseBeforeStatChangedEvent += Send;
                FlingShot.RaiseBeforeStatChangedEvent += Send;
                AimedShot.RaiseBeforeStatChangedEvent += Send;
                BodyDevelopment.RaiseBeforeStatChangedEvent += Send;
                Duck.RaiseBeforeStatChangedEvent += Send;
                Dodge.RaiseBeforeStatChangedEvent += Send;
                Evade.RaiseBeforeStatChangedEvent += Send;
                RunSpeed.RaiseBeforeStatChangedEvent += Send;
                FieldQuantumPhysics.RaiseBeforeStatChangedEvent += Send;
                WeaponSmithing.RaiseBeforeStatChangedEvent += Send;
                Pharmaceuticals.RaiseBeforeStatChangedEvent += Send;
                NanoProgramming.RaiseBeforeStatChangedEvent += Send;
                ComputerLiteracy.RaiseBeforeStatChangedEvent += Send;
                Psychology.RaiseBeforeStatChangedEvent += Send;
                Chemistry.RaiseBeforeStatChangedEvent += Send;
                Concealment.RaiseBeforeStatChangedEvent += Send;
                BreakingEntry.RaiseBeforeStatChangedEvent += Send;
                DriveGround.RaiseBeforeStatChangedEvent += Send;
                FullAuto.RaiseBeforeStatChangedEvent += Send;
                NanoAC.RaiseBeforeStatChangedEvent += Send;
                AlienLevel.RaiseBeforeStatChangedEvent += Send;
                HealthChangeBest.RaiseBeforeStatChangedEvent += Send;
                HealthChangeWorst.RaiseBeforeStatChangedEvent += Send;
                HealthChange.RaiseBeforeStatChangedEvent += Send;
                CurrentMovementMode.RaiseBeforeStatChangedEvent += Send;
                PrevMovementMode.RaiseBeforeStatChangedEvent += Send;
                AutoLockTimeDefault.RaiseBeforeStatChangedEvent += Send;
                AutoUnlockTimeDefault.RaiseBeforeStatChangedEvent += Send;
                MoreFlags.RaiseBeforeStatChangedEvent += Send;
                AlienNextXP.RaiseBeforeStatChangedEvent += Send;
                NPCFlags.RaiseBeforeStatChangedEvent += Send;
                CurrentNCU.RaiseBeforeStatChangedEvent += Send;
                MaxNCU.RaiseBeforeStatChangedEvent += Send;
                Specialization.RaiseBeforeStatChangedEvent += Send;
                EffectIcon.RaiseBeforeStatChangedEvent += Send;
                BuildingType.RaiseBeforeStatChangedEvent += Send;
                BuildingInstance.RaiseBeforeStatChangedEvent += Send;
                CardOwnerType.RaiseBeforeStatChangedEvent += Send;
                CardOwnerInstance.RaiseBeforeStatChangedEvent += Send;
                BuildingComplexInst.RaiseBeforeStatChangedEvent += Send;
                ExitInstance.RaiseBeforeStatChangedEvent += Send;
                NextDoorInBuilding.RaiseBeforeStatChangedEvent += Send;
                LastConcretePlayfieldInstance.RaiseBeforeStatChangedEvent += Send;
                ExtenalPlayfieldInstance.RaiseBeforeStatChangedEvent += Send;
                ExtenalDoorInstance.RaiseBeforeStatChangedEvent += Send;
                InPlay.RaiseBeforeStatChangedEvent += Send;
                AccessKey.RaiseBeforeStatChangedEvent += Send;
                PetMaster.RaiseBeforeStatChangedEvent += Send;
                OrientationMode.RaiseBeforeStatChangedEvent += Send;
                SessionTime.RaiseBeforeStatChangedEvent += Send;
                RP.RaiseBeforeStatChangedEvent += Send;
                Conformity.RaiseBeforeStatChangedEvent += Send;
                Aggressiveness.RaiseBeforeStatChangedEvent += Send;
                Stability.RaiseBeforeStatChangedEvent += Send;
                Extroverty.RaiseBeforeStatChangedEvent += Send;
                BreedHostility.RaiseBeforeStatChangedEvent += Send;
                ReflectProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ReflectMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ReflectEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ReflectChemicalAC.RaiseBeforeStatChangedEvent += Send;
                RechargeDelay.RaiseBeforeStatChangedEvent += Send;
                EquipDelay.RaiseBeforeStatChangedEvent += Send;
                MaxEnergy.RaiseBeforeStatChangedEvent += Send;
                TeamSide.RaiseBeforeStatChangedEvent += Send;
                CurrentNano.RaiseBeforeStatChangedEvent += Send;
                GmLevel.RaiseBeforeStatChangedEvent += Send;
                ReflectRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ReflectColdAC.RaiseBeforeStatChangedEvent += Send;
                ReflectNanoAC.RaiseBeforeStatChangedEvent += Send;
                ReflectFireAC.RaiseBeforeStatChangedEvent += Send;
                CurrBodyLocation.RaiseBeforeStatChangedEvent += Send;
                MaxNanoEnergy.RaiseBeforeStatChangedEvent += Send;
                AccumulatedDamage.RaiseBeforeStatChangedEvent += Send;
                CanChangeClothes.RaiseBeforeStatChangedEvent += Send;
                Features.RaiseBeforeStatChangedEvent += Send;
                ReflectPoisonAC.RaiseBeforeStatChangedEvent += Send;
                ShieldProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ShieldMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ShieldEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ShieldChemicalAC.RaiseBeforeStatChangedEvent += Send;
                ShieldRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ShieldColdAC.RaiseBeforeStatChangedEvent += Send;
                ShieldNanoAC.RaiseBeforeStatChangedEvent += Send;
                ShieldFireAC.RaiseBeforeStatChangedEvent += Send;
                ShieldPoisonAC.RaiseBeforeStatChangedEvent += Send;
                BerserkMode.RaiseBeforeStatChangedEvent += Send;
                InsurancePercentage.RaiseBeforeStatChangedEvent += Send;
                ChangeSideCount.RaiseBeforeStatChangedEvent += Send;
                AbsorbProjectileAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbMeleeAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbEnergyAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbChemicalAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbRadiationAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbColdAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbFireAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbPoisonAC.RaiseBeforeStatChangedEvent += Send;
                AbsorbNanoAC.RaiseBeforeStatChangedEvent += Send;
                TemporarySkillReduction.RaiseBeforeStatChangedEvent += Send;
                BirthDate.RaiseBeforeStatChangedEvent += Send;
                LastSaved.RaiseBeforeStatChangedEvent += Send;
                SoundVolume.RaiseBeforeStatChangedEvent += Send;
                PetCounter.RaiseBeforeStatChangedEvent += Send;
                MeetersWalked.RaiseBeforeStatChangedEvent += Send;
                QuestLevelsSolved.RaiseBeforeStatChangedEvent += Send;
                MonsterLevelsKilled.RaiseBeforeStatChangedEvent += Send;
                PvPLevelsKilled.RaiseBeforeStatChangedEvent += Send;
                MissionBits1.RaiseBeforeStatChangedEvent += Send;
                MissionBits2.RaiseBeforeStatChangedEvent += Send;
                AccessGrant.RaiseBeforeStatChangedEvent += Send;
                DoorFlags.RaiseBeforeStatChangedEvent += Send;
                ClanHierarchy.RaiseBeforeStatChangedEvent += Send;
                QuestStat.RaiseBeforeStatChangedEvent += Send;
                ClientActivated.RaiseBeforeStatChangedEvent += Send;
                PersonalResearchLevel.RaiseBeforeStatChangedEvent += Send;
                GlobalResearchLevel.RaiseBeforeStatChangedEvent += Send;
                PersonalResearchGoal.RaiseBeforeStatChangedEvent += Send;
                GlobalResearchGoal.RaiseBeforeStatChangedEvent += Send;
                TurnSpeed.RaiseBeforeStatChangedEvent += Send;
                LiquidType.RaiseBeforeStatChangedEvent += Send;
                GatherSound.RaiseBeforeStatChangedEvent += Send;
                CastSound.RaiseBeforeStatChangedEvent += Send;
                TravelSound.RaiseBeforeStatChangedEvent += Send;
                HitSound.RaiseBeforeStatChangedEvent += Send;
                SecondaryItemTemplate.RaiseBeforeStatChangedEvent += Send;
                EquippedWeapons.RaiseBeforeStatChangedEvent += Send;
                XPKillRange.RaiseBeforeStatChangedEvent += Send;
                AMSModifier.RaiseBeforeStatChangedEvent += Send;
                DMSModifier.RaiseBeforeStatChangedEvent += Send;
                ProjectileDamageModifier.RaiseBeforeStatChangedEvent += Send;
                MeleeDamageModifier.RaiseBeforeStatChangedEvent += Send;
                EnergyDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ChemicalDamageModifier.RaiseBeforeStatChangedEvent += Send;
                RadiationDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ItemHateValue.RaiseBeforeStatChangedEvent += Send;
                DamageBonus.RaiseBeforeStatChangedEvent += Send;
                MaxDamage.RaiseBeforeStatChangedEvent += Send;
                MinDamage.RaiseBeforeStatChangedEvent += Send;
                AttackRange.RaiseBeforeStatChangedEvent += Send;
                HateValueModifyer.RaiseBeforeStatChangedEvent += Send;
                TrapDifficulty.RaiseBeforeStatChangedEvent += Send;
                StatOne.RaiseBeforeStatChangedEvent += Send;
                NumAttackEffects.RaiseBeforeStatChangedEvent += Send;
                DefaultAttackType.RaiseBeforeStatChangedEvent += Send;
                ItemSkill.RaiseBeforeStatChangedEvent += Send;
                ItemDelay.RaiseBeforeStatChangedEvent += Send;
                ItemOpposedSkill.RaiseBeforeStatChangedEvent += Send;
                ItemSIS.RaiseBeforeStatChangedEvent += Send;
                InteractionRadius.RaiseBeforeStatChangedEvent += Send;
                Placement.RaiseBeforeStatChangedEvent += Send;
                LockDifficulty.RaiseBeforeStatChangedEvent += Send;
                Members.RaiseBeforeStatChangedEvent += Send;
                MinMembers.RaiseBeforeStatChangedEvent += Send;
                ClanPrice.RaiseBeforeStatChangedEvent += Send;
                MissionBits3.RaiseBeforeStatChangedEvent += Send;
                ClanType.RaiseBeforeStatChangedEvent += Send;
                ClanInstance.RaiseBeforeStatChangedEvent += Send;
                VoteCount.RaiseBeforeStatChangedEvent += Send;
                MemberType.RaiseBeforeStatChangedEvent += Send;
                MemberInstance.RaiseBeforeStatChangedEvent += Send;
                GlobalClanType.RaiseBeforeStatChangedEvent += Send;
                GlobalClanInstance.RaiseBeforeStatChangedEvent += Send;
                ColdDamageModifier.RaiseBeforeStatChangedEvent += Send;
                ClanUpkeepInterval.RaiseBeforeStatChangedEvent += Send;
                TimeSinceUpkeep.RaiseBeforeStatChangedEvent += Send;
                ClanFinalized.RaiseBeforeStatChangedEvent += Send;
                NanoDamageModifier.RaiseBeforeStatChangedEvent += Send;
                FireDamageModifier.RaiseBeforeStatChangedEvent += Send;
                PoisonDamageModifier.RaiseBeforeStatChangedEvent += Send;
                NPCostModifier.RaiseBeforeStatChangedEvent += Send;
                XPModifier.RaiseBeforeStatChangedEvent += Send;
                BreedLimit.RaiseBeforeStatChangedEvent += Send;
                GenderLimit.RaiseBeforeStatChangedEvent += Send;
                LevelLimit.RaiseBeforeStatChangedEvent += Send;
                PlayerKilling.RaiseBeforeStatChangedEvent += Send;
                TeamAllowed.RaiseBeforeStatChangedEvent += Send;
                WeaponDisallowedType.RaiseBeforeStatChangedEvent += Send;
                WeaponDisallowedInstance.RaiseBeforeStatChangedEvent += Send;
                Taboo.RaiseBeforeStatChangedEvent += Send;
                Compulsion.RaiseBeforeStatChangedEvent += Send;
                SkillDisabled.RaiseBeforeStatChangedEvent += Send;
                ClanItemType.RaiseBeforeStatChangedEvent += Send;
                ClanItemInstance.RaiseBeforeStatChangedEvent += Send;
                DebuffFormula.RaiseBeforeStatChangedEvent += Send;
                PvP_Rating.RaiseBeforeStatChangedEvent += Send;
                SavedXP.RaiseBeforeStatChangedEvent += Send;
                DoorBlockTime.RaiseBeforeStatChangedEvent += Send;
                OverrideTexture.RaiseBeforeStatChangedEvent += Send;
                OverrideMaterial.RaiseBeforeStatChangedEvent += Send;
                DeathReason.RaiseBeforeStatChangedEvent += Send;
                DamageOverrideType.RaiseBeforeStatChangedEvent += Send;
                BrainType.RaiseBeforeStatChangedEvent += Send;
                XPBonus.RaiseBeforeStatChangedEvent += Send;
                HealInterval.RaiseBeforeStatChangedEvent += Send;
                HealDelta.RaiseBeforeStatChangedEvent += Send;
                MonsterTexture.RaiseBeforeStatChangedEvent += Send;
                HasAlwaysLootable.RaiseBeforeStatChangedEvent += Send;
                TradeLimit.RaiseBeforeStatChangedEvent += Send;
                FaceTexture.RaiseBeforeStatChangedEvent += Send;
                SpecialCondition.RaiseBeforeStatChangedEvent += Send;
                AutoAttackFlags.RaiseBeforeStatChangedEvent += Send;
                NextXP.RaiseBeforeStatChangedEvent += Send;
                TeleportPauseMilliSeconds.RaiseBeforeStatChangedEvent += Send;
                SISCap.RaiseBeforeStatChangedEvent += Send;
                AnimSet.RaiseBeforeStatChangedEvent += Send;
                AttackType.RaiseBeforeStatChangedEvent += Send;
                NanoFocusLevel.RaiseBeforeStatChangedEvent += Send;
                NPCHash.RaiseBeforeStatChangedEvent += Send;
                CollisionRadius.RaiseBeforeStatChangedEvent += Send;
                OuterRadius.RaiseBeforeStatChangedEvent += Send;
                MonsterData.RaiseBeforeStatChangedEvent += Send;
                MonsterScale.RaiseBeforeStatChangedEvent += Send;
                HitEffectType.RaiseBeforeStatChangedEvent += Send;
                ResurrectDest.RaiseBeforeStatChangedEvent += Send;
                NanoInterval.RaiseBeforeStatChangedEvent += Send;
                NanoDelta.RaiseBeforeStatChangedEvent += Send;
                ReclaimItem.RaiseBeforeStatChangedEvent += Send;
                GatherEffectType.RaiseBeforeStatChangedEvent += Send;
                VisualBreed.RaiseBeforeStatChangedEvent += Send;
                VisualProfession.RaiseBeforeStatChangedEvent += Send;
                VisualSex.RaiseBeforeStatChangedEvent += Send;
                RitualTargetInst.RaiseBeforeStatChangedEvent += Send;
                SkillTimeOnSelectedTarget.RaiseBeforeStatChangedEvent += Send;
                LastSaveXP.RaiseBeforeStatChangedEvent += Send;
                ExtendedTime.RaiseBeforeStatChangedEvent += Send;
                BurstRecharge.RaiseBeforeStatChangedEvent += Send;
                FullAutoRecharge.RaiseBeforeStatChangedEvent += Send;
                GatherAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CastTargetAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CastSelfAbstractAnim.RaiseBeforeStatChangedEvent += Send;
                CriticalIncrease.RaiseBeforeStatChangedEvent += Send;
                RangeIncreaserWeapon.RaiseBeforeStatChangedEvent += Send;
                RangeIncreaserNF.RaiseBeforeStatChangedEvent += Send;
                SkillLockModifier.RaiseBeforeStatChangedEvent += Send;
                InterruptModifier.RaiseBeforeStatChangedEvent += Send;
                ACGEntranceStyles.RaiseBeforeStatChangedEvent += Send;
                ChanceOfBreakOnSpellAttack.RaiseBeforeStatChangedEvent += Send;
                ChanceOfBreakOnDebuff.RaiseBeforeStatChangedEvent += Send;
                DieAnim.RaiseBeforeStatChangedEvent += Send;
                TowerType.RaiseBeforeStatChangedEvent += Send;
                Expansion.RaiseBeforeStatChangedEvent += Send;
                LowresMesh.RaiseBeforeStatChangedEvent += Send;
                CriticalDecrease.RaiseBeforeStatChangedEvent += Send;
                OldTimeExist.RaiseBeforeStatChangedEvent += Send;
                ResistModifier.RaiseBeforeStatChangedEvent += Send;
                ChestFlags.RaiseBeforeStatChangedEvent += Send;
                PrimaryTemplateID.RaiseBeforeStatChangedEvent += Send;
                NumberOfItems.RaiseBeforeStatChangedEvent += Send;
                SelectedTargetType.RaiseBeforeStatChangedEvent += Send;
                Corpse_Hash.RaiseBeforeStatChangedEvent += Send;
                AmmoName.RaiseBeforeStatChangedEvent += Send;
                Rotation.RaiseBeforeStatChangedEvent += Send;
                CATAnim.RaiseBeforeStatChangedEvent += Send;
                CATAnimFlags.RaiseBeforeStatChangedEvent += Send;
                DisplayCATAnim.RaiseBeforeStatChangedEvent += Send;
                DisplayCATMesh.RaiseBeforeStatChangedEvent += Send;
                School.RaiseBeforeStatChangedEvent += Send;
                NanoSpeed.RaiseBeforeStatChangedEvent += Send;
                NanoPoints.RaiseBeforeStatChangedEvent += Send;
                TrainSkill.RaiseBeforeStatChangedEvent += Send;
                TrainSkillCost.RaiseBeforeStatChangedEvent += Send;
                IsFightingMe.RaiseBeforeStatChangedEvent += Send;
                NextFormula.RaiseBeforeStatChangedEvent += Send;
                MultipleCount.RaiseBeforeStatChangedEvent += Send;
                EffectType.RaiseBeforeStatChangedEvent += Send;
                ImpactEffectType.RaiseBeforeStatChangedEvent += Send;
                CorpseType.RaiseBeforeStatChangedEvent += Send;
                CorpseInstance.RaiseBeforeStatChangedEvent += Send;
                CorpseAnimKey.RaiseBeforeStatChangedEvent += Send;
                UnarmedTemplateInstance.RaiseBeforeStatChangedEvent += Send;
                TracerEffectType.RaiseBeforeStatChangedEvent += Send;
                AmmoType.RaiseBeforeStatChangedEvent += Send;
                CharRadius.RaiseBeforeStatChangedEvent += Send;
                ChanceOfUse.RaiseBeforeStatChangedEvent += Send;
                CurrentState.RaiseBeforeStatChangedEvent += Send;
                ArmourType.RaiseBeforeStatChangedEvent += Send;
                RestModifier.RaiseBeforeStatChangedEvent += Send;
                BuyModifier.RaiseBeforeStatChangedEvent += Send;
                SellModifier.RaiseBeforeStatChangedEvent += Send;
                CastEffectType.RaiseBeforeStatChangedEvent += Send;
                NPCBrainState.RaiseBeforeStatChangedEvent += Send;
                WaitState.RaiseBeforeStatChangedEvent += Send;
                SelectedTarget.RaiseBeforeStatChangedEvent += Send;
                MissionBits4.RaiseBeforeStatChangedEvent += Send;
                OwnerInstance.RaiseBeforeStatChangedEvent += Send;
                CharState.RaiseBeforeStatChangedEvent += Send;
                ReadOnly.RaiseBeforeStatChangedEvent += Send;
                DamageType.RaiseBeforeStatChangedEvent += Send;
                CollideCheckInterval.RaiseBeforeStatChangedEvent += Send;
                PlayfieldType.RaiseBeforeStatChangedEvent += Send;
                NPCCommand.RaiseBeforeStatChangedEvent += Send;
                InitiativeType.RaiseBeforeStatChangedEvent += Send;
                CharTmp1.RaiseBeforeStatChangedEvent += Send;
                CharTmp2.RaiseBeforeStatChangedEvent += Send;
                CharTmp3.RaiseBeforeStatChangedEvent += Send;
                CharTmp4.RaiseBeforeStatChangedEvent += Send;
                NPCCommandArg.RaiseBeforeStatChangedEvent += Send;
                NameTemplate.RaiseBeforeStatChangedEvent += Send;
                DesiredTargetDistance.RaiseBeforeStatChangedEvent += Send;
                VicinityRange.RaiseBeforeStatChangedEvent += Send;
                NPCIsSurrendering.RaiseBeforeStatChangedEvent += Send;
                StateMachine.RaiseBeforeStatChangedEvent += Send;
                NPCSurrenderInstance.RaiseBeforeStatChangedEvent += Send;
                NPCHasPatrolList.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityChars.RaiseBeforeStatChangedEvent += Send;
                ProximityRangeOutdoors.RaiseBeforeStatChangedEvent += Send;
                NPCFamily.RaiseBeforeStatChangedEvent += Send;
                CommandRange.RaiseBeforeStatChangedEvent += Send;
                NPCHatelistSize.RaiseBeforeStatChangedEvent += Send;
                NPCNumPets.RaiseBeforeStatChangedEvent += Send;
                ODMinSizeAdd.RaiseBeforeStatChangedEvent += Send;
                EffectRed.RaiseBeforeStatChangedEvent += Send;
                EffectGreen.RaiseBeforeStatChangedEvent += Send;
                EffectBlue.RaiseBeforeStatChangedEvent += Send;
                ODMaxSizeAdd.RaiseBeforeStatChangedEvent += Send;
                DurationModifier.RaiseBeforeStatChangedEvent += Send;
                NPCCryForHelpRange.RaiseBeforeStatChangedEvent += Send;
                LOSHeight.RaiseBeforeStatChangedEvent += Send;
                PetReq1.RaiseBeforeStatChangedEvent += Send;
                PetReq2.RaiseBeforeStatChangedEvent += Send;
                PetReq3.RaiseBeforeStatChangedEvent += Send;
                MapOptions.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart1.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart2.RaiseBeforeStatChangedEvent += Send;
                FixtureFlags.RaiseBeforeStatChangedEvent += Send;
                FallDamage.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedProjectileAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedMeleeAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedEnergyAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedChemicalAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedRadiationAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedColdAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedNanoAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedFireAC.RaiseBeforeStatChangedEvent += Send;
                ReflectReturnedPoisonAC.RaiseBeforeStatChangedEvent += Send;
                ProximityRangeIndoors.RaiseBeforeStatChangedEvent += Send;
                PetReqVal1.RaiseBeforeStatChangedEvent += Send;
                PetReqVal2.RaiseBeforeStatChangedEvent += Send;
                PetReqVal3.RaiseBeforeStatChangedEvent += Send;
                TargetFacing.RaiseBeforeStatChangedEvent += Send;
                Backstab.RaiseBeforeStatChangedEvent += Send;
                OriginatorType.RaiseBeforeStatChangedEvent += Send;
                QuestInstance.RaiseBeforeStatChangedEvent += Send;
                QuestIndex1.RaiseBeforeStatChangedEvent += Send;
                QuestIndex2.RaiseBeforeStatChangedEvent += Send;
                QuestIndex3.RaiseBeforeStatChangedEvent += Send;
                QuestIndex4.RaiseBeforeStatChangedEvent += Send;
                QuestIndex5.RaiseBeforeStatChangedEvent += Send;
                QTDungeonInstance.RaiseBeforeStatChangedEvent += Send;
                QTNumMonsters.RaiseBeforeStatChangedEvent += Send;
                QTKilledMonsters.RaiseBeforeStatChangedEvent += Send;
                AnimPos.RaiseBeforeStatChangedEvent += Send;
                AnimPlay.RaiseBeforeStatChangedEvent += Send;
                AnimSpeed.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID1.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount1.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID2.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount2.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterID3.RaiseBeforeStatChangedEvent += Send;
                QTKillNumMonsterCount3.RaiseBeforeStatChangedEvent += Send;
                QuestIndex0.RaiseBeforeStatChangedEvent += Send;
                QuestTimeout.RaiseBeforeStatChangedEvent += Send;
                Tower_NPCHash.RaiseBeforeStatChangedEvent += Send;
                PetType.RaiseBeforeStatChangedEvent += Send;
                OnTowerCreation.RaiseBeforeStatChangedEvent += Send;
                OwnedTowers.RaiseBeforeStatChangedEvent += Send;
                TowerInstance.RaiseBeforeStatChangedEvent += Send;
                AttackShield.RaiseBeforeStatChangedEvent += Send;
                SpecialAttackShield.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityPlayers.RaiseBeforeStatChangedEvent += Send;
                NPCUseFightModeRegenRate.RaiseBeforeStatChangedEvent += Send;
                Rnd.RaiseBeforeStatChangedEvent += Send;
                SocialStatus.RaiseBeforeStatChangedEvent += Send;
                LastRnd.RaiseBeforeStatChangedEvent += Send;
                ItemDelayCap.RaiseBeforeStatChangedEvent += Send;
                RechargeDelayCap.RaiseBeforeStatChangedEvent += Send;
                PercentRemainingHealth.RaiseBeforeStatChangedEvent += Send;
                PercentRemainingNano.RaiseBeforeStatChangedEvent += Send;
                TargetDistance.RaiseBeforeStatChangedEvent += Send;
                TeamCloseness.RaiseBeforeStatChangedEvent += Send;
                NumberOnHateList.RaiseBeforeStatChangedEvent += Send;
                ConditionState.RaiseBeforeStatChangedEvent += Send;
                ExpansionPlayfield.RaiseBeforeStatChangedEvent += Send;
                ShadowBreed.RaiseBeforeStatChangedEvent += Send;
                NPCFovStatus.RaiseBeforeStatChangedEvent += Send;
                DudChance.RaiseBeforeStatChangedEvent += Send;
                HealMultiplier.RaiseBeforeStatChangedEvent += Send;
                NanoDamageMultiplier.RaiseBeforeStatChangedEvent += Send;
                NanoVulnerability.RaiseBeforeStatChangedEvent += Send;
                AmsCap.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative1.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative2.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative3.RaiseBeforeStatChangedEvent += Send;
                ProcInitiative4.RaiseBeforeStatChangedEvent += Send;
                FactionModifier.RaiseBeforeStatChangedEvent += Send;
                MissionBits8.RaiseBeforeStatChangedEvent += Send;
                MissionBits9.RaiseBeforeStatChangedEvent += Send;
                StackingLine2.RaiseBeforeStatChangedEvent += Send;
                StackingLine3.RaiseBeforeStatChangedEvent += Send;
                StackingLine4.RaiseBeforeStatChangedEvent += Send;
                StackingLine5.RaiseBeforeStatChangedEvent += Send;
                StackingLine6.RaiseBeforeStatChangedEvent += Send;
                StackingOrder.RaiseBeforeStatChangedEvent += Send;
                ProcNano1.RaiseBeforeStatChangedEvent += Send;
                ProcNano2.RaiseBeforeStatChangedEvent += Send;
                ProcNano3.RaiseBeforeStatChangedEvent += Send;
                ProcNano4.RaiseBeforeStatChangedEvent += Send;
                ProcChance1.RaiseBeforeStatChangedEvent += Send;
                ProcChance2.RaiseBeforeStatChangedEvent += Send;
                ProcChance3.RaiseBeforeStatChangedEvent += Send;
                ProcChance4.RaiseBeforeStatChangedEvent += Send;
                OTArmedForces.RaiseBeforeStatChangedEvent += Send;
                ClanSentinels.RaiseBeforeStatChangedEvent += Send;
                OTMed.RaiseBeforeStatChangedEvent += Send;
                ClanGaia.RaiseBeforeStatChangedEvent += Send;
                OTTrans.RaiseBeforeStatChangedEvent += Send;
                ClanVanguards.RaiseBeforeStatChangedEvent += Send;
                GOS.RaiseBeforeStatChangedEvent += Send;
                OTFollowers.RaiseBeforeStatChangedEvent += Send;
                OTOperator.RaiseBeforeStatChangedEvent += Send;
                OTUnredeemed.RaiseBeforeStatChangedEvent += Send;
                ClanDevoted.RaiseBeforeStatChangedEvent += Send;
                ClanConserver.RaiseBeforeStatChangedEvent += Send;
                ClanRedeemed.RaiseBeforeStatChangedEvent += Send;
                SK.RaiseBeforeStatChangedEvent += Send;
                LastSK.RaiseBeforeStatChangedEvent += Send;
                NextSK.RaiseBeforeStatChangedEvent += Send;
                PlayerOptions.RaiseBeforeStatChangedEvent += Send;
                LastPerkResetTime.RaiseBeforeStatChangedEvent += Send;
                CurrentTime.RaiseBeforeStatChangedEvent += Send;
                ShadowBreedTemplate.RaiseBeforeStatChangedEvent += Send;
                NPCVicinityFamily.RaiseBeforeStatChangedEvent += Send;
                NPCScriptAMSScale.RaiseBeforeStatChangedEvent += Send;
                ApartmentsAllowed.RaiseBeforeStatChangedEvent += Send;
                ApartmentsOwned.RaiseBeforeStatChangedEvent += Send;
                ApartmentAccessCard.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart3.RaiseBeforeStatChangedEvent += Send;
                MapAreaPart4.RaiseBeforeStatChangedEvent += Send;
                NumberOfTeamMembers.RaiseBeforeStatChangedEvent += Send;
                ActionCategory.RaiseBeforeStatChangedEvent += Send;
                CurrentPlayfield.RaiseBeforeStatChangedEvent += Send;
                DistrictNano.RaiseBeforeStatChangedEvent += Send;
                DistrictNanoInterval.RaiseBeforeStatChangedEvent += Send;
                UnsavedXP.RaiseBeforeStatChangedEvent += Send;
                RegainXPPercentage.RaiseBeforeStatChangedEvent += Send;
                TempSaveTeamID.RaiseBeforeStatChangedEvent += Send;
                TempSavePlayfield.RaiseBeforeStatChangedEvent += Send;
                TempSaveX.RaiseBeforeStatChangedEvent += Send;
                TempSaveY.RaiseBeforeStatChangedEvent += Send;
                ExtendedFlags.RaiseBeforeStatChangedEvent += Send;
                ShopPrice.RaiseBeforeStatChangedEvent += Send;
                NewbieHP.RaiseBeforeStatChangedEvent += Send;
                HPLevelUp.RaiseBeforeStatChangedEvent += Send;
                HPPerSkill.RaiseBeforeStatChangedEvent += Send;
                NewbieNP.RaiseBeforeStatChangedEvent += Send;
                NPLevelUp.RaiseBeforeStatChangedEvent += Send;
                NPPerSkill.RaiseBeforeStatChangedEvent += Send;
                MaxShopItems.RaiseBeforeStatChangedEvent += Send;
                PlayerID.RaiseBeforeStatChangedEvent += Send;
                ShopRent.RaiseBeforeStatChangedEvent += Send;
                SynergyHash.RaiseBeforeStatChangedEvent += Send;
                ShopFlags.RaiseBeforeStatChangedEvent += Send;
                ShopLastUsed.RaiseBeforeStatChangedEvent += Send;
                ShopType.RaiseBeforeStatChangedEvent += Send;
                LockDownTime.RaiseBeforeStatChangedEvent += Send;
                LeaderLockDownTime.RaiseBeforeStatChangedEvent += Send;
                InvadersKilled.RaiseBeforeStatChangedEvent += Send;
                KilledByInvaders.RaiseBeforeStatChangedEvent += Send;
                MissionBits10.RaiseBeforeStatChangedEvent += Send;
                MissionBits11.RaiseBeforeStatChangedEvent += Send;
                MissionBits12.RaiseBeforeStatChangedEvent += Send;
                HouseTemplate.RaiseBeforeStatChangedEvent += Send;
                PercentFireDamage.RaiseBeforeStatChangedEvent += Send;
                PercentColdDamage.RaiseBeforeStatChangedEvent += Send;
                PercentMeleeDamage.RaiseBeforeStatChangedEvent += Send;
                PercentProjectileDamage.RaiseBeforeStatChangedEvent += Send;
                PercentPoisonDamage.RaiseBeforeStatChangedEvent += Send;
                PercentRadiationDamage.RaiseBeforeStatChangedEvent += Send;
                PercentEnergyDamage.RaiseBeforeStatChangedEvent += Send;
                PercentChemicalDamage.RaiseBeforeStatChangedEvent += Send;
                TotalDamage.RaiseBeforeStatChangedEvent += Send;
                TrackProjectileDamage.RaiseBeforeStatChangedEvent += Send;
                TrackMeleeDamage.RaiseBeforeStatChangedEvent += Send;
                TrackEnergyDamage.RaiseBeforeStatChangedEvent += Send;
                TrackChemicalDamage.RaiseBeforeStatChangedEvent += Send;
                TrackRadiationDamage.RaiseBeforeStatChangedEvent += Send;
                TrackColdDamage.RaiseBeforeStatChangedEvent += Send;
                TrackPoisonDamage.RaiseBeforeStatChangedEvent += Send;
                TrackFireDamage.RaiseBeforeStatChangedEvent += Send;
                NPCSpellArg1.RaiseBeforeStatChangedEvent += Send;
                NPCSpellRet1.RaiseBeforeStatChangedEvent += Send;
                CityInstance.RaiseBeforeStatChangedEvent += Send;
                DistanceToSpawnpoint.RaiseBeforeStatChangedEvent += Send;
                CityTerminalRechargePercent.RaiseBeforeStatChangedEvent += Send;
                UnreadMailCount.RaiseBeforeStatChangedEvent += Send;
                LastMailCheckTime.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash1.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash2.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash3.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash4.RaiseBeforeStatChangedEvent += Send;
                AdvantageHash5.RaiseBeforeStatChangedEvent += Send;
                ShopIndex.RaiseBeforeStatChangedEvent += Send;
                ShopID.RaiseBeforeStatChangedEvent += Send;
                IsVehicle.RaiseBeforeStatChangedEvent += Send;
                DamageToNano.RaiseBeforeStatChangedEvent += Send;
                AccountFlags.RaiseBeforeStatChangedEvent += Send;
                DamageToNanoMultiplier.RaiseBeforeStatChangedEvent += Send;
                MechData.RaiseBeforeStatChangedEvent += Send;
                VehicleAC.RaiseBeforeStatChangedEvent += Send;
                VehicleDamage.RaiseBeforeStatChangedEvent += Send;
                VehicleHealth.RaiseBeforeStatChangedEvent += Send;
                VehicleSpeed.RaiseBeforeStatChangedEvent += Send;
                BattlestationSide.RaiseBeforeStatChangedEvent += Send;
                VP.RaiseBeforeStatChangedEvent += Send;
                BattlestationRep.RaiseBeforeStatChangedEvent += Send;
                PetState.RaiseBeforeStatChangedEvent += Send;
                PaidPoints.RaiseBeforeStatChangedEvent += Send;
                VisualFlags.RaiseBeforeStatChangedEvent += Send;
                PVPDuelKills.RaiseBeforeStatChangedEvent += Send;
                PVPDuelDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPProfessionDuelKills.RaiseBeforeStatChangedEvent += Send;
                PVPProfessionDuelDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPRankedSoloKills.RaiseBeforeStatChangedEvent += Send;
                PVPRankedSoloDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPRankedTeamKills.RaiseBeforeStatChangedEvent += Send;
                PVPRankedTeamDeaths.RaiseBeforeStatChangedEvent += Send;
                PVPSoloScore.RaiseBeforeStatChangedEvent += Send;
                PVPTeamScore.RaiseBeforeStatChangedEvent += Send;
                PVPDuelScore.RaiseBeforeStatChangedEvent += Send;
                ACGItemSeed.RaiseBeforeStatChangedEvent += Send;
                ACGItemLevel.RaiseBeforeStatChangedEvent += Send;
                ACGItemTemplateID.RaiseBeforeStatChangedEvent += Send;
                ACGItemTemplateID2.RaiseBeforeStatChangedEvent += Send;
                ACGItemCategoryID.RaiseBeforeStatChangedEvent += Send;
                HasKnuBotData.RaiseBeforeStatChangedEvent += Send;
                QuestBoothDifficulty.RaiseBeforeStatChangedEvent += Send;
                QuestASMinimumRange.RaiseBeforeStatChangedEvent += Send;
                QuestASMaximumRange.RaiseBeforeStatChangedEvent += Send;
                VisualLODLevel.RaiseBeforeStatChangedEvent += Send;
                TargetDistanceChange.RaiseBeforeStatChangedEvent += Send;
                TideRequiredDynelID.RaiseBeforeStatChangedEvent += Send;
                StreamCheckMagic.RaiseBeforeStatChangedEvent += Send;
                Type.RaiseBeforeStatChangedEvent += Send;
                Instance.RaiseBeforeStatChangedEvent += Send;
                ShoulderMeshRight.RaiseBeforeStatChangedEvent += Send;
                ShoulderMeshLeft.RaiseBeforeStatChangedEvent += Send;
                WeaponMeshRight.RaiseBeforeStatChangedEvent += Send;
                WeaponMeshLeft.RaiseBeforeStatChangedEvent += Send;
                */
            }

            this.expansion.DoNotDontWriteToSql = true;
            this.accountFlags.DoNotDontWriteToSql = true;
            this.playerId.DoNotDontWriteToSql = true;
            this.professionLevel.DoNotDontWriteToSql = true;
            this.gmLevel.DoNotDontWriteToSql = true;
            this.objectType.DoNotDontWriteToSql = true;
            this.instance.DoNotDontWriteToSql = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public ClassStat AbsorbChemicalAC
        {
            get
            {
                return this.absorbChemicalAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbColdAC
        {
            get
            {
                return this.absorbColdAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbEnergyAC
        {
            get
            {
                return this.absorbEnergyAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbFireAC
        {
            get
            {
                return this.absorbFireAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbMeleeAC
        {
            get
            {
                return this.absorbMeleeAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbNanoAC
        {
            get
            {
                return this.absorbNanoAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbPoisonAC
        {
            get
            {
                return this.absorbPoisonAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbProjectileAC
        {
            get
            {
                return this.absorbProjectileAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AbsorbRadiationAC
        {
            get
            {
                return this.absorbRadiationAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AccessCount
        {
            get
            {
                return this.accessCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AccessGrant
        {
            get
            {
                return this.accessGrant;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AccessKey
        {
            get
            {
                return this.accessKey;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AccountFlags
        {
            get
            {
                return this.accountFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AccumulatedDamage
        {
            get
            {
                return this.accumulatedDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgEntranceStyles
        {
            get
            {
                return this.acgEntranceStyles;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgItemCategoryId
        {
            get
            {
                return this.acgItemCategoryId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgItemLevel
        {
            get
            {
                return this.acgItemLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgItemSeed
        {
            get
            {
                return this.acgItemSeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgItemTemplateId
        {
            get
            {
                return this.acgItemTemplateId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AcgItemTemplateId2
        {
            get
            {
                return this.acgItemTemplateId2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ActionCategory
        {
            get
            {
                return this.actionCategory;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AdvantageHash1
        {
            get
            {
                return this.advantageHash1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AdvantageHash2
        {
            get
            {
                return this.advantageHash2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AdvantageHash3
        {
            get
            {
                return this.advantageHash3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AdvantageHash4
        {
            get
            {
                return this.advantageHash4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AdvantageHash5
        {
            get
            {
                return this.advantageHash5;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Adventuring
        {
            get
            {
                return this.adventuring;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Age
        {
            get
            {
                return this.age;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AggDef
        {
            get
            {
                return this.aggDef;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Aggressiveness
        {
            get
            {
                return this.aggressiveness;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Agility
        {
            get
            {
                return this.agility;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill AimedShot
        {
            get
            {
                return this.aimedShot;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AlienLevel
        {
            get
            {
                return this.alienLevel;
            }
        }

        /// <summary>
        /// </summary>
        public StatAlienNextXP AlienNextXP
        {
            get
            {
                return this.alienNextXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AlienXP
        {
            get
            {
                return this.alienXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Alignment
        {
            get
            {
                return this.alignment;
            }
        }

        /// <summary>
        /// </summary>
        public List<ClassStat> All
        {
            get
            {
                return this.all;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AmmoName
        {
            get
            {
                return this.ammoName;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AmmoType
        {
            get
            {
                return this.ammoType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Ams
        {
            get
            {
                return this.ams;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AmsCap
        {
            get
            {
                return this.amsCap;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AmsModifier
        {
            get
            {
                return this.amsModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Anim
        {
            get
            {
                return this.anim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AnimPlay
        {
            get
            {
                return this.animPlay;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AnimPos
        {
            get
            {
                return this.animPos;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AnimSet
        {
            get
            {
                return this.animSet;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AnimSpeed
        {
            get
            {
                return this.animSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ApartmentAccessCard
        {
            get
            {
                return this.apartmentAccessCard;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ApartmentsAllowed
        {
            get
            {
                return this.apartmentsAllowed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ApartmentsOwned
        {
            get
            {
                return this.apartmentsOwned;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AreaInstance
        {
            get
            {
                return this.areaInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AreaType
        {
            get
            {
                return this.areaType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ArmourType
        {
            get
            {
                return this.armourType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill AssaultRifle
        {
            get
            {
                return this.assaultRifle;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AttackCount
        {
            get
            {
                return this.attackCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AttackRange
        {
            get
            {
                return this.attackRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AttackShield
        {
            get
            {
                return this.attackShield;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AttackSpeed
        {
            get
            {
                return this.attackSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AttackType
        {
            get
            {
                return this.attackType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Attitude
        {
            get
            {
                return this.attitude;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AutoAttackFlags
        {
            get
            {
                return this.autoAttackFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AutoLockTimeDefault
        {
            get
            {
                return this.autoLockTimeDefault;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat AutoUnlockTimeDefault
        {
            get
            {
                return this.autoUnlockTimeDefault;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BackMesh
        {
            get
            {
                return this.backMesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Backstab
        {
            get
            {
                return this.backstab;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BandolierSlots
        {
            get
            {
                return this.bandolierSlots;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BattlestationRep
        {
            get
            {
                return this.battlestationRep;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BattlestationSide
        {
            get
            {
                return this.battlestationSide;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BeltSlots
        {
            get
            {
                return this.beltSlots;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BerserkMode
        {
            get
            {
                return this.berserkMode;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill BiologicalMetamorphose
        {
            get
            {
                return this.biologicalMetamorphose;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BirthDate
        {
            get
            {
                return this.birthDate;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill BodyDevelopment
        {
            get
            {
                return this.bodyDevelopment;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Bow
        {
            get
            {
                return this.bow;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill BowSpecialAttack
        {
            get
            {
                return this.bowSpecialAttack;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BrainType
        {
            get
            {
                return this.brainType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Brawl
        {
            get
            {
                return this.brawl;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill BreakingEntry
        {
            get
            {
                return this.breakingEntry;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Breed
        {
            get
            {
                return this.breed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BreedHostility
        {
            get
            {
                return this.breedHostility;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BreedLimit
        {
            get
            {
                return this.breedLimit;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BuildingComplexInst
        {
            get
            {
                return this.buildingComplexInst;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BuildingInstance
        {
            get
            {
                return this.buildingInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BuildingType
        {
            get
            {
                return this.buildingType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Burst
        {
            get
            {
                return this.burst;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BurstRecharge
        {
            get
            {
                return this.burstRecharge;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat BuyModifier
        {
            get
            {
                return this.buyModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Can
        {
            get
            {
                return this.can;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CanChangeClothes
        {
            get
            {
                return this.canChangeClothes;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CardOwnerInstance
        {
            get
            {
                return this.cardOwnerInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CardOwnerType
        {
            get
            {
                return this.cardOwnerType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Cash
        {
            get
            {
                return this.cash;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CastEffectType
        {
            get
            {
                return this.castEffectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CastSelfAbstractAnim
        {
            get
            {
                return this.castSelfAbstractAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CastSound
        {
            get
            {
                return this.castSound;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CastTargetAbstractAnim
        {
            get
            {
                return this.castTargetAbstractAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CatAnim
        {
            get
            {
                return this.catAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CatAnimFlags
        {
            get
            {
                return this.catAnimFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CatMesh
        {
            get
            {
                return this.catMesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChanceOfBreakOnDebuff
        {
            get
            {
                return this.chanceOfBreakOnDebuff;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChanceOfBreakOnSpellAttack
        {
            get
            {
                return this.chanceOfBreakOnSpellAttack;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChanceOfUse
        {
            get
            {
                return this.chanceOfUse;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChangeSideCount
        {
            get
            {
                return this.changeSideCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharRadius
        {
            get
            {
                return this.charRadius;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharState
        {
            get
            {
                return this.charState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharTmp1
        {
            get
            {
                return this.charTmp1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharTmp2
        {
            get
            {
                return this.charTmp2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharTmp3
        {
            get
            {
                return this.charTmp3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CharTmp4
        {
            get
            {
                return this.charTmp4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChemicalAC
        {
            get
            {
                return this.chemicalAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChemicalDamageModifier
        {
            get
            {
                return this.chemicalDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Chemistry
        {
            get
            {
                return this.chemistry;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ChestFlags
        {
            get
            {
                return this.chestFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CityInstance
        {
            get
            {
                return this.cityInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CityTerminalRechargePercent
        {
            get
            {
                return this.cityTerminalRechargePercent;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Clan
        {
            get
            {
                return this.clan;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanConserver
        {
            get
            {
                return this.clanConserver;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanDevoted
        {
            get
            {
                return this.clanDevoted;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanFinalized
        {
            get
            {
                return this.clanFinalized;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanGaia
        {
            get
            {
                return this.clanGaia;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanHierarchy
        {
            get
            {
                return this.clanHierarchy;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanInstance
        {
            get
            {
                return this.clanInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanItemInstance
        {
            get
            {
                return this.clanItemInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanItemType
        {
            get
            {
                return this.clanItemType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanLevel
        {
            get
            {
                return this.clanLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanPrice
        {
            get
            {
                return this.clanPrice;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanRedeemed
        {
            get
            {
                return this.clanRedeemed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanSentinels
        {
            get
            {
                return this.clanSentinels;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanType
        {
            get
            {
                return this.clanType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanUpkeepInterval
        {
            get
            {
                return this.clanUpkeepInterval;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClanVanguards
        {
            get
            {
                return this.clanVanguards;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ClientActivated
        {
            get
            {
                return this.clientActivated;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill CloseCombatInitiative
        {
            get
            {
                return this.closeCombatInitiative;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ColdAC
        {
            get
            {
                return this.coldAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ColdDamageModifier
        {
            get
            {
                return this.coldDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CollideCheckInterval
        {
            get
            {
                return this.collideCheckInterval;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CollisionRadius
        {
            get
            {
                return this.collisionRadius;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CommandRange
        {
            get
            {
                return this.commandRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Compulsion
        {
            get
            {
                return this.compulsion;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill ComputerLiteracy
        {
            get
            {
                return this.computerLiteracy;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Concealment
        {
            get
            {
                return this.concealment;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ConditionState
        {
            get
            {
                return this.conditionState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Conformity
        {
            get
            {
                return this.conformity;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CorpseAnimKey
        {
            get
            {
                return this.corpseAnimKey;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CorpseHash
        {
            get
            {
                return this.corpseHash;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CorpseInstance
        {
            get
            {
                return this.corpseInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CorpseType
        {
            get
            {
                return this.corpseType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CriticalDecrease
        {
            get
            {
                return this.criticalDecrease;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CriticalIncrease
        {
            get
            {
                return this.criticalIncrease;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrBodyLocation
        {
            get
            {
                return this.currBodyLocation;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentMass
        {
            get
            {
                return this.currentMass;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentMovementMode
        {
            get
            {
                return this.currentMovementMode;
            }
        }

        /// <summary>
        /// </summary>
        public StatNanoPoints CurrentNano
        {
            get
            {
                return this.currentNano;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentNcu
        {
            get
            {
                return this.currentNCU;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentPlayfield
        {
            get
            {
                return this.currentPlayfield;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentState
        {
            get
            {
                return this.currentState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat CurrentTime
        {
            get
            {
                return this.currentTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DamageBonus
        {
            get
            {
                return this.damageBonus;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DamageOverrideType
        {
            get
            {
                return this.damageOverrideType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DamageToNano
        {
            get
            {
                return this.damageToNano;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DamageToNanoMultiplier
        {
            get
            {
                return this.damageToNanoMultiplier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DeadTimer
        {
            get
            {
                return this.deadTimer;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DeathReason
        {
            get
            {
                return this.deathReason;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DebuffFormula
        {
            get
            {
                return this.debuffFormula;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DefaultAttackType
        {
            get
            {
                return this.defaultAttackType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DefaultPos
        {
            get
            {
                return this.defaultPos;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DesiredTargetDistance
        {
            get
            {
                return this.desiredTargetDistance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DieAnim
        {
            get
            {
                return this.dieAnim;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Dimach
        {
            get
            {
                return this.dimach;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill DisarmTrap
        {
            get
            {
                return this.disarmTrap;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DisplayCatAnim
        {
            get
            {
                return this.displayCATAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DisplayCatMesh
        {
            get
            {
                return this.displayCATMesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DistanceToSpawnpoint
        {
            get
            {
                return this.distanceToSpawnpoint;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill DistanceWeaponInitiative
        {
            get
            {
                return this.distanceWeaponInitiative;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DistrictNano
        {
            get
            {
                return this.districtNano;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DistrictNanoInterval
        {
            get
            {
                return this.districtNanoInterval;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Dms
        {
            get
            {
                return this.dms;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DmsModifier
        {
            get
            {
                return this.dmsModifier;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Dodge
        {
            get
            {
                return this.dodge;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DoorBlockTime
        {
            get
            {
                return this.doorBlockTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DoorFlags
        {
            get
            {
                return this.doorFlags;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill DriveAir
        {
            get
            {
                return this.driveAir;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill DriveGround
        {
            get
            {
                return this.driveGround;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill DriveWater
        {
            get
            {
                return this.driveWater;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Duck
        {
            get
            {
                return this.duck;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DudChance
        {
            get
            {
                return this.dudChance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat DurationModifier
        {
            get
            {
                return this.durationModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EffectBlue
        {
            get
            {
                return this.effectBlue;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EffectGreen
        {
            get
            {
                return this.effectGreen;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EffectIcon
        {
            get
            {
                return this.effectIcon;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EffectRed
        {
            get
            {
                return this.effectRed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EffectType
        {
            get
            {
                return this.effectType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill ElectricalEngineering
        {
            get
            {
                return this.electricalEngineering;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Energy
        {
            get
            {
                return this.energy;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EnergyAC
        {
            get
            {
                return this.energyAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EnergyDamageModifier
        {
            get
            {
                return this.energyDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EquipDelay
        {
            get
            {
                return this.equipDelay;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat EquippedWeapons
        {
            get
            {
                return this.equippedWeapons;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Evade
        {
            get
            {
                return this.evade;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExitInstance
        {
            get
            {
                return this.exitInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Expansion
        {
            get
            {
                return this.expansion;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExpansionPlayfield
        {
            get
            {
                return this.expansionPlayfield;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExtenalDoorInstance
        {
            get
            {
                return this.extenalDoorInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExtenalPlayfieldInstance
        {
            get
            {
                return this.extenalPlayfieldInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExtendedFlags
        {
            get
            {
                return this.extendedFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ExtendedTime
        {
            get
            {
                return this.extendedTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Extroverty
        {
            get
            {
                return this.extroverty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FabricType
        {
            get
            {
                return this.fabricType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Face
        {
            get
            {
                return this.face;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FaceTexture
        {
            get
            {
                return this.faceTexture;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FactionModifier
        {
            get
            {
                return this.factionModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FallDamage
        {
            get
            {
                return this.fallDamage;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill FastAttack
        {
            get
            {
                return this.fastAttack;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Fatness
        {
            get
            {
                return this.fatness;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Features
        {
            get
            {
                return this.features;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill FieldQuantumPhysics
        {
            get
            {
                return this.fieldQuantumPhysics;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FireAC
        {
            get
            {
                return this.fireAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FireDamageModifier
        {
            get
            {
                return this.fireDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill FirstAid
        {
            get
            {
                return this.firstAid;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FixtureFlags
        {
            get
            {
                return this.fixtureFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Flags
        {
            get
            {
                return this.flags;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill FlingShot
        {
            get
            {
                return this.flingShot;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill FullAuto
        {
            get
            {
                return this.fullAuto;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat FullAutoRecharge
        {
            get
            {
                return this.fullAutoRecharge;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GMLevel
        {
            get
            {
                return this.gmLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GatherAbstractAnim
        {
            get
            {
                return this.gatherAbstractAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GatherEffectType
        {
            get
            {
                return this.gatherEffectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GatherSound
        {
            get
            {
                return this.gatherSound;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GenderLimit
        {
            get
            {
                return this.genderLimit;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GlobalClanInstance
        {
            get
            {
                return this.globalClanInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GlobalClanType
        {
            get
            {
                return this.globalClanType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GlobalResearchGoal
        {
            get
            {
                return this.globalResearchGoal;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat GlobalResearchLevel
        {
            get
            {
                return this.globalResearchLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Gos
        {
            get
            {
                return this.gos;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Grenade
        {
            get
            {
                return this.grenade;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HPLevelUp
        {
            get
            {
                return this.hpLevelUp;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HPPerSkill
        {
            get
            {
                return this.hpPerSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HairMesh
        {
            get
            {
                return this.hairMesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HasAlwaysLootable
        {
            get
            {
                return this.hasAlwaysLootable;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HasKnuBotData
        {
            get
            {
                return this.hasKnuBotData;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HateValueModifyer
        {
            get
            {
                return this.hateValueModifyer;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HeadMesh
        {
            get
            {
                return this.headMesh;
            }
        }

        /// <summary>
        /// </summary>
        public StatHealDelta HealDelta
        {
            get
            {
                return this.healDelta;
            }
        }

        /// <summary>
        /// </summary>
        public StatHealInterval HealInterval
        {
            get
            {
                return this.healInterval;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HealMultiplier
        {
            get
            {
                return this.healMultiplier;
            }
        }

        /// <summary>
        /// </summary>
        public StatHitPoints Health
        {
            get
            {
                return this.health;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HealthChange
        {
            get
            {
                return this.healthChange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HealthChangeBest
        {
            get
            {
                return this.healthChangeBest;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HealthChangeWorst
        {
            get
            {
                return this.healthChangeWorst;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HitEffectType
        {
            get
            {
                return this.hitEffectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HitSound
        {
            get
            {
                return this.hitSound;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat HouseTemplate
        {
            get
            {
                return this.houseTemplate;
            }
        }

        /// <summary>
        /// </summary>
        public StatIP IP
        {
            get
            {
                return this.ip;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Icon
        {
            get
            {
                return this.icon;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ImpactEffectType
        {
            get
            {
                return this.impactEffectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InPlay
        {
            get
            {
                return this.inPlay;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Info
        {
            get
            {
                return this.info;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InitiativeType
        {
            get
            {
                return this.initiativeType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Instance
        {
            get
            {
                return this.instance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InsurancePercentage
        {
            get
            {
                return this.insurancePercentage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InsuranceTime
        {
            get
            {
                return this.insuranceTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Intelligence
        {
            get
            {
                return this.intelligence;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InteractionRadius
        {
            get
            {
                return this.interactionRadius;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InterruptModifier
        {
            get
            {
                return this.interruptModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InvadersKilled
        {
            get
            {
                return this.invadersKilled;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InventoryId
        {
            get
            {
                return this.inventoryId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat InventoryTimeout
        {
            get
            {
                return this.inventoryTimeout;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat IsFightingMe
        {
            get
            {
                return this.isFightingMe;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat IsVehicle
        {
            get
            {
                return this.isVehicle;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemAnim
        {
            get
            {
                return this.itemAnim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemClass
        {
            get
            {
                return this.itemClass;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemDelay
        {
            get
            {
                return this.itemDelay;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemDelayCap
        {
            get
            {
                return this.itemDelayCap;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemHateValue
        {
            get
            {
                return this.itemHateValue;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemOpposedSkill
        {
            get
            {
                return this.itemOpposedSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemSis
        {
            get
            {
                return this.itemSIS;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemSkill
        {
            get
            {
                return this.itemSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat KilledByInvaders
        {
            get
            {
                return this.killedByInvaders;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill LREnergyWeapon
        {
            get
            {
                return this.lrEnergyWeapon;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill LRMultipleWeapon
        {
            get
            {
                return this.lrMultipleWeapon;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastConcretePlayfieldInstance
        {
            get
            {
                return this.lastConcretePlayfieldInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastMailCheckTime
        {
            get
            {
                return this.lastMailCheckTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastPerkResetTime
        {
            get
            {
                return this.lastPerkResetTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastRnd
        {
            get
            {
                return this.lastRnd;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastSK
        {
            get
            {
                return this.lastSK;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastSaveXP
        {
            get
            {
                return this.lastSaveXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastSaved
        {
            get
            {
                return this.lastSaved;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LastXP
        {
            get
            {
                return this.lastXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LeaderLockDownTime
        {
            get
            {
                return this.leaderLockDownTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Level
        {
            get
            {
                return this.level;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LevelLimit
        {
            get
            {
                return this.levelLimit;
            }
        }

        /// <summary>
        /// </summary>
        public StatHealth Life
        {
            get
            {
                return this.life;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LiquidType
        {
            get
            {
                return this.liquidType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LockDifficulty
        {
            get
            {
                return this.lockDifficulty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LockDownTime
        {
            get
            {
                return this.lockDownTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LosHeight
        {
            get
            {
                return this.losHeight;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat LowresMesh
        {
            get
            {
                return this.lowresMesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapAreaPart1
        {
            get
            {
                return this.mapAreaPart1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapAreaPart2
        {
            get
            {
                return this.mapAreaPart2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapAreaPart3
        {
            get
            {
                return this.mapAreaPart3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapAreaPart4
        {
            get
            {
                return this.mapAreaPart4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapFlags
        {
            get
            {
                return this.mapFlags;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MapNavigation
        {
            get
            {
                return this.mapNavigation;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MapOptions
        {
            get
            {
                return this.mapOptions;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MartialArts
        {
            get
            {
                return this.martialArts;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MaterialCreation
        {
            get
            {
                return this.materialCreation;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MaterialLocation
        {
            get
            {
                return this.materialLocation;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MaterialMetamorphose
        {
            get
            {
                return this.materialMetamorphose;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MaxDamage
        {
            get
            {
                return this.maxDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MaxEnergy
        {
            get
            {
                return this.maxEnergy;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MaxMass
        {
            get
            {
                return this.maxMass;
            }
        }

        /// <summary>
        /// </summary>
        public StatNano MaxNanoEnergy
        {
            get
            {
                return this.maxNanoEnergy;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MaxNcu
        {
            get
            {
                return this.maxNCU;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MaxShopItems
        {
            get
            {
                return this.maxShopItems;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MechData
        {
            get
            {
                return this.mechData;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MechanicalEngineering
        {
            get
            {
                return this.mechanicalEngineering;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MeleeAC
        {
            get
            {
                return this.meleeAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MeleeDamageModifier
        {
            get
            {
                return this.meleeDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MeleeEnergyWeapon
        {
            get
            {
                return this.meleeEnergyWeapon;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill MeleeMultiple
        {
            get
            {
                return this.meleeMultiple;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MemberInstance
        {
            get
            {
                return this.memberInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MemberType
        {
            get
            {
                return this.memberType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Members
        {
            get
            {
                return this.members;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Mesh
        {
            get
            {
                return this.mesh;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MetaType
        {
            get
            {
                return this.metaType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MetersWalked
        {
            get
            {
                return this.metersWalked;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MinDamage
        {
            get
            {
                return this.minDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MinMembers
        {
            get
            {
                return this.minMembers;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits1
        {
            get
            {
                return this.missionBits1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits10
        {
            get
            {
                return this.missionBits10;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits11
        {
            get
            {
                return this.missionBits11;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits12
        {
            get
            {
                return this.missionBits12;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits2
        {
            get
            {
                return this.missionBits2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits3
        {
            get
            {
                return this.missionBits3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits4
        {
            get
            {
                return this.missionBits4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits5
        {
            get
            {
                return this.missionBits5;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits6
        {
            get
            {
                return this.missionBits6;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits7
        {
            get
            {
                return this.missionBits7;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits8
        {
            get
            {
                return this.missionBits8;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MissionBits9
        {
            get
            {
                return this.missionBits9;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MonsterData
        {
            get
            {
                return this.monsterData;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MonsterLevelsKilled
        {
            get
            {
                return this.monsterLevelsKilled;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MonsterScale
        {
            get
            {
                return this.monsterScale;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MonsterTexture
        {
            get
            {
                return this.monsterTexture;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MonthsPaid
        {
            get
            {
                return this.monthsPaid;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MoreFlags
        {
            get
            {
                return this.moreFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat MultipleCount
        {
            get
            {
                return this.multipleCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NPCostModifier
        {
            get
            {
                return this.npCostModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NPLevelUp
        {
            get
            {
                return this.npLevelUp;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NPPerSkill
        {
            get
            {
                return this.npPerSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NameTemplate
        {
            get
            {
                return this.nameTemplate;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill NanoAC
        {
            get
            {
                return this.nanoAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoDamageModifier
        {
            get
            {
                return this.nanoDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoDamageMultiplier
        {
            get
            {
                return this.nanoDamageMultiplier;
            }
        }

        /// <summary>
        /// </summary>
        public StatNanoDelta NanoDelta
        {
            get
            {
                return this.nanoDelta;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill NanoEnergyPool
        {
            get
            {
                return this.nanoEnergyPool;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoFocusLevel
        {
            get
            {
                return this.nanoFocusLevel;
            }
        }

        /// <summary>
        /// </summary>
        public StatNanoInterval NanoInterval
        {
            get
            {
                return this.nanoInterval;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoPoints
        {
            get
            {
                return this.nanoPoints;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill NanoProgramming
        {
            get
            {
                return this.nanoProgramming;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill NanoProwessInitiative
        {
            get
            {
                return this.nanoProwessInitiative;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoSpeed
        {
            get
            {
                return this.nanoSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NanoVulnerability
        {
            get
            {
                return this.nanoVulnerability;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NewbieHP
        {
            get
            {
                return this.newbieHP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NewbieNP
        {
            get
            {
                return this.newbieNP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NextDoorInBuilding
        {
            get
            {
                return this.nextDoorInBuilding;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NextFormula
        {
            get
            {
                return this.nextFormula;
            }
        }

        /// <summary>
        /// </summary>
        public StatNextSK NextSK
        {
            get
            {
                return this.nextSK;
            }
        }

        /// <summary>
        /// </summary>
        public StatNextXP NextXP
        {
            get
            {
                return this.nextXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcBrainState
        {
            get
            {
                return this.npcBrainState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcCommand
        {
            get
            {
                return this.npcCommand;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcCommandArg
        {
            get
            {
                return this.npcCommandArg;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcCryForHelpRange
        {
            get
            {
                return this.npcCryForHelpRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcFamily
        {
            get
            {
                return this.npcFamily;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcFlags
        {
            get
            {
                return this.npcFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcFovStatus
        {
            get
            {
                return this.npcFovStatus;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcHasPatrolList
        {
            get
            {
                return this.npcHasPatrolList;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcHash
        {
            get
            {
                return this.npcHash;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcHatelistSize
        {
            get
            {
                return this.npcHatelistSize;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcIsSurrendering
        {
            get
            {
                return this.npcIsSurrendering;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcNumPets
        {
            get
            {
                return this.npcNumPets;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcScriptAmsScale
        {
            get
            {
                return this.npcScriptAmsScale;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcSpellArg1
        {
            get
            {
                return this.npcSpellArg1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcSpellRet1
        {
            get
            {
                return this.npcSpellRet1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcSurrenderInstance
        {
            get
            {
                return this.npcSurrenderInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcUseFightModeRegenRate
        {
            get
            {
                return this.npcUseFightModeRegenRate;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcVicinityChars
        {
            get
            {
                return this.npcVicinityChars;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcVicinityFamily
        {
            get
            {
                return this.npcVicinityFamily;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NpcVicinityPlayers
        {
            get
            {
                return this.npcVicinityPlayers;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NumAttackEffects
        {
            get
            {
                return this.numAttackEffects;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NumberOfItems
        {
            get
            {
                return this.numberOfItems;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NumberOfTeamMembers
        {
            get
            {
                return this.numberOfTeamMembers;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat NumberOnHateList
        {
            get
            {
                return this.numberOnHateList;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ODMaxSizeAdd
        {
            get
            {
                return this.odMaxSizeAdd;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ODMinSizeAdd
        {
            get
            {
                return this.odMinSizeAdd;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTArmedForces
        {
            get
            {
                return this.otArmedForces;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTFollowers
        {
            get
            {
                return this.otFollowers;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTMed
        {
            get
            {
                return this.otMed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTOperator
        {
            get
            {
                return this.otOperator;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTTrans
        {
            get
            {
                return this.otTrans;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OTUnredeemed
        {
            get
            {
                return this.otUnredeemed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ObjectType
        {
            get
            {
                return this.objectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OldTimeExist
        {
            get
            {
                return this.oldTimeExist;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OnTowerCreation
        {
            get
            {
                return this.onTowerCreation;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill OnehBluntWeapons
        {
            get
            {
                return this.onehBluntWeapons;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill OnehEdgedWeapon
        {
            get
            {
                return this.onehEdgedWeapon;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OrientationMode
        {
            get
            {
                return this.orientationMode;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OriginatorType
        {
            get
            {
                return this.originatorType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OuterRadius
        {
            get
            {
                return this.outerRadius;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideMaterial
        {
            get
            {
                return this.overrideMaterial;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTexture
        {
            get
            {
                return this.overrideTexture;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureAttractor
        {
            get
            {
                return this.overrideTextureAttractor;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureBack
        {
            get
            {
                return this.overrideTextureBack;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureHead
        {
            get
            {
                return this.overrideTextureHead;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureShoulderpadLeft
        {
            get
            {
                return this.overrideTextureShoulderpadLeft;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureShoulderpadRight
        {
            get
            {
                return this.overrideTextureShoulderpadRight;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureWeaponLeft
        {
            get
            {
                return this.overrideTextureWeaponLeft;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OverrideTextureWeaponRight
        {
            get
            {
                return this.overrideTextureWeaponRight;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OwnedTowers
        {
            get
            {
                return this.ownedTowers;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat OwnerInstance
        {
            get
            {
                return this.ownerInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PaidPoints
        {
            get
            {
                return this.paidPoints;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ParentInstance
        {
            get
            {
                return this.parentInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ParentType
        {
            get
            {
                return this.parentType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Parry
        {
            get
            {
                return this.parry;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentChemicalDamage
        {
            get
            {
                return this.percentChemicalDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentColdDamage
        {
            get
            {
                return this.percentColdDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentEnergyDamage
        {
            get
            {
                return this.percentEnergyDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentFireDamage
        {
            get
            {
                return this.percentFireDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentMeleeDamage
        {
            get
            {
                return this.percentMeleeDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentPoisonDamage
        {
            get
            {
                return this.percentPoisonDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentProjectileDamage
        {
            get
            {
                return this.percentProjectileDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentRadiationDamage
        {
            get
            {
                return this.percentRadiationDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentRemainingHealth
        {
            get
            {
                return this.percentRemainingHealth;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PercentRemainingNano
        {
            get
            {
                return this.percentRemainingNano;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Perception
        {
            get
            {
                return this.perception;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PersonalResearchGoal
        {
            get
            {
                return this.personalResearchGoal;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PersonalResearchLevel
        {
            get
            {
                return this.personalResearchLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetCounter
        {
            get
            {
                return this.petCounter;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetMaster
        {
            get
            {
                return this.petMaster;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReq1
        {
            get
            {
                return this.petReq1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReq2
        {
            get
            {
                return this.petReq2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReq3
        {
            get
            {
                return this.petReq3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReqVal1
        {
            get
            {
                return this.petReqVal1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReqVal2
        {
            get
            {
                return this.petReqVal2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetReqVal3
        {
            get
            {
                return this.petReqVal3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetState
        {
            get
            {
                return this.petState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PetType
        {
            get
            {
                return this.petType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Pharmaceuticals
        {
            get
            {
                return this.pharmaceuticals;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill PhysicalProwessInitiative
        {
            get
            {
                return this.physicalProwessInitiative;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Piercing
        {
            get
            {
                return this.piercing;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Pistol
        {
            get
            {
                return this.pistol;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Placement
        {
            get
            {
                return this.placement;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PlayerId
        {
            get
            {
                return this.playerId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PlayerKilling
        {
            get
            {
                return this.playerKilling;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PlayerOptions
        {
            get
            {
                return this.playerOptions;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PlayfieldType
        {
            get
            {
                return this.playfieldType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PoisonAC
        {
            get
            {
                return this.poisonAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PoisonDamageModifier
        {
            get
            {
                return this.poisonDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PrevMovementMode
        {
            get
            {
                return this.prevMovementMode;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PreviousHealth
        {
            get
            {
                return this.previousHealth;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Price
        {
            get
            {
                return this.price;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PrimaryItemInstance
        {
            get
            {
                return this.primaryItemInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PrimaryItemType
        {
            get
            {
                return this.primaryItemType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PrimaryTemplateId
        {
            get
            {
                return this.primaryTemplateId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcChance1
        {
            get
            {
                return this.procChance1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcChance2
        {
            get
            {
                return this.procChance2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcChance3
        {
            get
            {
                return this.procChance3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcChance4
        {
            get
            {
                return this.procChance4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcInitiative1
        {
            get
            {
                return this.procInitiative1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcInitiative2
        {
            get
            {
                return this.procInitiative2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcInitiative3
        {
            get
            {
                return this.procInitiative3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcInitiative4
        {
            get
            {
                return this.procInitiative4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcNano1
        {
            get
            {
                return this.procNano1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcNano2
        {
            get
            {
                return this.procNano2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcNano3
        {
            get
            {
                return this.procNano3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProcNano4
        {
            get
            {
                return this.procNano4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Profession
        {
            get
            {
                return this.profession;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProfessionLevel
        {
            get
            {
                return this.professionLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProjectileAC
        {
            get
            {
                return this.projectileAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProjectileDamageModifier
        {
            get
            {
                return this.projectileDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProximityRangeIndoors
        {
            get
            {
                return this.proximityRangeIndoors;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ProximityRangeOutdoors
        {
            get
            {
                return this.proximityRangeOutdoors;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Psychic
        {
            get
            {
                return this.psychic;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill PsychologicalModification
        {
            get
            {
                return this.psychologicalModification;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Psychology
        {
            get
            {
                return this.psychology;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvPLevelsKilled
        {
            get
            {
                return this.pvPLevelsKilled;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpDuelDeaths
        {
            get
            {
                return this.pvpDuelDeaths;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpDuelKills
        {
            get
            {
                return this.pvpDuelKills;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpDuelScore
        {
            get
            {
                return this.pvpDuelScore;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpProfessionDuelDeaths
        {
            get
            {
                return this.pvpProfessionDuelDeaths;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpProfessionDuelKills
        {
            get
            {
                return this.pvpProfessionDuelKills;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpRankedSoloDeaths
        {
            get
            {
                return this.pvpRankedSoloDeaths;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpRankedSoloKills
        {
            get
            {
                return this.pvpRankedSoloKills;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpRankedTeamDeaths
        {
            get
            {
                return this.pvpRankedTeamDeaths;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpRankedTeamKills
        {
            get
            {
                return this.pvpRankedTeamKills;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpRating
        {
            get
            {
                return this.pvpRating;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpSoloScore
        {
            get
            {
                return this.pvpSoloScore;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat PvpTeamScore
        {
            get
            {
                return this.pvpTeamScore;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTDungeonInstance
        {
            get
            {
                return this.qtDungeonInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterCount1
        {
            get
            {
                return this.qtKillNumMonsterCount1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterCount2
        {
            get
            {
                return this.qtKillNumMonsterCount2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterCount3
        {
            get
            {
                return this.qtKillNumMonsterCount3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterId1
        {
            get
            {
                return this.qtKillNumMonsterId1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterId2
        {
            get
            {
                return this.qtKillNumMonsterId2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKillNumMonsterId3
        {
            get
            {
                return this.qtKillNumMonsterID3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTKilledMonsters
        {
            get
            {
                return this.qtKilledMonsters;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QTNumMonsters
        {
            get
            {
                return this.qtNumMonsters;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestAsMaximumRange
        {
            get
            {
                return this.questAsMaximumRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestAsMinimumRange
        {
            get
            {
                return this.questAsMinimumRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestBoothDifficulty
        {
            get
            {
                return this.questBoothDifficulty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex0
        {
            get
            {
                return this.questIndex0;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex1
        {
            get
            {
                return this.questIndex1;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex2
        {
            get
            {
                return this.questIndex2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex3
        {
            get
            {
                return this.questIndex3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex4
        {
            get
            {
                return this.questIndex4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestIndex5
        {
            get
            {
                return this.questIndex5;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestInstance
        {
            get
            {
                return this.questInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestLevelsSolved
        {
            get
            {
                return this.questLevelsSolved;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestStat
        {
            get
            {
                return this.questStat;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat QuestTimeout
        {
            get
            {
                return this.questTimeout;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RP
        {
            get
            {
                return this.rp;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Race
        {
            get
            {
                return this.race;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RadiationAC
        {
            get
            {
                return this.radiationAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RadiationDamageModifier
        {
            get
            {
                return this.radiationDamageModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RangeIncreaserNF
        {
            get
            {
                return this.rangeIncreaserNF;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RangeIncreaserWeapon
        {
            get
            {
                return this.rangeIncreaserWeapon;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReadOnly
        {
            get
            {
                return this.readOnly;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RechargeDelay
        {
            get
            {
                return this.rechargeDelay;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RechargeDelayCap
        {
            get
            {
                return this.rechargeDelayCap;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReclaimItem
        {
            get
            {
                return this.reclaimItem;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectChemicalAC
        {
            get
            {
                return this.reflectChemicalAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectColdAC
        {
            get
            {
                return this.reflectColdAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectEnergyAC
        {
            get
            {
                return this.reflectEnergyAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectFireAC
        {
            get
            {
                return this.reflectFireAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectMeleeAC
        {
            get
            {
                return this.reflectMeleeAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectNanoAC
        {
            get
            {
                return this.reflectNanoAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectPoisonAC
        {
            get
            {
                return this.reflectPoisonAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectProjectileAC
        {
            get
            {
                return this.reflectProjectileAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectRadiationAC
        {
            get
            {
                return this.reflectRadiationAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedChemicalAC
        {
            get
            {
                return this.reflectReturnedChemicalAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedColdAC
        {
            get
            {
                return this.reflectReturnedColdAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedEnergyAC
        {
            get
            {
                return this.reflectReturnedEnergyAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedFireAC
        {
            get
            {
                return this.reflectReturnedFireAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedMeleeAC
        {
            get
            {
                return this.reflectReturnedMeleeAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedNanoAC
        {
            get
            {
                return this.reflectReturnedNanoAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedPoisonAC
        {
            get
            {
                return this.reflectReturnedPoisonAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedProjectileAC
        {
            get
            {
                return this.reflectReturnedProjectileAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ReflectReturnedRadiationAC
        {
            get
            {
                return this.reflectReturnedRadiationAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RegainXPPercentage
        {
            get
            {
                return this.regainXPPercentage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RepairDifficulty
        {
            get
            {
                return this.repairDifficulty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RepairSkill
        {
            get
            {
                return this.repairSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ResistModifier
        {
            get
            {
                return this.resistModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RestModifier
        {
            get
            {
                return this.restModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ResurrectDest
        {
            get
            {
                return this.resurrectDest;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Rifle
        {
            get
            {
                return this.rifle;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Riposte
        {
            get
            {
                return this.riposte;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat RitualTargetInst
        {
            get
            {
                return this.ritualTargetInst;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Rnd
        {
            get
            {
                return this.rnd;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Rotation
        {
            get
            {
                return this.rotation;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill RunSpeed
        {
            get
            {
                return this.runSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SK
        {
            get
            {
                return this.sk;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SavedXP
        {
            get
            {
                return this.savedXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat School
        {
            get
            {
                return this.school;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SecondaryItemInstance
        {
            get
            {
                return this.secondaryItemInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SecondaryItemTemplate
        {
            get
            {
                return this.secondaryItemTemplate;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SecondaryItemType
        {
            get
            {
                return this.secondaryItemType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SelectedTarget
        {
            get
            {
                return this.selectedTarget;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SelectedTargetType
        {
            get
            {
                return this.selectedTargetType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SellModifier
        {
            get
            {
                return this.sellModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Sense
        {
            get
            {
                return this.sense;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill SenseImprovement
        {
            get
            {
                return this.senseImprovement;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SessionTime
        {
            get
            {
                return this.sessionTime;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Sex
        {
            get
            {
                return this.sex;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShadowBreed
        {
            get
            {
                return this.shadowBreed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShadowBreedTemplate
        {
            get
            {
                return this.shadowBreedTemplate;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldChemicalAC
        {
            get
            {
                return this.shieldChemicalAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldColdAC
        {
            get
            {
                return this.shieldColdAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldEnergyAC
        {
            get
            {
                return this.shieldEnergyAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldFireAC
        {
            get
            {
                return this.shieldFireAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldMeleeAC
        {
            get
            {
                return this.shieldMeleeAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldNanoAC
        {
            get
            {
                return this.shieldNanoAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldPoisonAC
        {
            get
            {
                return this.shieldPoisonAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldProjectileAC
        {
            get
            {
                return this.shieldProjectileAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShieldRadiationAC
        {
            get
            {
                return this.shieldRadiationAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopFlags
        {
            get
            {
                return this.shopFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopId
        {
            get
            {
                return this.shopId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopIndex
        {
            get
            {
                return this.shopIndex;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopLastUsed
        {
            get
            {
                return this.shopLastUsed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopPrice
        {
            get
            {
                return this.shopPrice;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopRent
        {
            get
            {
                return this.shopRent;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShopType
        {
            get
            {
                return this.shopType;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Shotgun
        {
            get
            {
                return this.shotgun;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShoulderMeshHolder
        {
            get
            {
                return this.shoulderMeshHolder;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShoulderMeshLeft
        {
            get
            {
                return this.shoulderMeshLeft;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat ShoulderMeshRight
        {
            get
            {
                return this.shoulderMeshRight;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Side
        {
            get
            {
                return this.side;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SisCap
        {
            get
            {
                return this.sisCap;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SkillDisabled
        {
            get
            {
                return this.skillDisabled;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SkillLockModifier
        {
            get
            {
                return this.skillLockModifier;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SkillTimeOnSelectedTarget
        {
            get
            {
                return this.skillTimeOnSelectedTarget;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill SneakAttack
        {
            get
            {
                return this.sneakAttack;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SocialStatus
        {
            get
            {
                return this.socialStatus;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SoundVolume
        {
            get
            {
                return this.soundVolume;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SpecialAttackShield
        {
            get
            {
                return this.specialAttackShield;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SpecialCondition
        {
            get
            {
                return this.specialCondition;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Specialization
        {
            get
            {
                return this.specialization;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SpeedPenalty
        {
            get
            {
                return this.speedPenalty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Stability
        {
            get
            {
                return this.stability;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingLine2
        {
            get
            {
                return this.stackingLine2;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingLine3
        {
            get
            {
                return this.stackingLine3;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingLine4
        {
            get
            {
                return this.stackingLine4;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingLine5
        {
            get
            {
                return this.stackingLine5;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingLine6
        {
            get
            {
                return this.stackingLine6;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StackingOrder
        {
            get
            {
                return this.stackingOrder;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Stamina
        {
            get
            {
                return this.stamina;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StatOne
        {
            get
            {
                return this.statOne;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StateAction
        {
            get
            {
                return this.stateAction;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StateMachine
        {
            get
            {
                return this.stateMachine;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StaticInstance
        {
            get
            {
                return this.staticInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StaticType
        {
            get
            {
                return this.staticType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat StreamCheckMagic
        {
            get
            {
                return this.streamCheckMagic;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Strength
        {
            get
            {
                return this.strength;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill SubMachineGun
        {
            get
            {
                return this.subMachineGun;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Swim
        {
            get
            {
                return this.swim;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat SynergyHash
        {
            get
            {
                return this.synergyHash;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Taboo
        {
            get
            {
                return this.taboo;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TargetDistance
        {
            get
            {
                return this.targetDistance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TargetDistanceChange
        {
            get
            {
                return this.targetDistanceChange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TargetFacing
        {
            get
            {
                return this.targetFacing;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat Team
        {
            get
            {
                return this.team;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TeamAllowed
        {
            get
            {
                return this.teamAllowed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TeamCloseness
        {
            get
            {
                return this.teamCloseness;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TeamSide
        {
            get
            {
                return this.teamSide;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TeleportPauseMilliSeconds
        {
            get
            {
                return this.teleportPauseMilliSeconds;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TempSavePlayfield
        {
            get
            {
                return this.tempSavePlayfield;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TempSaveTeamId
        {
            get
            {
                return this.tempSaveTeamId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TempSaveX
        {
            get
            {
                return this.tempSaveX;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TempSaveY
        {
            get
            {
                return this.tempSaveY;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TemporarySkillReduction
        {
            get
            {
                return this.temporarySkillReduction;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill ThrowingKnife
        {
            get
            {
                return this.throwingKnife;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill ThrownGrapplingWeapons
        {
            get
            {
                return this.thrownGrapplingWeapons;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TideRequiredDynelId
        {
            get
            {
                return this.tideRequiredDynelId;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TimeExist
        {
            get
            {
                return this.timeExist;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TimeSinceCreation
        {
            get
            {
                return this.timeSinceCreation;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TimeSinceUpkeep
        {
            get
            {
                return this.timeSinceUpkeep;
            }
        }

        /// <summary>
        /// </summary>
        public StatTitleLevel TitleLevel
        {
            get
            {
                return this.titleLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TotalDamage
        {
            get
            {
                return this.totalDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TotalMass
        {
            get
            {
                return this.totalMass;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TowerInstance
        {
            get
            {
                return this.towerInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TowerNpcHash
        {
            get
            {
                return this.towerNpcHash;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TowerType
        {
            get
            {
                return this.towerType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TracerEffectType
        {
            get
            {
                return this.tracerEffectType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackChemicalDamage
        {
            get
            {
                return this.trackChemicalDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackColdDamage
        {
            get
            {
                return this.trackColdDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackEnergyDamage
        {
            get
            {
                return this.trackEnergyDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackFireDamage
        {
            get
            {
                return this.trackFireDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackMeleeDamage
        {
            get
            {
                return this.trackMeleeDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackPoisonDamage
        {
            get
            {
                return this.trackPoisonDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackProjectileDamage
        {
            get
            {
                return this.trackProjectileDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrackRadiationDamage
        {
            get
            {
                return this.trackRadiationDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TradeLimit
        {
            get
            {
                return this.tradeLimit;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrainSkill
        {
            get
            {
                return this.trainSkill;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrainSkillCost
        {
            get
            {
                return this.trainSkillCost;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TrapDifficulty
        {
            get
            {
                return this.trapDifficulty;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TravelSound
        {
            get
            {
                return this.travelSound;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Treatment
        {
            get
            {
                return this.treatment;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat TurnSpeed
        {
            get
            {
                return this.turnSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill Tutoring
        {
            get
            {
                return this.tutoring;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill TwohBluntWeapons
        {
            get
            {
                return this.twohBluntWeapons;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill TwohEdgedWeapons
        {
            get
            {
                return this.twohEdgedWeapons;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat UnarmedTemplateInstance
        {
            get
            {
                return this.unarmedTemplateInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat UnreadMailCount
        {
            get
            {
                return this.unreadMailCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat UnsavedXP
        {
            get
            {
                return this.unsavedXP;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat UserInstance
        {
            get
            {
                return this.userInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat UserType
        {
            get
            {
                return this.userType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VehicleAC
        {
            get
            {
                return this.vehicleAC;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VehicleDamage
        {
            get
            {
                return this.vehicleDamage;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VehicleHealth
        {
            get
            {
                return this.vehicleHealth;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VehicleSpeed
        {
            get
            {
                return this.vehicleSpeed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VeteranPoints
        {
            get
            {
                return this.veteranPoints;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VicinityRange
        {
            get
            {
                return this.vicinityRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VictoryPoints
        {
            get
            {
                return this.victoryPoints;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VisualBreed
        {
            get
            {
                return this.visualBreed;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VisualFlags
        {
            get
            {
                return this.visualFlags;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VisualLodLevel
        {
            get
            {
                return this.visualLodLevel;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VisualProfession
        {
            get
            {
                return this.visualProfession;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VisualSex
        {
            get
            {
                return this.visualSex;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VolumeMass
        {
            get
            {
                return this.volumeMass;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat VoteCount
        {
            get
            {
                return this.voteCount;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WaitState
        {
            get
            {
                return this.waitState;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponDisallowedInstance
        {
            get
            {
                return this.weaponDisallowedInstance;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponDisallowedType
        {
            get
            {
                return this.weaponDisallowedType;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponMeshHolder
        {
            get
            {
                return this.weaponMeshHolder;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponMeshLeft
        {
            get
            {
                return this.weaponMeshLeft;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponMeshRight
        {
            get
            {
                return this.weaponMeshRight;
            }
        }

        /// <summary>
        /// </summary>
        public StatSkill WeaponSmithing
        {
            get
            {
                return this.weaponSmithing;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponStyleLeft
        {
            get
            {
                return this.weaponStyleLeft;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponStyleRight
        {
            get
            {
                return this.weaponStyleRight;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat WeaponsStyle
        {
            get
            {
                return this.weaponsStyle;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat XP
        {
            get
            {
                return this.xp;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat XPBonus
        {
            get
            {
                return this.xpBonus;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat XPKillRange
        {
            get
            {
                return this.xpKillRange;
            }
        }

        /// <summary>
        /// </summary>
        public ClassStat XPModifier
        {
            get
            {
                return this.xpModifier;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        public void ClearChangedFlags()
        {
            foreach (ClassStat cs in this.all)
            {
                cs.Changed = false;
            }
        }

        /// <summary>
        /// </summary>
        public void ClearModifiers()
        {
            foreach (ClassStat c in this.all)
            {
                c.StatModifier = 0;
                c.StatPercentageModifier = 100;
                c.Trickle = 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public uint GetBaseValue(int stat)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                return c.StatBaseValue;
            }

            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetBaseValue");
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public int GetModifier(int stat)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                return c.StatModifier;
            }

            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetModifier");
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public int GetPercentageModifier(int stat)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                return c.StatPercentageModifier;
            }

            throw new StatDoesNotExistException("Stat " + stat + " does not exist.\r\nMethod: GetPercentageModifier");
        }

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// </param>
        /// <returns>
        /// </returns>
        public ClassStat GetStatbyNumber(int number)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }

                return c;
            }

            return null;
        }

        /// <summary>
        /// Read all stats from Sql
        /// </summary>
        public void ReadStatsfromSql()
        {
            foreach (StatDao statDao in DBStats.GetById(this.flags.Parent.Identity.Instance))
            {
                this.SetBaseValue(statDao.Stat, statDao.Value);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        public void Send(object sender, StatChangedEventArgs e)
        {
            Contract.Requires(sender != null);
            Contract.Requires(((ClassStat)sender).Parent != null);

            if (!((ClassStat)sender).Parent.DoNotDoTimers)
            {
                Stat.Send(e.Stat.Parent, e.Stat.StatNumber, e.NewValue, e.Stat.AnnounceToPlayfield);

                e.Stat.Changed = false;
            }
        }

        /// <summary>
        /// </summary>
        public void SetAbilityTricklers()
        {
            for (int c = 0; c < SkillTrickleTable.table.Length / 7; c++)
            {
                int skillnum = Convert.ToInt32(SkillTrickleTable.table[c, 0]);
                if (SkillTrickleTable.table[c, 1] > 0)
                {
                    this.strength.Affects.Add(skillnum);
                }

                if (SkillTrickleTable.table[c, 2] > 0)
                {
                    this.stamina.Affects.Add(skillnum);
                }

                if (SkillTrickleTable.table[c, 3] > 0)
                {
                    this.sense.Affects.Add(skillnum);
                }

                if (SkillTrickleTable.table[c, 4] > 0)
                {
                    this.agility.Affects.Add(skillnum);
                }

                if (SkillTrickleTable.table[c, 5] > 0)
                {
                    this.intelligence.Affects.Add(skillnum);
                }

                if (SkillTrickleTable.table[c, 6] > 0)
                {
                    this.psychic.Affects.Add(skillnum);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public void SetBaseValue(int stat, uint value)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                c.Changed = c.StatBaseValue != value;
                c.StatBaseValue = value;
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetBaseValue");
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public void SetModifier(int stat, int value)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                c.StatModifier = value;
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetModifier");
        }

        /// <summary>
        /// </summary>
        /// <param name="stat">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public void SetPercentageModifier(int stat, int value)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != stat)
                {
                    continue;
                }

                c.StatPercentageModifier = value;
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + stat + " does not exist.\r\nValue: " + value + "\r\nMethod: SetPercentageModifier");
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="number">
        /// Stat number
        /// </param>
        /// <param name="newValue">
        /// Stat's new value
        /// </param>
        public void SetStatValueByName(int number, uint newValue)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }

                c.Set(newValue);
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + number + " does not exist.\r\nValue: " + newValue + "\r\nMethod: Set");
        }

        /// <summary>
        /// Sets Stat's value
        /// </summary>
        /// <param name="statName">
        /// Stat's name
        /// </param>
        /// <param name="newValue">
        /// Stat's new value
        /// </param>
        public void SetStatValueByName(string statName, uint newValue)
        {
            Contract.Requires(statName != null);
            int statid = StatsList.GetStatId(statName.ToLower());
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }

                c.Set(newValue);
                return;
            }

            throw new StatDoesNotExistException(
                "Stat " + statName + " does not exist.\r\nValue: " + newValue + "\r\nMethod: GetID");
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public void SetTrickle(int statId, int value)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != statId)
                {
                    continue;
                }

                c.Trickle = value;
                return;
            }

            throw new StatDoesNotExistException("Stat " + statId + " does not exist.\r\nMethod: Trickle");
        }

        /// <summary>
        /// </summary>
        /// <param name="statName">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="StatDoesNotExistException">
        /// </exception>
        public int StatIdByName(string statName)
        {
            Contract.Requires(statName != null);
            int statid = StatsList.GetStatId(statName.ToLower());
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }

                return c.StatNumber;
            }

            throw new StatDoesNotExistException("Stat " + statName + " does not exist.\r\nMethod: GetID");
        }

        /// <summary>
        /// Returns Stat's value
        /// </summary>
        /// <param name="number">
        /// Stat number
        /// </param>
        /// <returns>
        /// Stat's value
        /// </returns>
        public int StatValueByName(int number)
        {
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != number)
                {
                    continue;
                }

                return c.Value;
            }

            throw new StatDoesNotExistException("Stat " + number + " does not exist.\r\nMethod: Get");
        }

        /// <summary>
        /// Returns Stat's value
        /// </summary>
        /// <param name="name">
        /// Name of the Stat
        /// </param>
        /// <returns>
        /// Stat's value
        /// </returns>
        public int StatValueByName(string name)
        {
            Contract.Requires(name != null);
            int statid = StatsList.GetStatId(name.ToLower());
            foreach (ClassStat c in this.all)
            {
                if (c.StatNumber != statid)
                {
                    continue;
                }

                return c.Value;
            }

            throw new StatDoesNotExistException("Stat " + name + " does not exist.\r\nMethod: Get");
        }

        /// <summary>
        /// Write all Stats to Sql
        /// </summary>
        public void WriteStatstoSql()
        {
            foreach (ClassStat c in this.all)
            {
                if (c.DoNotDontWriteToSql)
                {
                    continue;
                }

                c.WriteStatToSql(true);
            }
        }

        #endregion
    }

    #endregion
}