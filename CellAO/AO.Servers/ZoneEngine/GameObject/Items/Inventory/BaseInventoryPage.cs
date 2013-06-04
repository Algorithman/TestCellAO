using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items.Inventory
{
    using System.Data.Linq;

    using AO.Database;
    using AO.Database.Dao;
    using AO.Database.Entities;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Enums;

    public abstract class BaseInventoryPage : IInventoryPage
    {
        private IDictionary<int, IItem> Content;

        public Identity Identity { get; set; }

        public int Page { get; set; }

        public int MaxSlots { get; set; }

        public int FirstSlotNumber { get; set; }

        public virtual InventoryError Add(int slot, IItem item)
        {
            throw new NotImplementedException();
        }

        public IItem Remove(int slotNum)
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            foreach (DBItem item in ItemDao.GetAllInContainer((int)this.Identity.Type, this.Identity.Instance))
            {
                Item newItem = new Item(item.quality, item.lowid, item.highid);
                newItem.SetAttribute(212, item.multiplecount);
                Content.Add(item.containerplacement,newItem);
            }

            foreach (
                DBInstancedItem item in
                    InstancedItemDao.GetAllInContainer((int)this.Identity.Type, this.Identity.Instance))
            {
                Item newItem = new Item(item.quality, item.lowid, item.highid);
                newItem.SetAttribute(212, item.multiplecount);
                Identity temp = new Identity();
                temp.Type = (IdentityType)item.itemtype;
                temp.Instance = item.iteminstance;
                newItem.Identity = temp;

                byte[] binaryStats = item.stats.ToArray();
                for (int i = 0; i < binaryStats.Length / 8; i++)
                {
                    int statid = BitConverter.ToInt32(binaryStats, i * 8);
                    int statvalue = BitConverter.ToInt32(binaryStats, i * 8 + 4);
                    newItem.SetAttribute(statid, statvalue);
                }
            }
            return true;
        }

        public bool Write()
        {
            List<DBInstancedItem> DBinstanced = new List<DBInstancedItem>();
            List<DBItem> DBuninstanced = new List<DBItem>();
            foreach (KeyValuePair<int, IItem> kv in Content)
            {
                if (kv.Value.Identity.Type != IdentityType.None)
                {
                    DBInstancedItem dbi = new DBInstancedItem();
                    dbi.containerinstance = this.Identity.Instance;
                    dbi.containertype = (int)this.Identity.Type;
                    dbi.containerplacement = kv.Key;

                    dbi.itemtype = (int)kv.Value.Identity.Type;
                    dbi.iteminstance = kv.Value.Identity.Instance;
                    dbi.lowid = kv.Value.LowID;
                    dbi.highid = kv.Value.HighID;
                    dbi.quality = kv.Value.Quality;
                    dbi.multiplecount = kv.Value.MultipleCount;

                    dbi.stats=new Binary(kv.Value.GetItemAttributes());

                    DBinstanced.Add(dbi);
                }
                else
                {
                    DBItem dbi = new DBItem();
                    dbi.containerinstance = this.Identity.Instance;
                    dbi.containertype = (int)this.Identity.Type;
                    dbi.containerplacement = kv.Key;

                    dbi.lowid = kv.Value.LowID;
                    dbi.highid = kv.Value.HighID;
                    dbi.quality = kv.Value.Quality;
                    dbi.multiplecount = kv.Value.MultipleCount;
                    DBuninstanced.Add(dbi);
                }
            }
            ItemDao.Save(DBuninstanced);
            InstancedItemDao.Save(DBinstanced);
            return true;
        }

        public IItem this[int index]
        {
            get
            {
                if (this.Content.ContainsKey(index))
                {
                    return this.Content[index];
                }
                return null;
            }
        }

        public BaseInventoryPage(int pagenum, int maxslots, int firstslotnumber)
        {
            this.Page = pagenum;
            this.MaxSlots = maxslots;
            this.FirstSlotNumber = firstslotnumber;
            this.Content = new Dictionary<int, IItem>();
        }
    }
}
