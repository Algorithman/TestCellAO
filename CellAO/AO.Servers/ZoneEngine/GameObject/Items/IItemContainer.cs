using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Items
{
    public interface IItemContainer : IEntity
{
		/// <summary>
		/// The inventory of this Container
		/// </summary>
		BaseInventory BaseInventory { get; }
    }
}
