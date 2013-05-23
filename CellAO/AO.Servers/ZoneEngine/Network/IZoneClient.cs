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

namespace ZoneEngine
{
    #region Usings ...

    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.Messages;

    #endregion

    /// <summary>
    /// </summary>
    [ContractClass(typeof(IZoneClientContract))]
    public interface IZoneClient : IClient
    {
        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        void SendCompressed(MessageBody messageBody);
    }

    /// <summary>
    /// </summary>
    [ContractClassFor(typeof(IZoneClient))]
    internal abstract class IZoneClientContract : IZoneClient
    {
        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        public void SendCompressed(MessageBody messageBody)
        {
            Contract.Requires(messageBody != null);
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public ServerBase Server { get; private set; }

        /// <summary>
        /// </summary>
        public IPAddress ClientAddress { get; private set; }

        /// <summary>
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// </summary>
        public IPEndPoint UdpEndpoint { get; set; }

        /// <summary>
        /// </summary>
        public Socket TcpSocket { get; set; }

        /// <summary>
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void BeginReceive()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="packet">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Send(byte[] packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="packet">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void SendCopy(byte[] packet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="packet">
        /// </param>
        /// <param name="offset">
        /// </param>
        /// <param name="length">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Send(byte[] packet, int offset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="segment">
        /// </param>
        /// <param name="length">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Send(BufferSegment segment, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="host">
        /// </param>
        /// <param name="port">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Connect(string host, int port)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="addr">
        /// </param>
        /// <param name="port">
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Connect(IPAddress addr, int port)
        {
            throw new NotImplementedException();
        }
    }
}