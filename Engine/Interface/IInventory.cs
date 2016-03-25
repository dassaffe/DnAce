using System.Collections.Generic;
using Engine.Model;

namespace Engine.Interface
{
    internal interface IInventory
    {
         ICollection<Item> Inventory { get; }
    }
}