using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.States
{

    [Serializable]
    public class OrderState
    {
        public Guid Id { get; set; }

        public Dictionary<Guid, int> Items { get; set; }

        public Guid UserId { get; set; }

        public Guid PaymentId { get; set; }

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