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

namespace Database
{
    #region Usings ...

    using System.Collections.Generic;
    using System.Data;

    using Dapper;

    #endregion

    /// <summary>
    /// </summary>
    public static class DBStats
    {
        /// <summary>
        /// </summary>
        /// <param name="characterId">
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<StatDao> GetById(int characterId)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<StatDao>(
                    "SELECT Stat, Value FROM characters_stats where id=@id", new { id = characterId });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="characterId">
        /// </param>
        /// <param name="statNumber">
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<StatDao> GetById(int characterId, int statNumber)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<StatDao>(
                    "SELECT Stat, Value FROM characters_stats where id=@id AND Stat=@statnum", 
                    new { id = characterId, statnum = statNumber });
            }
        }
    }
}