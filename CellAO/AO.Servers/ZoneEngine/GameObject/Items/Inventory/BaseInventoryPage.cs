using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items.Inventory
{
    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.GameObject.Enums;

    public abstract class BaseInventoryPage : IInventoryPage
    {
        private IDictionary<int, IItem> Content;

        public Identity Identity { get; private set; }

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
            throw new NotImplementedException();
        }

        public bool Write()
        {
            throw new NotImplementedException();
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
