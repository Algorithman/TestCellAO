using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Dao
{
    using System.Data;

    using AO.Database.Entities;

    using Dapper;

    public static class ItemDao
    {
        public static IEnumerable<DBItem> GetAll()
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<DBItem>("SELECT * FROM items");
            }
        }

        public static DBItem ReadItem(int containerType, int containerInstance, int containerPlacement)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<DBItem>("SELECT * FROM items WHERE containertype=@containerType AND containerinstance=@containerInstance AND containerplacement=@containerPlacement", new { containerType, containerInstance, containerPlacement }).Single();
            }
        }

        public static IEnumerable<DBItem> GetAllInContainer(int containerType, int containerInstance)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<DBItem>("SELECT * FROM items WHERE containertype=@containerType AND containerinstance=@containerInstance", new { containerType, containerInstance });
            }
        }

        public static void Save(DBItem item)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute("REPLACE INTO items VALUES (@containerType, @containerInstance, @containerPlacement, @lowid, @highid, @quality, @multiplecount)", new { item });
            }
        }

        public static void Save(List<DBItem> items)
        {
            if (items.Count == 0)
            {
                return;
            }
            using (IDbConnection conn = Connector.GetConnection())
            {
                using (var trans = conn.BeginTransaction())
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
    }
}
