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

    #endregion

    /// <summary>
    /// </summary>
    public class StatNextXP : DynelStat
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <param name="sendBaseValue">
        /// </param>
        /// <param name="doNotWrite">
        /// </param>
        /// <param name="announceToPlayfield">
        /// </param>
        public StatNextXP(
            int number, uint defaultValue, bool sendBaseValue, bool doNotWrite, bool announceToPlayfield)
        {
            this.StatId = number;
            this.DefaultValue = defaultValue;

            this.BaseValue = this.DefaultValue;
            this.SendBaseValue = sendBaseValue;
            this.DoNotDontWriteToSql = doNotWrite;
            this.AnnounceToPlayfield = announceToPlayfield;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public override int Value
        {
            get
            {
                int level = this.Parent.Stats["Level"].Value;
                if (level >= 200)
                {
                    return 0;
                }

                return Convert.ToInt32(XPTable.TableRKXP[level - 1, 2]);
            }

            set
            {
                Set(value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        public override void CalcTrickle()
        {
            base.CalcTrickle();
            this.Set(this.Value);
        }

        #endregion
    }
}