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

    using System.Diagnostics.Contracts;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Playfields;

    #endregion

    /// <summary>
    /// </summary>
    [ContractClass(typeof(IInstancedEntityContract))]
    public interface IInstancedEntity : IEntity
    {
        #region Public Properties

        /// <summary>
        /// </summary>
        IPlayfield Playfield { get; set; }

        Vector3 Coordinates { get; set; }
        Quaternion Heading { get; set; }

        #endregion
    }

    /// <summary>
    /// </summary>
    [ContractClassFor(typeof(IInstancedEntity))]
    internal abstract class IInstancedEntityContract : IInstancedEntity
    {
        /// <summary>
        /// </summary>
        public Identity Identity
        {
            get
            {
                return default(Identity);
            }

            private set
            {
            }
        }

        /// <summary>
        /// </summary>
        public IPlayfield Playfield
        {
            get
            {
                Contract.Ensures(Contract.Result<IPlayfield>() != null);
                return default(IPlayfield);
            }

            set
            {
                Contract.Requires(value != null);
            }
        }

        public Vector3 Coordinates
        {
            get
            {
                Contract.Ensures(Contract.Result<Vector3>() != null);
                return default(Vector3);
            }
            set
            {
                Contract.Requires(value != null);
            }
        }

        public Quaternion Heading
        {
            get
            {
                Contract.Ensures(Contract.Result<Quaternion>() != null);
                return default(Quaternion);
            }
            set
            {
                Contract.Requires(value != null);
            }
        }
    }
}