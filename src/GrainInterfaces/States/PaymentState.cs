using System;
using System.Collections.Generic;
using System.Text;
using GrainInterfaces;
using GrainInterfaces.Enums;

namespace GrainInterfaces.States
{
    [Serializable]
    public class PaymentState
    {
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
