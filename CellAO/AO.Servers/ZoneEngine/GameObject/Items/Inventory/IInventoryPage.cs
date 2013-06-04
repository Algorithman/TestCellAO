using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items.Inventory
{
    using ZoneEngine.GameObject.Enums;

    public interface IInventoryPage : IEntity
    {
        int Page { get; set; }

        int MaxSlots { get; set; }

        int FirstSlotNumber { get; set; }

        InventoryError Add(int slot, IItem item);

        IItem Remove(int slotNum);

        bool Read();

        bool Write();

        IItem this[int index] { get; }

    }
}
