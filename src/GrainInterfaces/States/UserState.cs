using System;
using System.Collections.Generic;

namespace GrainInterfaces.States
{
    [Serializable]
    public class UserState
    {
        public ITransactionalState<double> UserBalance { get; set; }
        public List<Guid> CompletedOrderGuids { get; set; }
        public Guid ActiveOrderGuid { get; set; }
    }
}