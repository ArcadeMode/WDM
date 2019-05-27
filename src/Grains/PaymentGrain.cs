using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using GrainInterfaces;
using System.Threading.Tasks;

namespace Grains
{
    public class PaymentGrain : Grain, IPaymentGrain
    {

        private PaymentStatus paymentstatus;

        public PaymentGrain()
        {
            this.paymentstatus = Pending;
        }

        public async Task<PaymentStatus> Pay()
        {
            this.paymentstatus = 
        }

        public async Task<PaymentStatus> Cancel()
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentStatus> Status()
        {
            throw new NotImplementedException();
        } 

    }
}
