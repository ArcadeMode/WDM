using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.States
{

    [Serializable]
    public class OrderState
    {
        public Guid Id { get; set; }

        public Dictionary<IItemGrain, int> Items { get; set; }

        public IUserGrain User { get; set; }

        public decimal Total
        {
            get
            {
                //TODO: should be changed to get all items from the list of Ids, and then sum the price.
                return -1m;
            }
        }
    }
}