using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class OrderGrain: Grain<OrderState>, IOrderGrain
    {
        public Task<bool> AddItem(Guid itemGuid)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> RemoveItem(Guid itemGuid)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CancelOrder()
        {
            await ClearStateAsync();
            return true;
        }

        public Task<bool> Checkout()
        {
            throw new NotImplementedException();
        }
    }

    public class OrderState
    {
    }
}