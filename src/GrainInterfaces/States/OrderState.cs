using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.States
{
    [Serializable]
    public class Order
    {
        public Guid Id { get; set; }
        
        public List<int> Items { get; set; }

        //TODO: should be changed to get all items from the list of Ids, and then sum the price.
        // public decimal Total
        // {
        //     get
        //     {
        //         return Items?.Sum(_ => _.Price) ?? 0;
        //     }
        // }
    }

    [Serializable]
    public class OrderState
    {
        public Order Value { get; set; }
    }
}