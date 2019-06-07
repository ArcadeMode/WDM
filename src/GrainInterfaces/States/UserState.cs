using System;
using System.Collections.Generic;
using Orleans.Transactions.Abstractions;

namespace GrainInterfaces.States
{
    [Serializable]
    public class UserState
    {
        public decimal Balance { get; set; }
    }
}