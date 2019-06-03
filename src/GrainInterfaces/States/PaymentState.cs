using System;
using System.Collections.Generic;
using System.Text;
using GrainInterfaces;

namespace GrainInterfaces.States
{
    [Serializable]
    public class Payment
    {
        public Guid PaymentID { get; set; }

        public int PaymentStatus { get; set; }

    }

    [Serializable]
    public class PaymentState
    {
        public Payment Value { get; set; }
    }
}
