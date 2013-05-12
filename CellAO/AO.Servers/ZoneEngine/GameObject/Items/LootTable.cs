using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoneEngine.GameObject.Items
{
    public class LootTable
    {
        readonly List<LootTableEntry> LootTableEntries;
        readonly List<AOItem> Items;

        public LootTable()
        {
            this.LootTableEntries = new List<LootTableEntry>();
            this.Items = new List<AOItem>();
        }

        public void AddByHash(string hash)
        {
            // Get access to DB/or file and read the appropriate stuff for the hash
            // Either a AOItem with QL Range (LootTableEntry), or a Hash Group (which lets AddByHash call itself again)
            throw new NotImplementedException("LootTable.AddByHash");
        }
    }

    class LootTableEntry
    {
        readonly int MinQL = 1;
        readonly int MaxQL = 1;
        readonly float DropRate = 1.0f;
        readonly int LowID = 0;
        readonly int HighID = 0;
    }
}