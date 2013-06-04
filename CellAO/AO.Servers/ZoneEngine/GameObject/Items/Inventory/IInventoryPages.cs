using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items.Inventory
{
    using ZoneEngine.GameObject.Enums;

    public interface IInventoryPages
    {
        IDictionary<int, IInventoryPage> Pages { get; }

        InventoryError AddToPage(int pageNum, int slotNum, IItem item);

        int StandardPage { get; set; }

        IItem RemoveItem(int pageNum, int slotNum);

        bool Read();

        bool Write();

        IInventoryPage this[int index] { get; }
    }
}
