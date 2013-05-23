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

namespace ZoneEngine.Network.InternalBus
{
    #region Usings ...

    using System;
    using System.Collections.Generic;

    using MemBus;

    #endregion

    /// <summary>
    /// </summary>
    public class PlayfieldTimedList
    {
        /// <summary>
        /// </summary>
        private readonly List<PlayfieldTimedListEntry> TimerList = new List<PlayfieldTimedListEntry>();

        /// <summary>
        /// </summary>
        private readonly IBus playfieldBus;

        /// <summary>
        /// </summary>
        private bool stillSorted;

        /// <summary>
        /// </summary>
        /// <param name="playfieldTimedListEntry">
        /// </param>
        public void Add(PlayfieldTimedListEntry playfieldTimedListEntry)
        {
            if (playfieldTimedListEntry.Trigger <= DateTime.UtcNow)
            {
                this.playfieldBus.Publish(playfieldTimedListEntry.obj);
                return;
            }

            this.TimerList.Add(playfieldTimedListEntry);
            this.stillSorted = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="triggerTime">
        /// </param>
        /// <param name="obj">
        /// </param>
        public void Add(DateTime triggerTime, object obj)
        {
            if (triggerTime <= DateTime.UtcNow)
            {
                this.playfieldBus.Publish(obj);
                return;
            }

            this.TimerList.Add(new PlayfieldTimedListEntry(triggerTime, obj));
            this.stillSorted = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="milliseconds">
        /// </param>
        /// <param name="obj">
        /// </param>
        public void Add(int milliseconds, object obj)
        {
            this.TimerList.Add(
                new PlayfieldTimedListEntry(DateTime.UtcNow + TimeSpan.FromMilliseconds(milliseconds), obj));
        }

        /// <summary>
        /// </summary>
        public void Process()
        {
            DateTime toCheck = DateTime.UtcNow;
            if (!this.stillSorted)
            {
                this.TimerList.Sort();
                this.stillSorted = true;
            }

            List<object> tempList = new List<object>();
            if (this.TimerList.Count > 0)
            {
                lock (this.TimerList)
                {
                    while ((this.TimerList.Count > 0) && (this.TimerList[0].Trigger < toCheck))
                    {
                        tempList.Add(this.TimerList[0].obj);
                        this.TimerList.RemoveAt(0);
                    }
                }
            }

            foreach (object obj in tempList)
            {
                this.playfieldBus.Publish(obj);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int Count()
        {
            return this.TimerList.Count;
        }

        /// <summary>
        /// </summary>
        /// <param name="bus">
        /// </param>
        public PlayfieldTimedList(IBus bus)
        {
            this.playfieldBus = bus;
            this.stillSorted = false;
        }
    }
}