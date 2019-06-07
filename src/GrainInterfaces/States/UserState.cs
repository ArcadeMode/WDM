using System;
using System.Collections.Generic;
using Orleans.Transactions.Abstractions;

namespace GrainInterfaces.States
{
    [Serializable]
    public class UserState
    {
        public ITransactionalState<Balance> UserBalance { get; set; }
        public List<Guid> OrderGuids { get; set; }
    }
    
    [Serializable]
    public class Balance
    {
        public decimal Value { get; set; }
    }
}