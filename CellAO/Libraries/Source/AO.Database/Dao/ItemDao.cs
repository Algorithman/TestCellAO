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

namespace AO.Database.Dao
{
    #region Usings ...

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using AO.Database.Entities;

    using Dapper;

    #endregion

    /// <summary>
    /// </summary>
    public static class ItemDao
    {
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static IEnumerable<DBItem> GetAll()
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<DBItem>("SELECT * FROM items");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="containerType">
        /// </param>
        /// <param name="containerInstance">
        /// </param>
        /// <param name="containerPlacement">
        /// </param>
        /// <returns>
        /// </returns>
        public static DBItem ReadItem(int containerType, int containerInstance, int containerPlacement)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBItem>(
                        "SELECT * FROM items WHERE containertype=@containerType AND containerinstance=@containerInstance AND containerplacement=@containerPlacement", 
                        new { containerType, containerInstance, containerPlacement }).Single();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="containerType">
        /// </param>
        /// <param name="containerInstance">
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<DBItem> GetAllInContainer(int containerType, int containerInstance)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBItem>(
                        "SELECT * FROM items WHERE containertype=@containerType AND containerinstance=@containerInstance", 
                        new { containerType, containerInstance });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="item">
        /// </param>
        public static void Save(DBItem item)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute(
                    "REPLACE INTO items VALUES (@containerType, @containerInstance, @containerPlacement, @lowid, @highid, @quality, @multiplecount)", 
                    new { item });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="items">
        /// </param>
        public static void Save(List<DBItem> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            using (IDbConnection conn = Connector.GetConnection())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    conn.Execute(
                        "DELETE FROM items WHERE containertype=@containertype AND containerinstance=@containerinstance", 
                        new { items[0].containertype, items[0].containerinstance }, 
                        transaction: trans);
                    foreach (DBItem item in items)
                    {
                        conn.Execute(
                            "INSERT INTO items (containertype,containerinstance,containerplacement,itemtype,iteminstance"
                            + ",lowid,highid,quality,multiplecount,x,y,z,headingx,headingy,headingz,headingw,stats) VALUES (@conttype,"
                            + " @continstance, @contplacement, @itype, @iinstance, @low, @high, @ql, @mc, @ix, @iy, @iz, @hx, @hy, @hz, @hw, @st)", 
                            new
                                {
                                    conttype = item.containertype, 
                                    continstance = item.containerinstance, 
                                    contplacement = item.containerplacement, 
                                    low = item.lowid, 
                                    high = item.highid, 
                                    ql = item.quality, 
                                    mc = item.multiplecount, 
                                }, 
                            transaction: trans);
                    }

                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="containertype">
        /// </param>
        /// <param name="containerinstance">
        /// </param>
        /// <param name="containerplacement">
        /// </param>
        public static void RemoveItem(int containertype, int containerinstance, int containerplacement)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute(
                    "DELETE FROM items WHERE containertype=@containertype AND containerinstance=@containerinstance AND containerplacement=@containerplacement", 
                    new { containertype, containerinstance, containerplacement });
            }
        }
    }
}