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

namespace AO.Database
{
    #region Usings ...

    using System.Collections.Generic;
    using System.Data;

    using Dapper;

    #endregion

    /// <summary>
    /// </summary>
    public class CharacterDao
    {
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<DBCharacter> GetAll()
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBCharacter>(
                        "SELECT Name, FirstName, LastName, Textures0,Textures1,Textures2,Textures3,Textures4,playfield as Playfield, X,Y,Z,HeadingX,HeadingY,HeadingZ,HeadingW FROM characters");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="characterId">
        /// </param>
        /// <returns>
        /// </returns>
        public IEnumerable<DBCharacter> GetById(int characterId)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBCharacter>(
                        "SELECT Name, FirstName, LastName, Textures0,Textures1,Textures2,Textures3,Textures4,playfield as Playfield, X,Y,Z,HeadingX,HeadingY,HeadingZ,HeadingW FROM characters where id = @id", 
                        new { id = characterId });
            }
        }
    }
}