using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Dao
{
    using System.Data;

    using AO.Database.Entities;

    using Dapper;

    public static class OnlineDao
    {
        public static DBOnline IsOnline(int id)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<DBOnline>("SELECT Online from characters WHERE id=@charid", new { charid = id })
                        .First();
            }
        }

        public static void SetOnline(int id)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute("UPDATE characters SET online=1 WHERE id=@charid", new { charid = id });
            }
        }
    }
}
