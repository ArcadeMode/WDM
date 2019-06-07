using System;
using System.Collections.Generic;
using System.Text;
using GrainInterfaces;

namespace GrainInterfaces.States
{
    [Serializable]
    public class PaymentState
    {
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
