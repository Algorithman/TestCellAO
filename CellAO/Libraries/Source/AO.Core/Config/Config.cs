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

namespace AO.Core.Config
{

    #region enum

    /// <summary>
    /// This is the Enums for the Config File..
    /// </summary>
    public enum Information
    {
        /// <summary>
        /// IP to listen
        /// </summary>
        ListenIP, 

        /// <summary>
        /// Chat Server IP
        /// </summary>
        ChatIP, 

        /// <summary>
        /// Zone Server IP
        /// </summary>
        ZoneIP, 

        /// <summary>
        /// Communication Port between Zone and Chat Engines..
        /// </summary>
        CommPort, 

        /// <summary>
        /// Login Port Number
        /// </summary>
        LoginPort, 

        /// <summary>
        /// Zone Port Number
        /// </summary>
        ZonePort, 

        /// <summary>
        /// Chat Port Number
        /// </summary>
        ChatPort, 

        /// <summary>
        /// Chat Server's Motd
        /// </summary>
        Motd, 

        /// <summary>
        /// This is for the LoginEncryption to turn it on and off
        /// </summary>
        UsePassword, 

        /// <summary>
        /// This enables or Disables SQL Logging
        /// </summary>
        SqlLog, 

        /// <summary>
        /// This is for turning the chat logging on and off
        /// </summary>
        LogChat, 

        /// <summary>
        /// This is the Sql Connection String
        /// </summary>
        connectionString, 

        // This is for Grid Skill Checks
        /// <summary>
        /// Comp. Lit. Check to access the West Athen Grid
        /// </summary>
        WestAthen, 

        /// <summary>
        /// Comp. Lit. check to access the Old Athen Grid
        /// </summary>
        OldAthen, 

        /// <summary>
        /// Comp. Lit. check to access the Tir Grid
        /// </summary>
        Tir_Inside, 

        /// <summary>
        /// 
        /// </summary>
        Tir_Outside, 

        /// <summary>
        /// 
        /// </summary>
        Borealis, 

        /// <summary>
        /// 
        /// </summary>
        Meetmedeere, 

        /// <summary>
        /// 
        /// </summary>
        Newland, 

        /// <summary>
        /// 
        /// </summary>
        Omni_One_Trade, 

        /// <summary>
        /// 
        /// </summary>
        Rome_Red, 

        /// <summary>
        /// 
        /// </summary>
        Omni_One_Entertainment_North, 

        /// <summary>
        /// 
        /// </summary>
        Omni_One_Entertainment_South, 

        /// <summary>
        /// 
        /// </summary>
        Lush_Hills, 

        /// <summary>
        /// 
        /// </summary>
        Clondyke, 

        /// <summary>
        /// 
        /// </summary>
        Galway_County, 

        /// <summary>
        /// 
        /// </summary>
        Broken_Shores, 

        /// <summary>
        /// 
        /// </summary>
        four_holes, 

        /// <summary>
        /// 
        /// </summary>
        twoho, 

        /// <summary>
        /// 
        /// </summary>
        Harrys, 

        /// <summary>
        /// 
        /// </summary>
        Omni_One_HQ, 

        /// <summary>
        /// 
        /// </summary>
        Camelot, 

        /// <summary>
        /// 
        /// </summary>
        Sentinels, 

        /// <summary>
        /// 
        /// </summary>
        Teleport_Tower_noob_right, 

        /// <summary>
        /// 
        /// </summary>
        Teleport_Tower_noob_middle, 

        /// <summary>
        /// 
        /// </summary>
        Teleport_Tower_noob_left, 

        /// <summary>
        /// 
        /// </summary>
        ISCommLocalIP, 

        /// <summary>
        /// This is for the SQL Choices
        /// </summary>
        SQLType, 

        /// <summary>
        /// Locale language selection for you, en = english , gr = german, more languages as we go
        /// </summary>
        Locale, 
    }

    #endregion

    #region Config

    /// <summary>
    /// This here Sets up the Gets and Sets for the Config System.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or Sets the Mysql Connection String...
        /// </summary>
        public string MysqlConnection { get; set; }

        /// <summary>
        /// </summary>
        /// <summery>
        /// Gets or sets the MsSQL Connection String...
        /// </summery>
        public string MsSqlConnection { get; set; }

        /// <summary>
        /// </summary>
        /// <summery>
        /// Gets or sets the  PostgreSQL Connection String..
        /// </summery>
        public string PostgreConnection { get; set; }

        /// <summary>
        /// Gets or Sets the Login Port
        /// </summary>
        public int LoginPort { get; set; }

        /// <summary>
        /// Gets or Sets Communication Port for Communicating between Zone and Chat Engines.
        /// </summary>
        public int CommPort { get; set; }

        /// <summary>
        /// Gets or Sets IP to listen
        /// </summary>
        public string ListenIP { get; set; }

        /// <summary>
        /// Gets or Sets Chat server IP
        /// </summary>
        public string ChatIP { get; set; }

        /// <summary>
        /// Gets or Sets Zone server IP
        /// </summary>
        public string ZoneIP { get; set; }

        /// <summary>
        /// Gets or Sets the Motd for Chat Server
        /// </summary>
        public string Motd { get; set; }

        /// <summary>
        /// Gets or Sets the Zone Port
        /// </summary>
        public int ZonePort { get; set; }

        /// <summary>
        /// Gets or Sets the Chat Port
        /// </summary>
        public int ChatPort { get; set; }

        /// <summary>
        /// Turns on or off Sql Logging
        /// </summary>
        public bool SqlLog { get; set; }

        /// <summary>
        /// turns on or off the chat logging
        /// </summary>
        public bool LogChat { get; set; }

        /// <summary>
        /// Gets or Sets the UsePassword
        /// </summary>
        public bool UsePassword { get; set; }

        // Grid use checks
        /// <summary>
        /// Gets or Sets the West Athen Comp. Lit. Check Number
        /// </summary>
        public int WestAthen { get; set; }

        /// <summary>
        /// Gets or Sets the Old Athen Comp. Lit. Check Number
        /// </summary>
        public int OldAthen { get; set; }

        /// <summary>
        /// Gets or Sets the Tir Inside Comp. Lit. Check Number
        /// </summary>
        public int Tir_Inside { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Tir_Outside { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Borealis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Meetmedeere { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Newland { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Omni_One_Trade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Rome_Red { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Omni_One_Entertainment_North { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Omni_One_Entertainment_South { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Lush_Hills { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Clondyke { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Galway_County { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Broken_Shores { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int four_holes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int twoho { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Harrys { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Omni_One_HQ { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Camelot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Sentinels { get; set; }

        /// <summary>
        /// Local ISComm IP address
        /// </summary>
        public string ISCommLocalIP { get; set; }

        /// <summary>
        /// Gets or Sets Your SQL Type
        /// </summary>
        public string SQLType { get; set; }

        /// <summary>
        /// Gets or Sets your Locale language
        /// </summary>
        public string Locale { get; set; }
    }

    #endregion
}