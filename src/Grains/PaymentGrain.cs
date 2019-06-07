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
        public override async Task OnActivateAsync()
        {
            await ReadStateAsync();
            CreatePaymentIfNeeded(State);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        public async Task<PaymentStatus> Pay()
        {
            State.Status = PaymentStatus.Paid;
            return State.Status;
        }

        public async Task<PaymentStatus> Cancel()
        {
            State.Status = PaymentStatus.Cancelled;
            return State.Status;
        }

        public async Task<PaymentStatus> Status()
        {
            return State.Status;
        }

        private void CreatePaymentIfNeeded(PaymentState PState)
        {
            State = PState ?? new PaymentState
            {
                Id = this.GetPrimaryKey(),
                Status = (int)PaymentStatus.Pending
            };
        }

    }
}
