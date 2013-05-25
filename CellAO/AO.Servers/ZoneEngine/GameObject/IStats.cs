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

    using ZoneEngine.GameObject.Items;
    using ZoneEngine.GameObject.Playfields;
    using ZoneEngine.GameObject.Stats;

    #endregion

    /// <summary>
    /// </summary>
    [ContractClass(typeof(IStatsContract))]
    public interface IStats
    {
        #region Public Properties

        /// <summary>
        /// </summary>
        DynelStats Stats { get; }

        /// <summary>
        /// </summary>
        /// <param name="aof">
        /// </param>
        /// <param name="checkAll">
        /// </param>
        /// <returns>
        /// </returns>
        bool CheckRequirements(AOFunctions aof, bool checkAll);

        #endregion
    }

    /// <summary>
    /// </summary>
    [ContractClassFor(typeof(IStats))]
    internal abstract class IStatsContract : IStats
    {
        /// <summary>
        /// </summary>
        public DynelStats Stats
        {
            get
            {
                Contract.Ensures(this.Stats != null);
                return default(DynelStats);
            }

            private set
            {
                Contract.Requires(value != null);
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool CheckRequirements(AOFunctions aof, bool checkAll)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public Identity Identity { get; private set; }

        /// <summary>
        /// </summary>
        public IPlayfield Playfield { get; set; }

        /// <summary>
        /// </summary>
        public Vector3 Coordinates { get; set; }

        /// <summary>
        /// </summary>
        public Quaternion Heading { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        public string LastName { get; set; }
    }
}