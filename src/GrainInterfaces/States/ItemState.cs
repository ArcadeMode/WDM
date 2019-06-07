using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.States
{

    [Serializable]
    public class ItemState
    {
        public Guid Id { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }
    }
}