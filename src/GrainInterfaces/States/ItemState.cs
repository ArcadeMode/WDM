using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.States
{
    [Serializable]
    public class Item
    {
        public Guid ItemID { get; set; }

        public int ItemStock { get; set; }

        public decimal ItemPrice { get; }
    }

    [Serializable]
    public class ItemState
    {
        public Item Value { get; set; }
    }
}