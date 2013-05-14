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

namespace ZoneEngine.GameObject.Stats
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Xml.Serialization;

    using AO.Core;

    #endregion

    #region StatTypes Class

    /// <summary>
    /// 
    /// </summary>
    public class StatTypes
    {
        #region Fields

        /// <summary>
        /// </summary>
        [XmlElement("Default")]
        public int defaultValue = 1234567890;

        /// <summary>
        /// </summary>
        [XmlElement("FullName")]
        public string statFullName = string.Empty;

        /// <summary>
        /// </summary>
        [XmlAttribute("id")]
        public int statId;

        /// <summary>
        /// </summary>
        [XmlAttribute("Name")]
        public string statName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 
        /// </summary>
        public StatTypes()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <param name="statName">
        /// </param>
        public StatTypes(int statId, string statName)
        {
            this.statId = statId;
            this.statName = statName;
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <param name="statName">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        public StatTypes(int statId, string statName, int defaultValue)
            : this(statId, statName)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <param name="statName">
        /// </param>
        /// <param name="statFullName">
        /// </param>
        public StatTypes(int statId, string statName, string statFullName)
            : this(statId, statName)
        {
            this.statFullName = statFullName;
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <param name="statName">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <param name="statFullName">
        /// </param>
        public StatTypes(int statId, string statName, int defaultValue, string statFullName)
            : this(statId, statName, statFullName)
        {
            this.defaultValue = defaultValue;
        }

        #endregion
    }

    #endregion

    #region StatsList Class

    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Stats")]
    public class StatsList
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        [XmlIgnore]
        public static readonly StatsList Instance;

        #endregion

        #region Fields

        /// <summary>
        /// </summary>
        [XmlElement("Stat")]
        public List<StatTypes> stats;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        static StatsList()
        {
            Instance = LoadXML(Path.Combine("XML Data", "Stats.xml"));
        }

        /// <summary>
        /// </summary>
        private StatsList()
        {
        }

        #endregion

        // Generally this shouldn't be used outside of the static constructor

        // This really should only be used for development. Included for completeness.
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="fileName">
        /// </param>
        public static void DumpXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StatsList));
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, Instance, xsn);
            writer.Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <returns>
        /// </returns>
        public static int GetDefaultValue(int statId)
        {
            foreach (StatTypes stat in Instance.stats)
            {
                if (stat.statId == statId)
                {
                    return stat.defaultValue;
                }
            }

            Console.WriteLine("Using generic DefaultValue for statId " + statId);

            // Generic default value
            return 1234567890;
        }

        /// <summary>
        /// </summary>
        /// <param name="statName">
        /// </param>
        /// <returns>
        /// </returns>
        public static StatTypes GetStatByName(string statName)
        {
            if (statName == string.Empty)
            {
                throw new ArgumentNullException("statName cannot be null or empty");
            }

            foreach (StatTypes stat in Instance.stats)
            {
                if (stat.statName.ToLower() == statName.ToLower())
                {
                    return stat;
                }
            }

            throw new StatDoesNotExistException("Stat with name '" + statName + "' does not exist.");
        }

        /// <summary>
        /// </summary>
        /// <param name="statName">
        /// </param>
        /// <returns>
        /// </returns>
        public static int GetStatId(string statName)
        {
            if (statName == string.Empty)
            {
                return 1234567890;
            }

            try
            {
                // Are we passed a StatId? If so, return it as an int
                int statId = int.Parse(statName);

                if (statId >= 0)
                {
                    return statId;
                }
            }
            catch (Exception)
            {
            }

            foreach (StatTypes stat in Instance.stats)
            {
                if (stat.statName.ToLower() == statName.ToLower())
                {
                    return stat.statId;
                }
            }

            Console.WriteLine("Unknown statName: " + statName);

            return 1234567890;
        }

        /// <summary>
        /// </summary>
        /// <param name="statId">
        /// </param>
        /// <returns>
        /// </returns>
        public static string GetStatName(int statId)
        {
            foreach (StatTypes stat in Instance.stats)
            {
                if (stat.statId == statId)
                {
                    return stat.statName;
                }
            }

            Console.Write("Unknown statId: " + statId);

            return "UnknownStat#" + statId;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName">
        /// </param>
        /// <returns>
        /// </returns>
        public static StatsList LoadXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StatsList));
            TextReader reader = new StreamReader(fileName);
            StatsList data = (StatsList)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// </summary>
    internal class OrgMisc
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="orgId">
        /// </param>
        /// <returns>
        /// </returns>
        public static List<int> GetOrgMembers(uint orgId)
        {
            return GetOrgMembers(orgId, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="orgId">
        /// </param>
        /// <param name="excludePresident">
        /// </param>
        /// <returns>
        /// </returns>
        public static List<int> GetOrgMembers(uint orgId, bool excludePresident)
        {
            // Stat #5 == Clan == OrgID
            // Stat #48 == ClanLevel == Org Rank (0 is president)
            SqlWrapper mySql = new SqlWrapper();
            List<int> orgMembers = new List<int>();
            string pres = string.Empty;

            if (excludePresident)
            {
                pres = " AND `ID` NOT IN (SELECT `ID` FROM `characters_stats` WHERE `Stat` = '48' AND `Value` = '0')";
            }

            DataTable dt =
                mySql.ReadDatatable(
                    "SELECT `ID` FROM `characters_stats` WHERE `Stat` = '5' AND `Value` = '" + orgId + "'" + pres);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    orgMembers.Add((Int32)row[0]);
                }
            }

            return orgMembers;
        }

        #endregion
    }
}