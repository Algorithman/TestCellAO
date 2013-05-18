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

namespace ZoneEngine.Network
{
    #region Usings ...

    using System;
    using System.Threading;

    using AO.Core.Logger;

    using ZoneEngine.GameObject.Playfields;

    #endregion

    /// <summary>
    /// </summary>
    public class PlayfieldWorker
    {

        /// <summary>
        /// </summary>
        public IPlayfield playfield;

        /// <summary>
        /// </summary>
        public void DoWork()
        {
            // TODO: Load Mobs/Characters/Statels HERE
            LogUtil.Debug("Created playfield " + this.playfield.Identity.Instance.ToString());
            while (!this._shouldStop)
            {
                // TODO: Add message processing here
                Thread.Sleep(10);
            }
            playfield.DisconnectAllClients();
            LogUtil.Debug("Stopped playfield " + this.playfield.Identity.Instance.ToString());
        }

        /// <summary>
        /// </summary>
        public void RequestStop()
        {
            this._shouldStop = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="playfield">
        /// </param>
        public void SetPlayfield(IPlayfield playfield)
        {
            this.playfield = playfield;
        }

        /// <summary>
        /// </summary>
        private volatile bool _shouldStop;
    }
    public class PlayfieldWorkerHolder
    {
        public Thread thread;

        public PlayfieldWorker PlayfieldWorker;

        public PlayfieldWorkerHolder()
        {
            this.PlayfieldWorker = new PlayfieldWorker();
        }
    }
}