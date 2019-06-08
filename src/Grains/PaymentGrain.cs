using System;
using System.Collections.Generic;
using GrainInterfaces;
using GrainInterfaces.Enums;
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
            State = State.Id != Guid.Empty ? State : new PaymentState
            {
                Id = this.GetPrimaryKey(),
                Status = (int)PaymentStatus.Pending
            };
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        public async Task<PaymentStatus> Pay(IUserGrain user, decimal amount)
        {
            return State.Status = await user.ModifyCredit(-1*amount) ? PaymentStatus.Paid : PaymentStatus.Pending;
        }

        public async Task<PaymentStatus> Cancel()
        {
            return State.Status = PaymentStatus.Cancelled;
        }

        public async Task<PaymentStatus> Status()
        {
            return State.Status;
        }

    }
}
