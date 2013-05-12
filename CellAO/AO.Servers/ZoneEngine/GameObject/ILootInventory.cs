using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZoneEngine.GameObject.Items;
namespace ZoneEngine.GameObject
{
    public interface ILootInventory
    {
        IList<AOItem> Inventory
        {
            get;
        }

        bool FillInventory(LootTable lootTable);

    }
}
