using System;

namespace GrainInterfaces.States
{
    [Serializable]
    public class UserState
    {
        public Guid Id { get; set; }
        public decimal UserBalance { get; set; }
       
    }
}