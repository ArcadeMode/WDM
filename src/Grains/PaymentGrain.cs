using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName = "PaymentStorage")]
    public class PaymentGrain : Grain<PaymentState>, IPaymentGrain
    {

        public async Task<int> Pay()
        {
            await ReadStateAsync();

            CreatePaymentIfNeeded(State);
            State.Value.PaymentStatus = (int)PaymentStatusEnum.Paid;

            await WriteStateAsync();
            return State.Value.PaymentStatus;
        }

        public async Task<int> Cancel()
        {
            await ReadStateAsync();

            CreatePaymentIfNeeded(State);
            State.Value.PaymentStatus = (int)PaymentStatusEnum.Cancelled;

            await WriteStateAsync();
            return State.Value.PaymentStatus;
        }

        public async Task<int> Status()
        {
            await ReadStateAsync();
            CreatePaymentIfNeeded(State);
            return State.Value.PaymentStatus;
        }

        private void CreatePaymentIfNeeded(PaymentState State)
        {
            if (State.Value == null)
            {
                State.Value = new Payment
                {
                    PaymentID = this.GetPrimaryKey(),
                    PaymentStatus = (int)PaymentStatusEnum.Pending
                };
            }
        }

    }
}
