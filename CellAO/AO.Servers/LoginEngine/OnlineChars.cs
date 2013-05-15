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

namespace LoginEngine
{
    #region Usings ...

    using System;
    using System.Data;

    using AO.Core;

    #endregion

    /// <summary>
    /// </summary>
    public static class OnlineChars
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        public static void Initialize()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="CharacterDoesNotExistException">
        /// </exception>
        public static bool IsOnline(int id)
        {
            var sql = new SqlWrapper();
            DataTable dt = sql.ReadDatatable("SELECT * FROM characters WHERE ID = " + id + ";");
            if (dt.Rows.Count == 0)
            {
                throw new CharacterDoesNotExistException("Character does not exist: " + id);
            }

            if ((Int16)dt.Rows[0]["Online"] == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        public static void SetOnline(int id)
        {
            var sql = new SqlWrapper();
            sql.SqlUpdate("UPDATE characters SET Online = 1 WHERE ID = " + id + ";");
        }

        #endregion
    }
}